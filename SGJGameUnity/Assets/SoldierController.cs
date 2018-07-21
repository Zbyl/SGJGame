using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierController : MonoBehaviour
{
    Animator feetAnimator;
    Animator bodyAnimator;

    protected new Rigidbody2D rigidbody;
    public float strafeMultiplier = 130f;
    public float moveMultiplier = 130f;
    public float strafeDisablesMoveTreshold = 0.05f;
    public float feetAnimSpeedMultiplier = 0.1f;
    public float bodyAnimSpeedMultiplier = 0.1f;
    public float lockStrafeTreshold = 0.1f;
    public float directionLerpFactor = 0.3f;
    public float maxSpeed = 50.0f;

    protected Vector3 targetDirection = Vector3.right;
    protected Vector3 currentDirection = Vector3.right;

    public Weapons.WeaponKind currentWeapon = Weapons.WeaponKind.Rifle;

    private float lastShot = 0.0f;
    private bool shotAnimFinished = true;
    private float lastReload = 0.0f;
    private bool tryReloading = false;
    public Transform[] weaponProjectileSources = { null, null, null, null };

    private string[] weaponIdleAnims = { "KnifeIdle", "PistolIdle", "RifleIdle", "ShotgunIdle" };
    private string[] weaponWalkAnims = { "KnifeWalk", "PistolWalk", "RifleWalk", "ShotgunWalk" };
    private string[] weaponShootAnims = { "KnifeShoot", "PistolShoot", "RifleShoot", "ShotgunShoot" };
    private string[] weaponReloadAnims = { null, "PistolReload", "RifleReload", "ShotgunReload" };

    public int[] weaponMagazineSizes = { 0, 5, 15, 1 };
    public int[] weaponMagazineFills = { 0, 5, 15, 1 };
    public int[] weaponBulletsOwned = { 0, 1000, 1000, 1000 };

    protected void Awake()
    {
    }

    // Use this for initialization
    protected void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        feetAnimator = transform.Find("Feet").GetComponent<Animator>();
        bodyAnimator = transform.Find("Body").GetComponent<Animator>();
        switchWeapon(currentWeapon);
    }

    // Physics in FixedUpdate
    protected void UpdateAnims()
    {
        if (tryReloading)
        {
            ReloadWeapon();
        }
    }

    // Physics in FixedUpdate
    protected void FixedLerpDirection()
    {
        currentDirection = Vector3.Slerp(currentDirection, targetDirection, directionLerpFactor);

        // Update sprite rotation
        var upDir = Vector3.Cross(Vector3.forward, currentDirection);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, upDir);
    }

    // Physics in FixedUpdate
    protected void FixedMove(Vector3 moveVector)
    {
        rigidbody.AddForce(moveVector);
        if (rigidbody.velocity.magnitude > maxSpeed)
        {
            rigidbody.velocity = rigidbody.velocity.normalized * maxSpeed;
        }

        FixedUpdateAnims(moveVector);
    }

    // Physics in FixedUpdate
    void FixedUpdateAnims(Vector3 moveVector)
    {
        var moveAlongDirection = Vector3.Dot(moveVector, currentDirection) * currentDirection;
        var moveSideways = moveVector - moveAlongDirection;
        var strafing = moveSideways.sqrMagnitude > moveAlongDirection.sqrMagnitude;

        feetAnimator.SetFloat("Speed", rigidbody.velocity.magnitude * feetAnimSpeedMultiplier);
        bodyAnimator.SetFloat("Speed", rigidbody.velocity.magnitude * bodyAnimSpeedMultiplier);

        feetAnimator.SetBool("Strafing", strafing);
        bodyAnimator.SetBool("Moving", (rigidbody.velocity.magnitude > 1f));
    }

    protected void switchWeapon(Weapons.WeaponKind weapon)
    {
        Debug.Log(string.Format("Switching to weapon {0}", weapon));
        currentWeapon = weapon;

        bodyAnimator.SetBool("Knife", weapon == Weapons.WeaponKind.Knife);
        bodyAnimator.SetBool("Pistol", weapon == Weapons.WeaponKind.Pistol);
        bodyAnimator.SetBool("Rifle", weapon == Weapons.WeaponKind.Rifle);
        bodyAnimator.SetBool("Shotgun", weapon == Weapons.WeaponKind.Shotgun);
        bodyAnimator.Play(weaponIdleAnims[(int)weapon]);
        shotAnimFinished = true;
    }

    bool reloadFinished()
    {
        var weaponReloadDelay = Weapons.instance.weaponReloadDelays[(int)currentWeapon];
        return (Time.time - lastReload >= weaponReloadDelay);
    }

    bool shotFinished()
    {
        var weaponDelay = Weapons.instance.weaponDelays[(int)currentWeapon];
        return shotAnimFinished;// && (Time.time - lastShot >= weaponDelay);
    }

    public void ReloadWeapon()
    {
        if (!shotFinished() || !reloadFinished())
        {
            return;
        }
        tryReloading = false;

        var magazineSize = weaponMagazineSizes[(int)currentWeapon];
        var bulletsLeft = weaponMagazineFills[(int)currentWeapon];
        if (bulletsLeft >= magazineSize)
        {
            return;
        }

        lastReload = Time.time;

        var reloadAnim = weaponReloadAnims[(int)currentWeapon];
        if (reloadAnim != null)
            bodyAnimator.Play(reloadAnim);

        var totalBulletsLeft = weaponBulletsOwned[(int)currentWeapon];
        if (totalBulletsLeft == 0)
        {
            Debug.Log("Reload failed.");
            Weapons.instance.playReloadFailedSound(currentWeapon, transform.position);
            return;
        }

        var bulletsToTransfer = magazineSize - bulletsLeft;
        if (bulletsToTransfer > totalBulletsLeft)
            bulletsToTransfer = totalBulletsLeft;

        //Debug.Log(string.Format("Reloaded: bulletsToTransfer={0}", bulletsToTransfer));
        weaponBulletsOwned[(int)currentWeapon] -= bulletsToTransfer;
        weaponMagazineFills[(int)currentWeapon] += bulletsToTransfer;

        Weapons.instance.playReloadSound(currentWeapon, transform.position);
    }

    public void ShootWeapon()
    {
        if (!shotFinished() || !reloadFinished())
        {
            return;
        }

        var magazineSize = weaponMagazineSizes[(int)currentWeapon];
        var bulletsLeft = weaponMagazineFills[(int)currentWeapon];
        if ((magazineSize > 0) && (bulletsLeft == 0))
        {
            ReloadWeapon();
            return;
        }

        //Debug.Log("Shooting.");
        lastShot = Time.time;

        if (magazineSize > 0)
        {
            weaponMagazineFills[(int)currentWeapon] -= 1;
            bulletsLeft = weaponMagazineFills[(int)currentWeapon];
            if (bulletsLeft == 0)
            {
                tryReloading = true;
            }
        }

        //Debug.Log("ShootStarted");
        shotAnimFinished = false;
        var shootAnim = weaponShootAnims[(int)currentWeapon];
        bodyAnimator.Play(shootAnim);
    }

    public void Shoot()
    {
        //Debug.Log("ShootCompleted");
        shotAnimFinished = true;
        var projectileSource = weaponProjectileSources[(int)currentWeapon];
        Weapons.instance.Shoot(currentWeapon, projectileSource.position, currentDirection);
    }

    public void ShootFinished()
    {
        //var idleAnim = weaponIdleAnims[(int)currentWeapon];
        //bodyAnimator.Play(idleAnim);
    }
}
