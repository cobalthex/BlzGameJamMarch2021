using UnityEngine;

public class Shake : MonoBehaviour
{
    public float DurationSec = 0.5f;
    public float Strength = 0.3f; //lerps to zero

    public Transform Transform; // optional, uses local transform if unset

    float endTime;

    void Start()
    {
        if (Transform == null)
            Transform = transform;
    }

    void OnEnable()
    {
        endTime = Time.time + DurationSec;
    }

    // Update is called once per frame
    void Update()
    {
        var timeLeft = endTime - Time.time;
        if (timeLeft > 0)
        {
            var strength = (timeLeft / DurationSec) * Strength;

            transform.position += new Vector3(
                Random.Range(-strength, strength),
                Random.Range(-strength, strength)
                // no z ?
            );
        }
        else
            enabled = false;
    }
}
