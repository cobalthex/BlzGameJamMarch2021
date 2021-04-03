using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawn : MonoBehaviour
{
    void Start()
    {
        GetComponent<MeshRenderer>().enabled = false; // invisible in the world, but visible in the editor
    }
}
