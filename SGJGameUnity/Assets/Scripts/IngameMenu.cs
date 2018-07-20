using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameMenu : MonoBehaviour {

    public GameObject pauseMenu;
    public GameObject saveMenu;
    public GameObject loadMenu;

    private SceneController sceneController;

    private void Awake()
    {
        sceneController = FindObjectOfType<SceneController>();

        if (!sceneController)
            throw new UnityException("Scene Controller could not be found, ensure that it exists in the Persistent scene.");

        pauseMenu.SetActive(SceneController.GameIsPaused);
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetButtonDown("Menu"))
        {
            Debug.Log("Menu pressed");
            SceneController.GameIsPaused = !SceneController.GameIsPaused;
        }

        if (SceneController.GameIsPaused)
        {
            pauseMenu.SetActive(true);
            //Time.timeScale = 0.0f;
        }
        else
        {
            pauseMenu.SetActive(false);
            //Time.timeScale = 1.0f;
        }

        if (SceneController.ShowSaveLoadMenus)
        {
            saveMenu.SetActive(true);
            loadMenu.SetActive(true);
        }
        else
        {
            saveMenu.SetActive(false);
            loadMenu.SetActive(false);
        }
    }
}
