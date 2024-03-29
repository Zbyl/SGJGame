﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitable : MonoBehaviour {

    public enum HitableType
    {
        Player,
        Enemy,
        Object,
        Target,
    }

    public HitableType hitableType;

    public float health = 100.0f;

    public GameObject[] lootDrops = { };
    public GameObject[] deadBodies = { };

    public GameObject[] injuryEffects = { };
    public AudioClip[] injurySounds = { };

    public GameObject[] dyingEffects = { };
    public AudioClip[] dyingSounds = { };

    public GameObject[] deadEffects = { };
    public AudioClip[] deadSounds = { };

    public float destroyDelay = 0.0f;

    public ScoresScript scores;


    public struct HitResult
    {
        public HitResult(bool effectShown, bool soundPlayed)
        {
            this.effectShown = effectShown;
            this.soundPlayed = soundPlayed;
        }

        public bool effectShown;
        public bool soundPlayed;
    }


    // Use this for initialization
    void Awake () {
        scores = GameObject.FindGameObjectWithTag("Scores").GetComponent<ScoresScript>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnEnable()
    {
        scores.RegisterHitable(hitableType, true);
    }

    void OnDisable()
    {
        scores.RegisterHitable(hitableType, false);
    }

    public HitResult Hit(float damage, Vector3 contactPoint, Vector3 contactNormal)
    {
        if (health <= 0)
        {
            var effectShown = Weapons.createRandomObject(deadEffects, contactPoint, contactNormal);
            var soundPlayed = Weapons.playRandomSound(deadSounds, contactPoint);

            return new HitResult(effectShown, soundPlayed);
        }

        //Debug.Log(string.Format("damage={0}", damage));

        health -= damage;
        if (health > 0)
        {
            var effectShown = Weapons.createRandomObject(injuryEffects, contactPoint, contactNormal);
            var soundPlayed = Weapons.playRandomSound(injurySounds, contactPoint);

            return new HitResult(effectShown, soundPlayed);
        }
        else
        {
            var effectShown = Weapons.createRandomObject(dyingEffects, transform.position, Weapons.randomDirection());
            var soundPlayed = Weapons.playRandomSound(dyingSounds, transform.position);

            onDead();

            return new HitResult(effectShown, soundPlayed);
        }
    }

    protected virtual void onDead()
    {
        Destroy(gameObject, destroyDelay);
        Weapons.createRandomObject(lootDrops, transform.position, Vector3.right);
        Weapons.createRandomObject(deadBodies, transform.position, Vector3.right);
    }
}
