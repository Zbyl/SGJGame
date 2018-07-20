using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float bulletLifeTime = 2.0f;
    public float bulletDamage = 10.0f;
    public GameObject bulletHit;

    AudioSource audioSource;

    // Use this for initialization
    void Start () {
        audioSource = GetComponent<AudioSource>();
        Destroy(gameObject, bulletLifeTime);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Bullet collision with: " + collision.gameObject.name);
        foreach (var aContact in collision.contacts)
        {
            Debug.DrawRay(aContact.point, aContact.normal, Color.white);
        }
        //if (collision.relativeVelocity.magnitude > 2)
        audioSource.Play();

        var contact = collision.contacts[0];

        var hitTarget = collision.gameObject.GetComponent<Hitable>();
        if (hitTarget != null)
        {
            hitTarget.Hit(bulletDamage, contact.point, contact.normal);
        }

        // projectile weapon
        var rot = Quaternion.FromToRotation(bulletHit.transform.right, contact.normal);
        GameObject projectileClone = Instantiate(bulletHit, contact.point, rot);

        Destroy(gameObject);
    }
}
