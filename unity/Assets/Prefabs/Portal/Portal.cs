// based off https://medium.com/@limdingwen_66715/multiple-recursive-portals-and-ai-in-unity-part-1-basic-portal-rendering-7c3d957f656c

using UnityEngine;
using UnityEditor;

public class Portal : MonoBehaviour
{
    public Portal LinkedPortal;
    public int MaxUseCount = 1;

    public Vector3 Forward => transform.up;
    public Vector3 Up => -transform.right;

    int useCount = 0;

    Camera portalCamera;
    Renderer viewthrough;
    RenderTexture viewthroughRt;

    Camera viewCamera;

    Transform back;
    Transform front;
    Vector4 portalPlane;

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

        if (LinkedPortal == this)
        {
            // todo: make this work (mirror)
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

    void OnTriggerEnter(Collider collider)
    {
        if (LinkedPortal == null || (MaxUseCount > 0 && useCount > MaxUseCount))
            return;

        // todo: needs to teleport when origin passes thru

        var colliderRigidbody = collider.GetComponent<Rigidbody>();

        var relVelocity = -front.InverseTransformDirection(colliderRigidbody.velocity);
        colliderRigidbody.velocity = LinkedPortal.front.TransformDirection(relVelocity);

        var relPoint = front.InverseTransformPoint(collider.transform.position);
        collider.transform.position = LinkedPortal.front.TransformPoint(relPoint) + colliderRigidbody.velocity.normalized;

        ++useCount;
        Debug.Log(useCount, this);
    }

    //todo: these don't need to be static

    Vector3 GetTargetRelativePosition(Vector3 position)
    {
        return LinkedPortal.back.TransformPoint(front.InverseTransformPoint(position));

        // sort of works (todo: get working)
        //var dist = Vector3.Distance(position, transform.position);
        //var reject = position - Vector3.Project(position, Forward);
        //return (LinkedPortal.transform.position - (LinkedPortal.Forward * dist) + reject);
    }

    Quaternion GetTargetRelativeRotation(Quaternion rotation)
    {
        // todo: should be able to calculate front/back from viewthrough.transform

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

        if (portal?.LinkedPortal != null)
        {
            Handles.DrawLine(portal.transform.position,
                             portal.LinkedPortal.transform.position);

            //Handles.DrawLine(portal.transform.position, portal.transform.position + portal.transform.right * 2, 3);
            //Handles.DrawLine(portal.transform.position, portal.transform.position + portal.transform.forward * 2, 8);
            //Handles.DrawLine(portal.transform.position, portal.transform.position + portal.transform.up * 2, 13);

            Handles.DrawLine(portal.transform.position, portal.transform.position + portal.Forward * 2, 4);
        }
    }
}