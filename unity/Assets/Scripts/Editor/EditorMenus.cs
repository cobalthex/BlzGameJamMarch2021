using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorMenus
{
    // Start is called before the first frame update

    [MenuItem("Tools/Find Far Objects")]
    public static void FindFarObjects()
    {
        var allObjs = Object.FindObjectsOfType<GameObject>();
        foreach (var obj in allObjs)
        {
            if ((Mathf.Abs(obj.transform.position.x) > 10000) ||
                (Mathf.Abs(obj.transform.position.y) > 5000) ||
                (Mathf.Abs(obj.transform.position.z) > 10000))
                Debug.LogWarning($"Found object {obj} at location {obj.transform.position}");
        }
    }

    [MenuItem("Tools/Reset Camera to Player")]
    public static void ResetCameraToPlayer()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var camera = player.GetComponentInChildren<Camera>();

        camera.enabled = false;
        camera.enabled = true;
    }

    static void DropObject(GameObject obj, Vector3 down)
    {
        Vector3 position;

        Collider collider;
        Renderer renderer;
        if ((collider = obj.GetComponent<Collider>()) != null)
        {
            var projection = Vector3.Project(collider.bounds.extents, down);
            position = collider.bounds.center - projection;
        }
        else if ((renderer = obj.GetComponent<Renderer>()) != null)
        {
            var projection = Vector3.Project(renderer.bounds.extents, down);
            position = renderer.bounds.center - projection;
        }
        else
            position = obj.transform.position;

        // note: this will only detect objects with a collider
        if (Physics.Raycast(position, down, out var hit))
        {
            obj.transform.position -= (position - hit.point);
        }
        else
        {
            Debug.LogWarning("No collider to drop onto");
        }
    }

    [MenuItem("Tools/Teleport Object/Drop _g")]
    public static void DropObject()
    {
        if (Selection.activeGameObject == null)
            return;

        var down = Physics.gravity.normalized;
        DropObject(Selection.activeGameObject, down);
    }

    [MenuItem("Tools/Teleport Object/+X")]
    public static void DropObjectPX()
    {
        if (Selection.activeGameObject == null)
            return;

        var up = Vector3.right;
        DropObject(Selection.activeGameObject, up);
    }

    [MenuItem("Tools/Teleport Object/-X")]
    public static void DropObjectNX()
    {
        if (Selection.activeGameObject == null)
            return;

        var up = Vector3.left;
        DropObject(Selection.activeGameObject, up);
    }

    [MenuItem("Tools/Teleport Object/+Y")]
    public static void DropObjectPY()
    {
        if (Selection.activeGameObject == null)
            return;

        var up = Vector3.up;
        DropObject(Selection.activeGameObject, up);
    }

    [MenuItem("Tools/Teleport Object/-Y")]
    public static void DropObjectNY()
    {
        if (Selection.activeGameObject == null)
            return;

        var up = Vector3.down;
        DropObject(Selection.activeGameObject, up);
    }

    [MenuItem("Tools/Teleport Object/+Z")]
    public static void DropObjectPZ()
    {
        if (Selection.activeGameObject == null)
            return;

        var up = Vector3.forward;
        DropObject(Selection.activeGameObject, up);
    }

    [MenuItem("Tools/Teleport Object/-Z")]
    public static void DropObjectNZ()
    {
        if (Selection.activeGameObject == null)
            return;

        var up = Vector3.back;
        DropObject(Selection.activeGameObject, up);
    }
}
