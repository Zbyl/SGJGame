using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    public AudioClip pickUpSound;
    public float healthAdd = 0.0f;
    public int pistolBulletsAdd = 0;
    public int rifleBulletsAdd = 0;
    public int shellsAdd = 0;

    GameObject player;
    PlayerSoldierController playerController;
    Hitable playerHitable;

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerSoldierController>();
        playerHitable = player.GetComponent<Hitable>();
    }

    // Update is called once per frame
    void Update () {
		
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (var contact in collision.contacts)
        {
            if (contact.collider.gameObject != player)
                continue;

            playerHitable.health += healthAdd;
            if (playerHitable.health > 150)
            {
                playerHitable.health = 150;
            }

            playerController.weaponBulletsOwned[(int)Weapons.WeaponKind.Pistol] += pistolBulletsAdd;
            playerController.weaponBulletsOwned[(int)Weapons.WeaponKind.Rifle] += rifleBulletsAdd;
            playerController.weaponBulletsOwned[(int)Weapons.WeaponKind.Shotgun] += shellsAdd;

            if (pickUpSound != null)
                AudioSource.PlayClipAtPoint(pickUpSound, transform.position);

            Destroy(gameObject);
            return;
        }
    }
}
