using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoresScript : MonoBehaviour {

    public AudioClip winMusic;
    public AudioClip lostMusic;

    private Dictionary<Hitable.HitableType, int> currentNumberOfHitables = new Dictionary<Hitable.HitableType, int>();
    private Dictionary<Hitable.HitableType, int> totalNumberOfHitables = new Dictionary<Hitable.HitableType, int>();

    PlayerHitable playerHitable;

    private Canvas diedCanvas;
    private Canvas scoresCanvas;
    private Text targetsText;
    private Text targetsTextHud;
    private Text enemiesText;
    private Text objectsText;
    private Text totalTargetsText;
    private Text totalTargetsTextHud;
    private Text totalEnemiesText;
    private Text totalObjectsText;
    private Button continueButton;

    private bool gameResolved = false;

    public void RegisterHitable(Hitable.HitableType hitableType, bool register)
    {
        int count;
        currentNumberOfHitables.TryGetValue(hitableType, out count);
        currentNumberOfHitables[hitableType] = (register ? count + 1 : count - 1);

        if (register)
        {
            int totalCount;
            totalNumberOfHitables.TryGetValue(hitableType, out totalCount);
            totalNumberOfHitables[hitableType] = totalCount + 1;
        }
    }

    public int GetCurrentHitablesCount(Hitable.HitableType hitableType)
    {
        int count;
        currentNumberOfHitables.TryGetValue(hitableType, out count);
        return count;
    }

    public int GetTotalHitablesCount(Hitable.HitableType hitableType)
    {
        int count;
        totalNumberOfHitables.TryGetValue(hitableType, out count);
        return count;
    }

    public bool PlayerWon()
    {
        int countTargets;
        currentNumberOfHitables.TryGetValue(Hitable.HitableType.Target, out countTargets);
        return (countTargets == 0);
    }

    public bool PlayerLost()
    {
        return playerHitable.health <= 0.0f;
    }

    public bool GameEnded()
    {
        return PlayerWon() || PlayerLost();
    }

    // Use this for initialization
    void Start() {
        playerHitable = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHitable>();

        diedCanvas = transform.Find("DiedCanvas").GetComponent<Canvas>();

        var scoresCanvasObj = transform.Find("ScoresCanvas");
        scoresCanvas = scoresCanvasObj.GetComponent<Canvas>();
        targetsText = scoresCanvasObj.Find("TargetsText").GetComponent<Text>();
        enemiesText = scoresCanvasObj.Find("EnemiesText").GetComponent<Text>();
        objectsText = scoresCanvasObj.Find("ObjectsText").GetComponent<Text>();
        totalTargetsText = scoresCanvasObj.Find("TotalTargetsText").GetComponent<Text>();
        totalEnemiesText = scoresCanvasObj.Find("TotalEnemiesText").GetComponent<Text>();
        totalObjectsText = scoresCanvasObj.Find("TotalObjectsText").GetComponent<Text>();
        continueButton = scoresCanvasObj.Find("ContinueButton").GetComponent<Button>();

        var hudCanvasObj = GameObject.FindObjectOfType<HudScript>().transform;
        targetsTextHud = hudCanvasObj.Find("TargetsText").GetComponent<Text>();
        totalTargetsTextHud = hudCanvasObj.Find("TotalTargetsText").GetComponent<Text>();

        diedCanvas.gameObject.SetActive(false);
        scoresCanvas.gameObject.SetActive(false);

        AmbientMusic.instance.play(AmbientMusic.instance.levelMusic);
    }

    // Update is called once per frame
    void Update() {
        var totalTargets = GetTotalHitablesCount(Hitable.HitableType.Target);
        targetsTextHud.text = (totalTargets - GetCurrentHitablesCount(Hitable.HitableType.Target)).ToString();
        totalTargetsTextHud.text = totalTargets.ToString();

        if (gameResolved)
            return;

        if (!GameEnded())
            return;

        gameResolved = true;

        if (PlayerWon())
        {
            AmbientMusic.instance.play(AmbientMusic.instance.wonMusic);

            var totalEnemies = GetTotalHitablesCount(Hitable.HitableType.Enemy);
            var totalObjects = GetTotalHitablesCount(Hitable.HitableType.Object);

            targetsText.text = (totalTargets - GetCurrentHitablesCount(Hitable.HitableType.Target)).ToString();
            enemiesText.text = (totalEnemies - GetCurrentHitablesCount(Hitable.HitableType.Enemy)).ToString();
            objectsText.text = (totalObjects - GetCurrentHitablesCount(Hitable.HitableType.Object)).ToString();

            totalTargetsText.text = totalTargets.ToString();
            totalEnemiesText.text = totalEnemies.ToString();
            totalObjectsText.text = totalObjects.ToString();

            scoresCanvas.gameObject.SetActive(true);
            continueButton.Select();
            Invoke("onNextLevel", 5.0f);
        }

        if (PlayerLost())
        {
            AmbientMusic.instance.play(AmbientMusic.instance.lostMusic);

            diedCanvas.gameObject.SetActive(true);
            Invoke("showMenu", 2.0f);
        }
    }

    void onNextLevel()
    {
        Debug.Log("Loading next level");
        if (SceneController.instance != null)
            SceneController.instance.NextLevel();
    }

    private void showMenu()
    {
        SceneController.ShowPauseMenu(false, false, true);
    }
}
