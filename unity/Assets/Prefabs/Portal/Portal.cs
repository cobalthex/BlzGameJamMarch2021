// based off https://medium.com/@limdingwen_66715/multiple-recursive-portals-and-ai-in-unity-part-1-basic-portal-rendering-7c3d957f656c

//#define RECURSIVE_PORTALS

using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

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

    static int frame;
    static int portalsRendered;

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
#if RECURSIVE_PORTALS
        portalCamera.enabled = false;
#else
        portalCamera.enabled = true;
#endif

        // todo: if portal is self-portaling (mirror), flip texture on horizontal access

        viewCamera = Camera.main;

        var plane = new Plane(front.forward, transform.position);
        portalPlane = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);
    }

    void OnDestroy()
    {
        viewthroughRt.Release();
        Destroy(viewthroughRt);
    }

    void SetCameraClipMatrix()
    {
        var clipMatrix = Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * LinkedPortal.portalPlane;
        var obliqueProjectionMatrix = viewCamera.CalculateObliqueMatrix(clipMatrix);
        portalCamera.projectionMatrix = obliqueProjectionMatrix;
    }

    static bool InCamerasFrustum(Renderer renderer, Camera camera)
    {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
    }

    readonly Vector3[] renderPositions = new Vector3[MaxRecursion];
    readonly Quaternion[] renderRotations = new Quaternion[MaxRecursion];
    bool occluded = false;


#if RECURSIVE_PORTALS

    void OnRenderObject()
    {
        if (LinkedPortal == null)
        {
            viewthroughRt.DiscardContents();
            return;
        }

        if (portalsRendered > 2)
        {
            Debug.Log("Trying to render too many portals");
            return;
        }

        var dot = Vector3.Dot(viewCamera.transform.position - front.position, front.forward);
        if (dot < 0)
        {
            occluded = true;
            return;
        }

        if (!InCamerasFrustum(viewthrough, viewCamera)) // this should be done automatically
        {
            occluded = true;
            return;
        }

        // create a collider with layer Portal to block line-of-sight
        {
            var toView = (viewCamera.transform.position - front.position);
            var toViewLength = toView.magnitude;
            if (Physics.Raycast(new Ray(front.position, toView / toViewLength), out var hit, toViewLength, ~LayerMask.NameToLayer("Portal")))
            {
                occluded = true;
                return;
            }
        }

        if (frame != Time.frameCount)
        {
            frame = Time.frameCount;
            portalsRendered = 0;
        }

        ++portalsRendered;
        occluded = false;

        var localToWorldMatrix = viewCamera.transform.localToWorldMatrix;

        viewthrough.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        LinkedPortal.viewthrough.material.SetInt("displayMask", 0);

        SetCameraView();
        SetCameraClipMatrix();
        portalCamera.Render();
        // todo: recursive rendering

        viewthrough.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }

#else

    void LateUpdate()
    {
        SetCameraView();
        SetCameraClipMatrix();
    }

#endif

    void SetCameraView()
    {
        var lookPosition = GetTargetRelativePosition(viewCamera.transform.position);
        var lookRotation = GetTargetRelativeRotation(viewCamera.transform.rotation);
        portalCamera.transform.SetPositionAndRotation(lookPosition, lookRotation);
    }

    Vector3 GetTargetRelativePosition(Vector3 position)
    {
        if (IsMirror)
        {
            return front.position - front.forward * 0.01f;

            //var d = position - front.position;
            //var dot = Vector3.Dot(d, front.right);

            //return front.position - (front.forward * 0.01f) - (front.right * dot);

            //return front.position + Vector3.Reflect(position - front.position, front.forward);

            //var p = new Plane(front.forward, front.position);
            //var r = new Ray(viewCamera.transform.position, viewCamera.transform.forward);
            //bool _hit = p.Raycast(r, out var d);
            //return r.GetPoint(d + 0.01f);
        }

        return LinkedPortal.back.TransformPoint(front.InverseTransformPoint(position));
    }

    Quaternion GetTargetRelativeRotation(Quaternion rotation)
    {
        if (IsMirror)
        {
            Vector3 dir = (viewCamera.transform.position - front.position).normalized;
            Quaternion rot = Quaternion.LookRotation(dir);

            rot.eulerAngles = (front.eulerAngles - rot.eulerAngles).normalized; // todo: use quat

            return rot;


            //var q = Quaternion.LookRotation(Vector3.Reflect(viewCamera.transform.forward, back.forward), viewCamera.transform.up);
            //return q;
            //rotation.y *= -1;
            //rotation.z *= -1;
            //return rotation;
        }

        // todo: this does not work if entering the portal backwards (always places user forward)
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
        if (LinkedPortal == null || (MaxUseCount >= 0 && useCount > MaxUseCount))
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
        Handles.color = new Color(0.25f, 0, 1f);
        Handles.DrawAAPolyLine(2, front.position, LinkedPortal.front.position);

        var pct = portalCamera.transform;
        EditorDrawUtils.DrawArrow(4, pct.position, pct.position + pct.forward, pct.up, Color.white);
    }

    void OnDrawGizmos()
    {
        if (occluded)
            return;

        front ??= transform.Find("front");
        EditorDrawUtils.DrawArrow(6, front.position, front.position - front.right, front.forward, Color.green); // up
        EditorDrawUtils.DrawArrow(3, front.position, front.position + front.forward, -front.right, Color.blue); // forward
    }
}