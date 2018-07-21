using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoldierController : SoldierController {
    new void Awake()
    {
        base.Awake();
    }

    // Use this for initialization
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update () {
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

        float dpadX = Input.GetAxis("DPadX");
        float dpadY = Input.GetAxis("DPadY");
        //Debug.Log(string.Format("dpadX={0} dpadY={1}", dpadX, dpadY));

        if (dpadX < -0.5)
            switchWeapon(Weapons.WeaponKind.Knife);
        if (dpadX > 0.5)
            switchWeapon(Weapons.WeaponKind.Shotgun);
        if (dpadY > 0.5)
            switchWeapon(Weapons.WeaponKind.Pistol);
        if (dpadY < -0.5)
            switchWeapon(Weapons.WeaponKind.Rifle);
    }

    // Physics in FixedUpdate
    void FixedUpdate()
    {
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
