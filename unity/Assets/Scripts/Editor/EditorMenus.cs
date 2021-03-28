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

    [MenuItem("Tools/Drop object _g")]
    public static void DropObject()
    {
        if (Selection.activeGameObject == null)
            return;

        var down = Physics.gravity.normalized;
        Vector3 position;

        Collider collider;
        Renderer renderer;
        if ((collider = Selection.activeGameObject.GetComponent<Collider>()) != null)
        {
            var projection = Vector3.Project(collider.bounds.extents, down);
            position = collider.bounds.center - projection;
        }
        else if ((renderer = Selection.activeGameObject.GetComponent<Renderer>()) != null)
        {
            var projection = Vector3.Project(renderer.bounds.extents, down);
            position = renderer.bounds.center - projection;
        }
        else
            position = Selection.activeTransform.position;

        // note: this will only detect objects with a collider
        if (Physics.Raycast(position, down, out var hit))
        {
            Selection.activeTransform.position -= (position - hit.point);
        }
        else
        {
            Debug.LogWarning("No collider to drop onto");
        }
    }
}
