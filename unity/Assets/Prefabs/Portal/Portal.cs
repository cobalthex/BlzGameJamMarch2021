// based off https://medium.com/@limdingwen_66715/multiple-recursive-portals-and-ai-in-unity-part-1-basic-portal-rendering-7c3d957f656c
// and https://github.com/SebLague/Portals/

using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

public class Portal : MonoBehaviour
{
    [Range(0, 10)]
    public int MaxRecursion = 5; // this renders the scene this many times, so this can get expensive

    public enum EnteringBackBehavior
    {
        Passthru,
        Block,
        Teleport,
    }

    struct Subrender
    {
        public Portal portal;
        public Vector3 position;
        public Quaternion rotation;

        public override string ToString() => $"{portal} {position} {rotation}";
    }


    public Portal LinkedPortal;
    public int MaxUseCount = -1;

    public EnteringBackBehavior EnteringFromBackBehavior;
    

    int useCount = 0;

    Camera portalCamera;
    Renderer surface;
    RenderTexture surfaceTarget;

    public RenderTexture Fucker;

    Camera viewCamera; // camera looking at this portal

    Transform back;
    Transform front;
    Vector4 portalPlane;

    static int frame;
    static int portalsRendered;

    private Portal[] visiblePortals;
    Stack<Subrender> subrenders = new Stack<Subrender>();

    //public Vector3 Forward => transform.up;
    //public Vector3 Up => -transform.right;

    bool IsMirror => this == LinkedPortal;

    void Awake()
    {
        portalCamera = transform.Find("Viewfinder").GetComponent<Camera>(); // unity sucks
        surface = GetComponent<Renderer>();

        // todo: can probably calculate these
        back = transform.Find("back");
        front = transform.Find("front");
    }

    void Start()
    {
        GetComponent<Camera>().enabled = true;

        surfaceTarget = new RenderTexture(Screen.width, Screen.height, 0)
        {
            name = $"[surface target ({name})]"
        };
        //surfaceTarget.Create();

        //surface.material.mainTexture = surfaceTarget;
        if (LinkedPortal != null)
            LinkedPortal.surface.material.mainTexture = surfaceTarget;
        portalCamera.targetTexture = surfaceTarget;

        portalCamera.enabled = false;

        // todo: if portal is self-portaling (mirror), flip texture on horizontal access

        viewCamera = Camera.main;

        var plane = new Plane(front.forward, transform.position);
        portalPlane = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);

        visiblePortals = FindObjectsOfType<Portal>(true);
        visiblePortals = visiblePortals.Where(p =>
        {
            if (this == p)
                return false;

            if (!Physics.Raycast(front.position, (p.front.position - front.position).normalized, out var hit))
                return false;

            return hit.transform == p.transform;
        }).ToArray();
    }

    void OnDestroy()
    {
        surfaceTarget.Release();
        Destroy(surfaceTarget);
    }

    static bool CanSee(Camera from, Portal other, bool raycast = false)
    {
        // is the portal is facing the camera
        var dot = Vector3.Dot(from.transform.position - other.front.position, other.front.forward);
        if (dot < 0)
            return false;

        // is the portal in the camera's view frustum
        if (!Utility.InCamerasFrustum(from, other.surface)) // this should be done automatically
            return false;

        // create a collider with layer Portal to block line-of-sight
        //{
        //    var toView = (viewCamera.transform.position - front.position);
        //    var toViewLength = toView.magnitude;
        //    if (Physics.Raycast(new Ray(front.position, toView / toViewLength), out var hit, toViewLength, ~LayerMask.NameToLayer("Portal")))
        //    {
        //        occluded = true;
        //        return true;
        //    }
        //}

        return true;
    }

    List<Subrender> draws = new List<Subrender>(); // DEBUG

    bool occluded = false;

    private void OnPreCull()
    {
        if (!CanSee(viewCamera, this))
        {
            occluded = true;
            return;
        }

        if (frame != Time.frameCount)
        {
            frame = Time.frameCount;
            portalsRendered = 0;
        }

        ++portalsRendered;
        occluded = false;

        subrenders.Push(new Subrender
        {
            portal = this,
            position = GetTargetRelativePosition(viewCamera.transform.position),
            rotation = GetTargetRelativeRotation(viewCamera.transform.rotation),
        });

        var localToWorldMatrix = viewCamera.transform.localToWorldMatrix;

        // calculate the positions of each recursive render
        for (int i = 0; i <= MaxRecursion; ++i) // one extra that is drawn specially
        {
            var top = subrenders.Peek();

            if (top.portal.LinkedPortal == null)
                break;

            // todo: occlude

            var next = new Subrender
            {
                portal = top.portal.LinkedPortal,
                position = top.position + front.forward * 2,
                rotation = top.portal.GetTargetRelativeRotation(top.rotation),
            };
            subrenders.Push(next);
        }
        // draw something if at max recursion

        surface.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        LinkedPortal.surface.material.SetInt("displayMask", 0);

        draws.Clear();
        while (subrenders.Count > 0)
        {
            var top = subrenders.Pop();
            if (subrenders.Count == MaxRecursion)
            {
                continue;
            }

            draws.Add(top);
            portalCamera.transform.SetPositionAndRotation(top.position, top.rotation);
            SetCameraClipMatrix();

            portalCamera.Render();
        }

        surface.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }

    void SetCameraClipMatrix()
    {
        var clipMatrix = Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * LinkedPortal.portalPlane;
        var obliqueProjectionMatrix = viewCamera.CalculateObliqueMatrix(clipMatrix); // todo: this needs to be relative to subrender position
        portalCamera.projectionMatrix = obliqueProjectionMatrix;
    }

    Vector3 GetTargetRelativePosition(Vector3 position)
    {
        if (IsMirror)
        {
            // todo
            return front.position - front.forward * 0.01f;
        }

        return LinkedPortal.back.TransformPoint(front.InverseTransformPoint(position));
    }

    Quaternion GetTargetRelativeRotation(Quaternion rotation)
    {
        if (IsMirror)
        {
            // todo
            var q = Quaternion.LookRotation(Vector3.Reflect(viewCamera.transform.forward, back.forward), viewCamera.transform.up);
            return q;

            rotation.y *= -1;
            rotation.z *= -1;
            return rotation;
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
        if (LinkedPortal != null)
        {
            Handles.color = new Color(0.25f, 0, 1f);
            Handles.DrawAAPolyLine(3, front.position, LinkedPortal.front.position);
        }

        Handles.color = new Color(0, 1, 0.25f);
        if (visiblePortals != null)
        {
            foreach (var visible in visiblePortals)
                Handles.DrawAAPolyLine(1, front.position, visible.front.position);
        }

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

        for (int i = 0; i < draws.Count; ++i)
        {
            var c = Color.Lerp(Color.red, Color.blue, i / (float)draws.Count);
            var pos = draws[i].position + new Vector3(0, 0.03f * i, 0);
            EditorDrawUtils.DrawArrow(3, pos, pos + draws[i].rotation * Vector3.forward, Vector3.up, c);
        }
    }
}