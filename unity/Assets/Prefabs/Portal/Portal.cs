// based off https://medium.com/@limdingwen_66715/multiple-recursive-portals-and-ai-in-unity-part-1-basic-portal-rendering-7c3d957f656c

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Portal : MonoBehaviour
{
    public enum EnteringBackBehavior
    {
        Passthru,
        Block,
        Teleport,
    }

    //public Transform Exit;
    public Portal LinkedPortal;
    public int MaxUseCount = 1;

    public EnteringBackBehavior EnteringFromBackBehavior;

    int useCount = 0;

    Camera portalCamera;
    Renderer viewthrough;
    RenderTexture viewthroughRt;

    Camera viewCamera;

    Transform back;
    Transform front;
    Vector4 portalPlane;

    public Vector3 Forward => front.forward;
    public Vector3 Up => front.up;

    //public Vector3 Forward => transform.up;
    //public Vector3 Up => -transform.right;

    public bool IsMirror => this == LinkedPortal;
    //public bool IsMirror => transform == Exit;

    void Start()
    {
        portalCamera = GetComponentInChildren<Camera>();
        viewthrough = GetComponent<Renderer>();
        back = transform.Find("back");
        front = transform.Find("front");

        viewthroughRt = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.DefaultHDR);
        // viewthroughRt.Create(); // created on-demand

        viewthrough.material.mainTexture = viewthroughRt;
        portalCamera.targetTexture = viewthroughRt;

        if (IsMirror)
        {
            // todo: make this work
            viewthrough.material.SetTextureScale("_MainTex", new Vector2(-1, 1));
        }

        // todo: if portal is self-portaling (mirror), flip texture on horizontal access

        viewCamera = Camera.main;

        var plane = new Plane(front.forward, transform.position);
        portalPlane = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);
    }

    void LateUpdate()
    {
        if (LinkedPortal == null)
            return; // clear target?

        var lookPosition = GetTargetRelativePosition(viewCamera.transform.position);
        var lookRotation = GetTargetRelativeRotation(viewCamera.transform.rotation);

        portalCamera.transform.SetPositionAndRotation(lookPosition, lookRotation);

        //var portalPlane = new Plane(Exit)
        var clipMatrix = Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * LinkedPortal.portalPlane;

        // Set portal camera projection matrix to clip walls between target portal and portal camera
        // Inherits main camera near/far clip plane and FOV settings

        var obliqueProjectionMatrix = viewCamera.CalculateObliqueMatrix(clipMatrix);
        portalCamera.projectionMatrix = obliqueProjectionMatrix;
    }

    void OnDestroy()
    {
        viewthroughRt.Release();
        Destroy(viewthroughRt);
    }

    void Teleport(Rigidbody body)
    {
        var relVelocity = front.InverseTransformDirection(body.velocity);
        body.velocity = LinkedPortal.back.TransformDirection(relVelocity);

        var pos = GetTargetRelativePosition(body.transform.position);
        var rot = GetTargetRelativeRotation(body.transform.rotation);
        body.transform.SetPositionAndRotation(pos, rot);

        ++useCount;
    }

    HashSet<Collider> ignored = new HashSet<Collider>();

    void OnTriggerEnter(Collider collider)
    {
        var velocity = collider.attachedRigidbody.velocity;
        var entranceDirection = Vector3.Dot(velocity, front.forward);

        var distToPortal = Vector3.Dot(collider.transform.position - transform.position, front.forward); // prevent just-teleported items from being caught

        // did the collider enter the back side?
        if (distToPortal < 0 && entranceDirection > 0)
        {
            switch (EnteringFromBackBehavior)
            {
                case EnteringBackBehavior.Passthru:
                case EnteringBackBehavior.Block:
                    ignored.Add(collider);
                    break;

                case EnteringBackBehavior.Teleport:
                    break;
            }
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (LinkedPortal == null || (MaxUseCount > 0 && useCount > MaxUseCount))
            return;

        if (ignored.Contains(collider))
        {
            if (EnteringFromBackBehavior == EnteringBackBehavior.Block)
            {
                const float pushbackSpeed = 1;

                var entranceDirection = Vector3.Dot(collider.attachedRigidbody.velocity, front.forward);

                collider.attachedRigidbody.velocity += ((entranceDirection + pushbackSpeed) * back.forward);
                collider.attachedRigidbody.angularVelocity = Vector3.zero; // ?
            }

            return;
        }

        // wait until the center crosses the threshold
        var distToPortal = Vector3.Dot(collider.transform.position - transform.position, front.forward);
        if (distToPortal > 0)
            return;

        Teleport(collider.attachedRigidbody);
    }

    void OnTriggerExit(Collider collider)
    {
        ignored.Remove(collider);
    }

    Vector3 GetTargetRelativePosition(Vector3 position)
    {
        // todo: this needs to work with mirrors

        return LinkedPortal.back.TransformPoint(front.InverseTransformPoint(position));

        // sort of works (todo: would be nice to get working)
        //var dist = Vector3.Distance(position, transform.position);
        //var reject = position - Vector3.Project(position, Forward);
        //return (LinkedPortal.transform.position - (LinkedPortal.Forward * dist) + reject);
    }

    Quaternion GetTargetRelativeRotation(Quaternion rotation)
    {
        var sourceRelative = Quaternion.Inverse(back.transform.rotation) * rotation;
        return LinkedPortal.front.transform.rotation * sourceRelative;
    }
}

[CustomEditor(typeof(Portal))]
class PortalLinkage : Editor
{
    void OnSceneGUI()
    {
        Portal portal = target as Portal;

        //if (portal?.LinkedPortal != null)
        //{
        //    Handles.DrawLine(portal.transform.position,
        //                     portal.LinkedPortal.transform.position);

        //    //Handles.DrawLine(portal.transform.position, portal.transform.position + portal.transform.right * 2, 3);
        //    //Handles.DrawLine(portal.transform.position, portal.transform.position + portal.transform.forward * 2, 8);
        //    //Handles.DrawLine(portal.transform.position, portal.transform.position + portal.transform.up * 2, 13);

        //    Handles.DrawLine(portal.transform.position, portal.transform.position + portal.Forward * 2f, 4);
        //    Handles.DrawLine(portal.transform.position, portal.transform.position + portal.Up * 1.5f, 8);
        //}
    }
}