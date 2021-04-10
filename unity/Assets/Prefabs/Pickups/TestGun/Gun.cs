using UnityEngine;

public class Gun : Equippable
{
    public Transform Muzzle;
    public Rigidbody Projectile;
    public int StartingAmmo = 0;

    public float ShotDelaySec = 0.1f;
    public float ShotSpeed = 10;

    float nextShotTimeSec;

    int ammo = 0;

    private void Start()
    {
        ammo = StartingAmmo;
    }

    public override void Use(PlayerController player)
    {
        if (Time.time >= nextShotTimeSec &&
            (StartingAmmo == 0 || ammo > 0))
        {
            nextShotTimeSec = Time.time + ShotDelaySec;

            var shot = Instantiate(Projectile, Muzzle.position + Muzzle.forward * 0.1f, Muzzle.rotation);
            shot.velocity = shot.transform.forward * ShotSpeed;
            --ammo;
        }
    }

}
