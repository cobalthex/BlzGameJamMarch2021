// based off https://medium.com/@limdingwen_66715/multiple-recursive-portals-and-ai-in-unity-part-1-basic-portal-rendering-7c3d957f656c

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Portal : MonoBehaviour
{
    const int MaxRecursion = 5;

    public enum EnteringBackBehavior
    {
        Passthru,
        Block,
        Teleport,
    }

    public Portal LinkedPortal;
    public int MaxUseCount = -1;

    public EnteringBackBehavior EnteringFromBackBehavior;

    int useCount = 0;

    Camera portalCamera;
    Renderer viewthrough;
    RenderTexture viewthroughRt;

    Camera viewCamera;

    Transform back;
    Transform front;
    Vector4 portalPlane;

    //public Vector3 Forward => transform.up;
    //public Vector3 Up => -transform.right;

    bool IsMirror => this == LinkedPortal;

    void Awake()
    {
        portalCamera = transform.Find("Viewfinder").GetComponent<Camera>(); // unity sucks
        viewthrough = GetComponent<Renderer>();
        back = transform.Find("back");
        front = transform.Find("front");
    }

    void Start()
    {
        GetComponent<Camera>().enabled = true;

        viewthroughRt = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.DefaultHDR);
        // viewthroughRt.Create(); // created on-demand

        viewthrough.material.mainTexture = viewthroughRt;
        portalCamera.targetTexture = viewthroughRt;
        portalCamera.enabled = false;

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

    void SetCameraClipMatrix()
    {
        var clipMatrix = Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * LinkedPortal.portalPlane;
        var obliqueProjectionMatrix = viewCamera.CalculateObliqueMatrix(clipMatrix);
        portalCamera.projectionMatrix = obliqueProjectionMatrix;
    }

    void LateUpdate()
    {

        // creates a view frustrum that is clipping right at the portal's plane

    }

    static bool VisibleFromCamera(Renderer renderer, Camera camera)
    {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
    }

    void OnRenderObject()
    {
        if (!VisibleFromCamera(viewthrough, viewCamera))
            return;

        if (LinkedPortal == null)
        {
            viewthroughRt.DiscardContents();
            return;
        }

        var lookPosition = GetTargetRelativePosition(viewCamera.transform.position);
        var lookRotation = GetTargetRelativeRotation(viewCamera.transform.rotation);

        portalCamera.transform.SetPositionAndRotation(lookPosition, lookRotation);

        SetCameraClipMatrix();
        portalCamera.Render();
    }

    void OnDestroy()
    {
        viewthroughRt.Release();
        Destroy(viewthroughRt);
    }

    Vector3 GetTargetRelativePosition(Vector3 position)
    {
        // todo: this needs to work with mirrors

        return LinkedPortal.back.TransformPoint(front.InverseTransformPoint(position));
    }

    Quaternion GetTargetRelativeRotation(Quaternion rotation)
    {
        var sourceRelative = Quaternion.Inverse(back.transform.rotation) * rotation;
        return LinkedPortal.front.rotation * sourceRelative;
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

    void OnDrawGizmosSelected()
    {
        if (LinkedPortal == this)
        {
            Handles.color = new Color(1, 0.75f, 0.3f); // different colors per instance ID?
            Handles.DrawSolidDisc(transform.position, transform.forward, 0.5f);
        }
        else if (LinkedPortal != null)
        {
            Handles.color = new Color(0.35f, 0.3f, 1); // different colors per instance ID?
            Handles.DrawSolidDisc(transform.position, transform.forward, 0.5f);
            Handles.DrawAAPolyLine(2, transform.position, LinkedPortal.transform.position);
        }
    }

    void OnDrawGizmos()
    {
        Handles.color = Color.blue;
        Handles.DrawAAPolyLine(6, transform.position, transform.position - transform.right); // up
    }
}