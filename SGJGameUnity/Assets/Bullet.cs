using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public bool isPlayerBullet = false;
    // Use this for initialization
    void Start () {
        Destroy(gameObject, Weapons.instance.bulletLifetime);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Bullet collision with: " + collision.gameObject.name);
        var contact = collision.contacts[0];

        Weapons.instance.projectileHitAction(Weapons.WeaponKind.Pistol, collision.gameObject, contact.point, contact.normal);

        Destroy(gameObject);
    }

}
