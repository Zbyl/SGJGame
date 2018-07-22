using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IngameMenu : MonoBehaviour {

    public GameObject pauseMenu;

    public GameObject newMenu;
    public GameObject saveMenu;
    public GameObject loadMenu;
    public GameObject endMenu;

    public EventSystem eventSystem;

    private void Awake()
    {
        pauseMenu.SetActive(SceneController.GameIsPaused);
    }

    void selectButton(GameObject button)
    {
        eventSystem.SetSelectedGameObject(button);

        var buttonUi = button.GetComponent<Button>();
        buttonUi.OnSelect(new BaseEventData(eventSystem));
    }

    // Update is called once per frame
    void Update () {
        if (SceneController.SelectInGameMenu)
        {
            SceneController.SelectInGameMenu = false;
            if (SceneController.ShowNewMenu)
                selectButton(newMenu);
            else
            if (SceneController.ShowEndMenu)
                selectButton(endMenu);
            else
                selectButton(saveMenu);
        }

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

        newMenu.SetActive(SceneController.ShowNewMenu);
        saveMenu.SetActive(SceneController.ShowSaveLoadMenus);
        loadMenu.SetActive(SceneController.ShowSaveLoadMenus);
        endMenu.SetActive(SceneController.ShowEndMenu);
    }
}
