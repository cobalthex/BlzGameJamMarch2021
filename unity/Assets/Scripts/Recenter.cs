using UnityEngine;

public class Recenter : MonoBehaviour
{
    Vector3 originalPosition;

    void Awake()
    {
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, 0.25f); //linear time?
    }
}
