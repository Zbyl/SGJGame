using UnityEngine;
using System.Collections;

public class Weapons : MonoBehaviour
{
    public static Weapons instance;

    public enum WeaponKind
    {
        Knife,
        Pistol,
        Rifle,
        Shotgun,
    }

    public float knifeRadius = 1.0f;

    public float bulletSpeed = 50.0f;
    public float bulletLifetime = 2.0f;
    public GameObject projectileHitEffect;
    public AudioClip projectileHitSound;

    public float shotgunDistance = 10.0f;
    public int shotgunRayCount = 5;
    public int shotgunSpreadAngle = 35;
    public LineRenderer shotgunShotTrail;
    public float shotgunShotTrailDuration = 0.1f;

    public float[] weaponDelays = { 1.0f, 0.5f, 0.1f, 1.0f };
    public float[] weaponReloadDelays = { 0.0f, 0.0f, 0.0f, 0.0f };
    public float[] weaponDamages = { 50.0f, 10.0f, 10.0f, 10.0f };
    public GameObject[] weaponProjectiles = { null, null, null, null }; // Not used for knife, bullet hit for shotgun.
    public AudioClip[] weaponShootSounds = { null, null, null, null };
    public AudioClip reloadSound;
    public AudioClip reloadFailedSound;

    public static int destroyablesMask = 512;
    public static int enemiesMask = 1024;
    public static int playerMask = 2048;
    public static int hitablesMask = enemiesMask | destroyablesMask;
    public static int allHitablesMask = enemiesMask | destroyablesMask | playerMask;

    void Awake()
    {
        if (Weapons.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Weapons.instance = this;
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Shoot(GameObject shooter, WeaponKind weapon, Vector3 source, Vector3 direction)
    {
        var weaponShootSound = weaponShootSounds[(int)weapon];
        if (weaponShootSound != null)
            AudioSource.PlayClipAtPoint(weaponShootSound, source);

        switch (weapon)
        {
            case WeaponKind.Knife: ShootKnife(shooter, weapon, source, direction); break;
            case WeaponKind.Pistol: ShootRifle(weapon, source, direction); break;
            case WeaponKind.Rifle: ShootRifle(weapon, source, direction); break;
            case WeaponKind.Shotgun: ShootShotgun(weapon, source, direction); break;
        }
    }

    public void ShootKnife(GameObject shooter, WeaponKind weapon, Vector3 source, Vector3 direction)
    {
        Debug.Log("Knife shot.");
        // melee weapon - hit all enemies in radius.
        var weaponDamage = weaponDamages[(int)weapon];

        var colliders = Physics2D.OverlapCircleAll(source, knifeRadius); //, hitablesMask);
        foreach (var collider in colliders)
        {
            var targetRadius = collider.bounds.size.x / 2;
            var dirToSource = source - collider.transform.position;
            var hitPosition = collider.transform.position + dirToSource.normalized * targetRadius;

            var hitTarget = collider.GetComponent<Hitable>();
            if (hitTarget == null)
            {
                Debug.LogWarning("Knife hit non-hittable: " + collider.gameObject.name);
                continue;
            }

            if (hitTarget.gameObject == shooter)
                continue;
            projectileHitAction(weapon, collider.gameObject, hitPosition, -dirToSource.normalized);
        }
    }

    public void ShootRifle(WeaponKind weapon, Vector3 source, Vector3 direction)
    {
        // projectile weapon - shoot a projectile
        var weaponProjectile = weaponProjectiles[(int)weapon];
        var rot = Quaternion.FromToRotation(Vector3.right, direction);
        Rigidbody2D projectileClone = Instantiate(weaponProjectile, source, rot).GetComponent<Rigidbody2D>();
        projectileClone.velocity = direction * bulletSpeed;

        // You can also access other components / scripts of the clone
        //projectileClone.GetComponent<Bullet>().isPlayerBullet = true;
    }

    public void ShootShotgun(WeaponKind weapon, Vector3 source, Vector3 direction)
    {
        // raycast weapon
        var rayCount = shotgunRayCount;
        for (var i = 0; i < rayCount; ++i)
        {
            var angleDiff = Random.Range(-shotgunSpreadAngle, shotgunSpreadAngle);
            var rayDir = Quaternion.Euler(0, 0, angleDiff) * direction;
            var hit = Physics2D.Raycast(source, rayDir, shotgunDistance);

            var hitPoint = source + rayDir * shotgunDistance;
            if (hit.collider != null)
            {
                hitPoint = hit.point;
                projectileHitAction(weapon, hit.collider.gameObject, hitPoint, -direction);
            }

            LineRenderer rayTrail = Instantiate(shotgunShotTrail, transform);
            var pointsCount = 30;
            var points = new Vector3[pointsCount];
            for (var p = 0; p < pointsCount; ++p)
            {
                var q = 1.0f * p / (pointsCount - 1);
                var point = source * q + hitPoint * (1 - q);
                points[p] = point;
            }
            rayTrail.positionCount = points.Length;
            rayTrail.SetPositions(points);
            //rayTrail.SetPositions(new Vector3[] { source, hitPoint });
            Destroy(rayTrail.gameObject, shotgunShotTrailDuration);
        }
    }

    public void projectileHitAction(WeaponKind weapon, GameObject target, Vector3 position, Vector3 normal)
    {
        var hitTarget = target.GetComponent<Hitable>();
        var hitResult = new Hitable.HitResult(false, false);
        if (hitTarget != null)
        {
            var weaponDamage = weaponDamages[(int)weapon];
            hitResult = hitTarget.Hit(weaponDamage, position, normal);
        }

        if (!hitResult.soundPlayed)
        {
            if (projectileHitSound != null)
                AudioSource.PlayClipAtPoint(projectileHitSound, position);
        }

        if (!hitResult.effectShown)
        {
            var rot = Quaternion.FromToRotation(Vector3.right, normal);
            GameObject projectileClone = Instantiate(projectileHitEffect, position, rot);
        }
    }

    public static bool playRandomSound(AudioClip[] sounds, Vector3 position)
    {
        if (sounds.Length == 0)
            return false;

        var soundsIdx = Random.Range(0, sounds.Length);
        var sound = sounds[soundsIdx];
        if (sound == null)
            return false;

        AudioSource.PlayClipAtPoint(sound, position);
        return true;
    }

    public static bool createRandomObject(GameObject[] objects, Vector3 position, Vector3 direction)
    {
        if (objects.Length == 0)
            return false;

        var objIdx = Random.Range(0, objects.Length);
        var obj = objects[objIdx];
        if (obj == null)
            return false;

        var rot = Quaternion.FromToRotation(Vector3.right, direction);
        GameObject injuryClone = Instantiate(obj, position, rot);
        return true;
    }

    public static Vector3 randomDirection()
    {
        var angleDiff = Random.Range(-180f, 180f);
        var rayDir = Quaternion.Euler(0, 0, angleDiff) * Vector3.right;
        return rayDir;
    }

    public void playReloadSound(WeaponKind currentWeapon, Vector3 position)
    {
        if (reloadSound != null)
            AudioSource.PlayClipAtPoint(reloadSound, position);
    }

    public void playReloadFailedSound(WeaponKind currentWeapon, Vector3 position)
    {
        if (reloadFailedSound != null)
            AudioSource.PlayClipAtPoint(reloadFailedSound, position);
    }

}
