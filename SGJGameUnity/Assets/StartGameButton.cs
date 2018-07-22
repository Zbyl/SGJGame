using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameButton : MonoBehaviour {

    private void Awake()
    {
    }

    public void Press()
    {
        SceneController.instance.NewGame();
    }
}
