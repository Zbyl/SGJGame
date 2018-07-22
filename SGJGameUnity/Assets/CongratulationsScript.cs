using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CongratulationsScript : MonoBehaviour {
    public float showMenuDelay = 5.0f;

	// Use this for initialization
	void Start () {
        Invoke("showMenu", showMenuDelay);
        AmbientMusic.instance.play(AmbientMusic.instance.wonMusic);
    }

    // Update is called once per frame
    void Update () {
		
	}

    void showMenu()
    {
        SceneController.ShowPauseMenu(false, false, true);
    }
}
