using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;

public class SceneController : MonoBehaviour
{
    public event Action Saving;
    public event Action Loading;

    public event Action BeforeSceneUnload;
    public event Action AfterSceneLoad;

    public CanvasGroup faderCanvasGroup;
    public float fadeDuration = 1f;
    public string startingSceneName = "MainMenu";
    public string newGameSceneName = "GameStart";
    public SaveData playerSaveData;

    public void NewGame()
    {
        if (isFading)
        {
            Debug.Log("Menu is fading. Ignoring New game.");
            return;
        }

        StartCoroutine(FadeAndSwitchScenes(newGameSceneName, () => {
            playerSaveData.Reset();
            IngameMenu.GameIsPaused = false;
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
            IngameMenu.GameIsPaused = false;
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

        string sceneName = newGameSceneName;
        saveData.Load("currentScene", ref sceneName);

        StartCoroutine(FadeAndSwitchScenes(sceneName, () => {
            playerSaveData.Assign(saveData);
            IngameMenu.GameIsPaused = false;
        }));
    }

    public void SaveGame()
    {
        if (Saving != null)
            Saving();

        string json = JsonUtility.ToJson(playerSaveData, true);
        File.WriteAllText(Application.persistentDataPath + "/gamesave.save", json);
        IngameMenu.GameIsPaused = false;
    }

    private bool isFading;
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
