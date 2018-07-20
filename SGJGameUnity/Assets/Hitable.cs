using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitable : MonoBehaviour {

    public float health = 100.0f;
    public GameObject[] injuries = {};
    public GameObject[] explosions = {};
    public float destroyDelay = 0.5f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Hit(float damage, Vector3 contactPoint, Vector3 contactNormal)
    {
        if (health <= 0)
        {
            return;
        }

        //Debug.Log(string.Format("damage={0}", damage));

        if (injuries.Length > 0)
        {
            var injuryIdx = Random.Range(0, injuries.Length);
            var injury = injuries[injuryIdx];

            var rot = Quaternion.FromToRotation(Vector3.right, contactNormal);
            GameObject injuryClone = Instantiate(injury, contactPoint, rot);
        }

        health -= damage;
        if (health > 0)
        {
            return;
        }

        if (explosions.Length > 0)
        {
            Debug.Log("Instantiating explosion.");
            var explosionIdx = Random.Range(0, explosions.Length);
            var explosion = explosions[explosionIdx];

            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(gameObject, destroyDelay);
        }
    }
}
