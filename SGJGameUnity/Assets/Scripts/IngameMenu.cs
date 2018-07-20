using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameMenu : MonoBehaviour {

    public static bool GameIsPaused = false;

    public GameObject pauseMenu;

    // Use this for initialization
    void Awake () {
        pauseMenu.SetActive(GameIsPaused);
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetButtonDown("Menu"))
        {
            Debug.Log("Menu pressed");
            GameIsPaused = !GameIsPaused;
        }

        if (GameIsPaused)
        {
            pauseMenu.SetActive(true);
            //Time.timeScale = 0.0f;
        }
        else
        {
            pauseMenu.SetActive(false);
            //Time.timeScale = 1.0f;
        }
    }
}
