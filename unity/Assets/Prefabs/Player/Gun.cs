using UnityEngine;

public class Gun : MonoBehaviour
{
    public Rigidbody Projectile;
    public float ShotDelaySec = 0.1f;
    public float ShotSpeed = 10;

    float nextShotTimeSec;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Fire(Transform from)
    {
        if (Time.time >= nextShotTimeSec)
        {
            nextShotTimeSec = Time.time + ShotDelaySec;

            var shot = Instantiate(Projectile, from.position + from.forward * 0.1f, Quaternion.identity);
            shot.velocity = transform.forward * ShotSpeed;
        }
    }

}
