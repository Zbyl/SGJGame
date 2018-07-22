using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoLevel : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //SceneController.ShowPauseMenu(true, false, true);
        AmbientMusic.instance.play(AmbientMusic.instance.menuMusic);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
