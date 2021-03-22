using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
    public float DelaySeconds = 3;

    void Start()
    {
        Destroy(gameObject, DelaySeconds);
    }
}
