using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobinHoodController : MonoBehaviour {

    Animator feetAnimator;
    Animator bodyAnimator;

    private new Rigidbody2D rigidbody;
    public float strafeMultiplier = 1f;
    public float moveMultiplier = 1f;
    public float strafeDisablesMoveTreshold = 0.05f;
    public float feetAnimSpeedMultiplier = 0.1f;
    public float bodyAnimSpeedMultiplier = 0.1f;
    public float lockStrafeTreshold = 0.1f;
    public float directionLerpFactor = 0.5f;
    public float maxSpeed = 50.0f;

    private Vector3 targetDirection = Vector3.right;
    private Vector3 currentDirection = Vector3.right;

    public enum WeaponKind
    {
        Rifle,
        Shotgun,
        Pistol,
        Knife,
    }

    private float lastShot = 0.0f;
    public float[] weaponDelays = { 0.5f, 1.0f, 0.3f, 1.0f };
    public Transform[] weaponProjectileSources = { null, null, null, null };
    public GameObject[] weaponProjectiles = { null, null, null, null };

    public string[] weaponIdleAnims = { "RifleIdle", "ShotgunIdle", "PistolIdle", "KnifeIdle" };
    public string[] weaponWalkAnims = { "RifleWalk", "ShotgunWalk", "PistolWalk", "KnifeWalk" };
    public string[] weaponShootAnims = { "RifleShoot", "ShotgunShoot", "PistolShoot", "KnifeShoot" };
    public string[] weaponReloadAnims = { "RifleReload", "ShotgunReload", "PistolReloade", null };

    public float knifeRadius = 1.0f;
    public float knifeDamage = 50.0f;

    public float bulletSpeed = 50.0f;

    public static int destroyablesMask = 512;
    public static int enemiesMask = 1024;
    public static int playerMask = 2048;
    public static int hitablesMask = enemiesMask | destroyablesMask;

    public WeaponKind currentWeapon = WeaponKind.Rifle;

    private void Awake()
    {
    }

    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        feetAnimator = transform.Find("Feet").GetComponent<Animator>();
        bodyAnimator = transform.Find("Body").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetButton("Fire1"))
        {
            ShootWeapon();
        }
    }

    // Physics in FixedUpdate
    void FixedUpdate()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        float strafeX = Input.GetAxis("StrafeX");
        float strafeY = Input.GetAxis("StrafeY");
        float lockStrafeValue = Input.GetAxis("LockStrafe");
        bool lockStrafe = (lockStrafeValue > lockStrafeTreshold) || (lockStrafeValue < -lockStrafeTreshold);

        //Debug.Log(string.Format("lockStrafeValue={0} lockStrafe={1}", lockStrafeValue, lockStrafe));
        //Debug.Log(string.Format("moveX={0} moveY={1} strafeX={2} strafeY={3}", moveX, moveY, strafeX, strafeY));

        var moveDir = (Vector3.right * moveX + Vector3.up * moveY);
        if (!lockStrafe)
        {
            if (moveDir.magnitude > 0.01f)
            {
                targetDirection = moveDir.normalized;
            }

            currentDirection = Vector3.Slerp(currentDirection, targetDirection, directionLerpFactor);
        }

        // Update sprite rotation
        var upDir = Vector3.Cross(Vector3.forward, currentDirection);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, upDir);

        var move = Vector3.zero;

        // When strafing move control doesn't contribute to move, only to direction.
        var strafeDir = (Vector3.right * strafeX + Vector3.up * strafeY);

        if (strafeDir.magnitude > strafeDisablesMoveTreshold)
        {
            //move = strafeDir * strafeMultiplier;
        }
        else
        {
            //move = moveDir * moveMultiplier;
        }
        move = moveDir * moveMultiplier;
        var strafe = strafeDir * strafeMultiplier;

        var moveAlongDirection = Vector3.Dot(move, currentDirection) * currentDirection;
        var moveSideways = move - moveAlongDirection;
        var strafing = moveSideways.sqrMagnitude > moveAlongDirection.sqrMagnitude;
        //move += moveSideways;

        //Debug.Log(string.Format("move={0} moveAlongDirection={1} moveSideways={2}", move, moveAlongDirection, moveSideways));

        feetAnimator.SetFloat("Speed", rigidbody.velocity.magnitude * feetAnimSpeedMultiplier);
        bodyAnimator.SetFloat("Speed", rigidbody.velocity.magnitude * bodyAnimSpeedMultiplier);

        feetAnimator.SetBool("Strafing", strafing);
        bodyAnimator.SetBool("Moving", (rigidbody.velocity.magnitude > 1f));

        rigidbody.AddForce(move);
        if (rigidbody.velocity.magnitude > maxSpeed)
        {
            rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
        }
    }

    public void ShootWeapon()
    {
        var weaponDelay = weaponDelays[(int)currentWeapon];
        if (Time.time - lastShot < weaponDelay)
        {
            return;
        }
        lastShot = Time.time;

        var shootAnim = weaponShootAnims[(int)currentWeapon];
        bodyAnimator.Play(shootAnim);
    }

    public void Shoot()
    {
        var weaponProjectile = weaponProjectiles[(int)currentWeapon];
        var projectileSource = weaponProjectileSources[(int)currentWeapon];

        if (currentWeapon == WeaponKind.Knife)
        {
            // Hit all enemies in radius.
            var colliders = Physics2D.OverlapCircleAll(projectileSource.position, knifeRadius, hitablesMask);
            foreach (var collider in colliders)
            {
                var targetRadius = collider.bounds.size.x / 2;
                var dirToSource = projectileSource.position - collider.transform.position;
                var hitPosition = collider.transform.position + dirToSource.normalized * targetRadius;

                var hitTarget = collider.GetComponent<Hitable>();
                hitTarget.Hit(knifeDamage, hitPosition, dirToSource.normalized);

                // projectile weapon
                var rot = Quaternion.FromToRotation(projectileSource.right, currentDirection);
                GameObject projectileClone = Instantiate(weaponProjectile, hitPosition, rot);
            }
            return;
        }

        if (currentWeapon == WeaponKind.Shotgun)
        {
            // raycast weapon
        }
        else
        {
            // projectile weapon
            var rot = Quaternion.FromToRotation(projectileSource.right, currentDirection);
            Rigidbody2D projectileClone = Instantiate(weaponProjectile, projectileSource.position, rot).GetComponent<Rigidbody2D>();
            projectileClone.velocity = currentDirection * bulletSpeed;

            // You can also access other components / scripts of the clone
            //arrowClone.GetComponent<MyRocketScript>().DoSomething();
        }
    }
}
