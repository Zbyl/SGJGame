using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameButton : MonoBehaviour {

    private SceneController sceneController;

    private void Awake()
    {
        sceneController = FindObjectOfType<SceneController>();

        if (!sceneController)
            throw new UnityException("Scene Controller could not be found, ensure that it exists in the Persistent scene.");
    }

    public void Press()
    {
        sceneController.NewGame();
    }
}
