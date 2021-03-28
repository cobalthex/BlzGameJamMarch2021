using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Path : MonoBehaviour
{
    // Start is called before the first frame update
    Transform[] nodes;

    static Vector3 startColor, endColor;

    static Path()
    {
        Color.RGBToHSV(Color.red, out startColor.x, out startColor.y, out startColor.z);
        Color.RGBToHSV(Color.green, out endColor.x, out endColor.y, out endColor.z);
    }

    void Awake()
    {
        nodes = GetComponentsInChildren<Transform>();
    }

    void Update()
    {
        nodes = GetComponentsInChildren<Transform>();
    }

    void OnDrawGizmos()
    {
        // "this" is the first node, so there should always be at least one node

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(nodes[0].position, 0.1f);
        for (int i = 1; i < nodes.Length; ++i)
        {
            var color = Vector3.Lerp(startColor, endColor, (float)i / nodes.Length);
            Gizmos.color = Color.HSVToRGB(color.x, color.y, color.z);
            Gizmos.DrawWireSphere(nodes[i].position, 0.1f);
            Gizmos.DrawLine(nodes[i - 1].position, nodes[i].position);
        }
    }
}
