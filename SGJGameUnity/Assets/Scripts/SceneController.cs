using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    public static bool SelectInGameMenu = true;
    public static bool GameIsPaused = true;
    public static bool ShowNewMenu = true;
    public static bool ShowSaveLoadMenus = false;
    public static bool ShowEndMenu = true;

    public event Action Saving;
    public event Action Loading;

    public event Action BeforeSceneUnload;
    public event Action AfterSceneLoad;

    public CanvasGroup faderCanvasGroup;
    public float fadeDuration = 1f;
    public string startingSceneName = "MainMenu";
    public string[] gameLevels = { "GameStart", "HardLevel", "Congratulations" };
    private string currentSceneName;
    public SaveData playerSaveData;

    private bool isFading;

    public static void ShowPauseMenu(bool showNew, bool showSaveLoad, bool showEnd)
    {
        SelectInGameMenu = true;
        GameIsPaused = true;
        ShowNewMenu = true;// showNew;
        ShowSaveLoadMenus = false; // showSaveLoad;
        ShowEndMenu = showEnd;
    }

    public void NewGame()
    {
        if (isFading)
        {
            Debug.Log("Menu is fading. Ignoring New game.");
            return;
        }

        StartCoroutine(FadeAndSwitchScenes(gameLevels[0], () => {
            playerSaveData.Reset();
            ShowPauseMenu(true, true, true);
            GameIsPaused = false;
        }));
    }

    public void EndGame()
    {
        if (isFading)
        {
            Debug.Log("Menu is fading. Ignoring End game.");
            return;
        }

        StartCoroutine(FadeAndSwitchScenes(startingSceneName, () => {
            playerSaveData.Reset();
            ShowPauseMenu(true, false, true);
        }));
    }

    public void LoadGame()
    {
        if (isFading)
        {
            Debug.Log("Menu is fading. Ignoring Load game.");
            return;
        }

        Debug.Log(Application.persistentDataPath);
        if (!File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            Debug.Log("Save game does not exist.");
            return;
        }

        var json = File.ReadAllText(Application.persistentDataPath + "/gamesave.save");
        var saveData = JsonUtility.FromJson<SaveData>(json);

        string sceneName = gameLevels[0];
        saveData.Load("currentScene", ref sceneName);

        StartCoroutine(FadeAndSwitchScenes(sceneName, () => {
            playerSaveData.Assign(saveData);
            ShowPauseMenu(true, true, true);
            GameIsPaused = false;
        }));
    }

    public void SaveGame()
    {
        if (Saving != null)
            Saving();

        string json = JsonUtility.ToJson(playerSaveData, true);
        File.WriteAllText(Application.persistentDataPath + "/gamesave.save", json);
        ShowPauseMenu(true, true, true);
        GameIsPaused = false;
    }

    public void NextLevel()
    {
        for (var i = 0; i < gameLevels.Length - 1; ++i)
        {
            if (gameLevels[i] == currentSceneName)
            {
                var levelName = gameLevels[i + 1];
                Debug.Log("Loading level: " + levelName);
                FadeAndLoadScene(levelName);
                return;
            }
        }
        Debug.Log("There is no next level to load.");
    }

    void Awake()
    {
        if (SceneController.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        SceneController.instance = this;
    }

    private IEnumerator Start ()
    {
        faderCanvasGroup.alpha = 1f;
        // playerSaveData.Save (PlayerMovement.startingPositionKey, initialStartingPositionName);
        yield return StartCoroutine (LoadSceneAndSetActive (startingSceneName));
        StartCoroutine (Fade (0f));
    }

    public void FadeAndLoadScene (string sceneName)
    {
        if (!isFading)
        {
            StartCoroutine (FadeAndSwitchScenes (sceneName, () => { }));
        }
    }

    private IEnumerator FadeAndSwitchScenes (string sceneName, Action doLoading)
    {
        yield return StartCoroutine (Fade (1f));
        if (Saving != null)
            Saving();

        if (BeforeSceneUnload != null)
            BeforeSceneUnload();

        yield return SceneManager.UnloadSceneAsync (SceneManager.GetActiveScene ().buildIndex);

        doLoading();

        yield return StartCoroutine (LoadSceneAndSetActive (sceneName));

        if (AfterSceneLoad != null)
            AfterSceneLoad ();

        if (Loading != null)
            Loading();

        yield return StartCoroutine (Fade (0f));
    }
    private IEnumerator LoadSceneAndSetActive (string sceneName)
    {
        currentSceneName = sceneName;
        playerSaveData.Save("currentScene", sceneName);
        yield return SceneManager.LoadSceneAsync (sceneName, LoadSceneMode.Additive);
        Scene newlyLoadedScene = SceneManager.GetSceneAt (SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene (newlyLoadedScene);
    }
    private IEnumerator Fade (float finalAlpha)
    {
        //Time.timeScale = 1.0f;
        isFading = true;
        Debug.Log("Fading");

        faderCanvasGroup.blocksRaycasts = true;
        float fadeSpeed = Mathf.Abs (faderCanvasGroup.alpha - finalAlpha) / fadeDuration;
        while (!Mathf.Approximately (faderCanvasGroup.alpha, finalAlpha))
        {
            faderCanvasGroup.alpha = Mathf.MoveTowards (faderCanvasGroup.alpha, finalAlpha,
                fadeSpeed * Time.deltaTime);
            yield return null;
        }

        isFading = false;
        Debug.Log("Not Fading");

        faderCanvasGroup.blocksRaycasts = false;
    }
}
