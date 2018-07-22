using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientMusic : MonoBehaviour {
    public static AmbientMusic instance;
    private AudioClip currentClip;
    public AudioClip menuMusic;
    public AudioClip levelMusic;
    public AudioClip wonMusic;
    public AudioClip lostMusic;

    void Awake()
    {
        if (AmbientMusic.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        AmbientMusic.instance = this;
    }

    // Use this for initialization
    void Start () {
        currentClip = GetComponent<AudioSource>().clip;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void play(AudioClip clip)
    {
        var audioSource = GetComponent<AudioSource>();
        if (audioSource.clip == clip)
        {
            return;
        }
        audioSource.clip = clip;
        audioSource.Play();
    }
}
