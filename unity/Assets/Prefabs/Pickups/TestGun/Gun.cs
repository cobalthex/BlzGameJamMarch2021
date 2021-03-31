using UnityEngine;

public class Gun : Equippable
{
    public Transform Muzzle;

    public Rigidbody Projectile;
    public float ShotDelaySec = 0.1f;
    public float ShotSpeed = 10;

    float nextShotTimeSec;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Use(PlayerController player)
    {
        if (Time.time >= nextShotTimeSec)
        {
            nextShotTimeSec = Time.time + ShotDelaySec;

            var shot = Instantiate(Projectile, Muzzle.position + Muzzle.forward * 0.1f, Muzzle.rotation);
            shot.velocity = shot.transform.forward * ShotSpeed;
        }
    }

}
