using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoldierController : SoldierController {
    private float lastWeaponChange = 0;
    public float weaponChangeDelay = 0.2f;

    new void Awake()
    {
        base.Awake();
    }

    // Use this for initialization
    new void Start()
    {
        base.Start();
    }

    private void OnEnable()
    {
        FollowCamera.instance.target = transform;
    }

    private void OnDisable()
    {
        FollowCamera.instance.target = null;
    }

    // Update is called once per frame
    void Update () {
        if (SceneController.GameIsPaused)
            return;

        UpdateAnims();

        float lockStrafeValue = Input.GetAxis("LockStrafe");
        bool fire = (lockStrafeValue > lockStrafeTreshold) || (lockStrafeValue < -lockStrafeTreshold);

        if (Input.GetButton("Fire") || fire)
        {
            ShootWeapon();
        }

        if (Input.GetButton("Reload"))
        {
            ReloadWeapon();
        }

        if (Input.GetButton("Cheat"))
        {
            GetComponent<PlayerHitable>().health += 1;
            for (var i = 1; i <= 3; ++i)
            {
                weaponBulletsOwned[i] += 1;
            }
        }

        if (Input.GetButton("WeaponKnife"))
            switchWeapon(Weapons.WeaponKind.Knife);
        if (Input.GetButton("WeaponPistol"))
            switchWeapon(Weapons.WeaponKind.Pistol);
        if (Input.GetButton("WeaponRifle"))
            switchWeapon(Weapons.WeaponKind.Rifle);
        if (Input.GetButton("WeaponShotgun"))
            switchWeapon(Weapons.WeaponKind.Shotgun);

        float dpadX = Input.GetAxis("DPadX");
        float dpadY = Input.GetAxis("DPadY");

        if (Time.time - lastWeaponChange > weaponChangeDelay)
        {
            if (dpadY > 0.5)
            {
                lastWeaponChange = Time.time;
                var weapon = ((int)currentWeapon + 3) % 4;
                switchWeapon((Weapons.WeaponKind)weapon);
            }

            if (dpadY < -0.5)
            {
                lastWeaponChange = Time.time;
                var weapon = ((int)currentWeapon + 1) % 4;
                switchWeapon((Weapons.WeaponKind)weapon);
            }
        }
    }

    // Physics in FixedUpdate
    void FixedUpdate()
    {
        if (SceneController.GameIsPaused)
            return;

        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        float strafeX = Input.GetAxis("StrafeX");
        float strafeY = Input.GetAxis("StrafeY");

        //Debug.Log(string.Format("lockStrafeValue={0} lockStrafe={1}", lockStrafeValue, lockStrafe));
        //Debug.Log(string.Format("moveX={0} moveY={1} strafeX={2} strafeY={3}", moveX, moveY, strafeX, strafeY));

        var moveDir = (Vector3.right * moveX + Vector3.up * moveY);
        if (moveDir.magnitude > 0.01f)
        {
            targetDirection = moveDir.normalized;
        }

        // When strafing move control doesn't contribute to move, only to direction.
        var strafeDir = (Vector3.right * strafeX + Vector3.up * strafeY);

        if (strafeDir.magnitude > 0.1f)
        {
            targetDirection = strafeDir.normalized;
            //currentDirection = targetDirection;
        }

        FixedLerpDirection();

        var move = moveDir * moveMultiplier;
        FixedMove(move);
    }
}
