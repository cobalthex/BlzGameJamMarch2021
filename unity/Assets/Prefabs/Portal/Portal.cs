// based off https://medium.com/@limdingwen_66715/multiple-recursive-portals-and-ai-in-unity-part-1-basic-portal-rendering-7c3d957f656c

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Portal : MonoBehaviour
{
    const int MaxRecursion = 5; // this renders the scene this many times, so this can get expensive

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

    static bool InCamerasFrustum(Renderer renderer, Camera camera)
    {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
    }

    readonly Vector3[] renderPositions = new Vector3[MaxRecursion];
    readonly Quaternion[] renderRotations = new Quaternion[MaxRecursion];
    bool occluded = false;

    void OnRenderObject()
    {
        if (!InCamerasFrustum(viewthrough, viewCamera)) // this should be done automatically
        {
#if UNITY_EDITOR
            occluded = true;
#endif
            return;
        }

        var dot = Vector3.Dot(viewCamera.transform.position - front.position, front.forward);
        if (dot < 0)
        {
#if UNITY_EDITOR
            occluded = true;
#endif
            return;
        }

        occluded = false;

        if (LinkedPortal == null)
        {
            viewthroughRt.DiscardContents();
            return;
        }

        var lookPosition = GetTargetRelativePosition(viewCamera.transform.position);
        var lookRotation = GetTargetRelativeRotation(viewCamera.transform.rotation);

        portalCamera.transform.SetPositionAndRotation(lookPosition, lookRotation);

        var localToWorldMatrix = viewCamera.transform.localToWorldMatrix;

        viewthrough.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        LinkedPortal.viewthrough.material.SetInt("displayMask", 0);

        SetCameraClipMatrix();
        portalCamera.Render();

        viewthrough.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
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

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (occluded)
            return;
#endif

        front ??= transform.Find("front");
        Handles_DrawArrow(6, front.position, front.position - front.right, front.forward, Color.green); // up

        Handles_DrawArrow(3, front.position, front.position + front.forward, -front.right, Color.blue); // forward
    }

    void Handles_DrawArrow(float width, Vector3 tail, Vector3 nose, Vector3 wingPlaneNormal, Color color)
    {
        Handles.color = color;
        Handles.DrawAAPolyLine(width, tail, nose);

        var wingLength = (tail - nose).magnitude;
        var wingTangent = ((tail - nose) / wingLength) / 3;

        var left = nose + Quaternion.AngleAxis(-30, wingPlaneNormal) * wingTangent;
        var right = nose + Quaternion.AngleAxis(30, wingPlaneNormal) * wingTangent;
        Handles.DrawAAPolyLine(width, left, nose, right);
    }
}