using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudScript : MonoBehaviour {

    private Text healthText;
    private Text ammoClipText;
    private Text ammoTotalText;

    private Image knifeImage;
    private Image pistolImage;
    private Image rifleImage;
    private Image shotgunImage;

    public Color gunOn;
    public Color gunOff;

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

        knifeImage = transform.Find("Knife").GetComponent<Image>();
        pistolImage = transform.Find("Pistol").GetComponent<Image>();
        rifleImage = transform.Find("Rifle").GetComponent<Image>();
        shotgunImage = transform.Find("Shotgun").GetComponent<Image>();
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

        knifeImage.color = gunOff;
        pistolImage.color = gunOff;
        rifleImage.color = gunOff;
        shotgunImage.color = gunOff;

        switch (weapon)
        {
            case Weapons.WeaponKind.Knife: knifeImage.color = gunOn; break;
            case Weapons.WeaponKind.Pistol: pistolImage.color = gunOn; break;
            case Weapons.WeaponKind.Rifle: rifleImage.color = gunOn; break;
            case Weapons.WeaponKind.Shotgun: shotgunImage.color = gunOn; break;
        }
    }
}
