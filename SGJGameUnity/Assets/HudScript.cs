using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudScript : MonoBehaviour {

    private Text healthText;
    private Text ammoClipText;
    private Text ammoTotalText;

    PlayerSoldierController playerController;
    Hitable playerHitable;

    // Use this for initialization
    void Start () {
        var player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerSoldierController>();
        playerHitable = player.GetComponent<Hitable>();
        healthText = transform.Find("HealthText").GetComponent<Text>();
        ammoClipText = transform.Find("AmmoClip").GetComponent<Text>();
        ammoTotalText = transform.Find("AmmoTotal").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update () {
        //public int[] weaponMagazineFills = { 0, 5, 15, 1 };
        //public int[] weaponBulletsOwned = { 1000, 1000, 1000, 1000 };

        var health = playerHitable.health;
        if (health < 0)
            health = 0;
        health = Mathf.Ceil(health);
        healthText.text = health.ToString();

        var weapon = playerController.currentWeapon;
        ammoClipText.text = playerController.weaponMagazineFills[(int)weapon].ToString();
        ammoTotalText.text = playerController.weaponBulletsOwned[(int)weapon].ToString();
    }
}
