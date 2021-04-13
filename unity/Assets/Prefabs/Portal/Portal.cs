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
        public int depth;

        public override string ToString() => $"{portal} {position} {rotation}";
    }


    public Portal LinkedPortal;
    public int MaxUseCount = -1;

    public EnteringBackBehavior EnteringFromBackBehavior;

    public Texture2D MaxRecursionTexture;

    int useCount = 0;

    Camera portalCamera;
    Renderer surface;
    RenderTexture surfaceTarget;

    Transform back;
    Transform front;
    Vector4 portalPlane;

    static int frame;

    public Portal[] VisiblePortals { get; private set; }
    Stack<Subrender> subrenders = new Stack<Subrender>();
    public int SubRenderCount { get; private set; }

    public Vector3 Forward => front.transform.forward;
    public Vector3 Up => -front.transform.right;
    public Vector3 Right => front.transform.up;

    public bool Mirror;

    void Awake()
    {
        portalCamera = transform.Find("Viewfinder").GetComponent<Camera>(); // unity sucks
        surface = GetComponent<Renderer>();

        // todo: can probably calculate these
        back = transform.Find("back");
        front = transform.Find("front");

        GetComponent<Camera>().enabled = true;
    }

    void Start()
    {
        surfaceTarget = new RenderTexture(Screen.width, Screen.height, 0)//, RenderTextureFormat.DefaultHDR)
        {
            name = $"[surface target ({name})]",
        };
        //surfaceTarget.Create();

        surface.material.mainTexture = surfaceTarget;
        portalCamera.targetTexture = surfaceTarget;

        surface.material.SetInt("_IsMirror", Mirror ? 1 : 0);

        portalCamera.enabled = false;

        var plane = new Plane(front.forward, front.position);
        portalPlane = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);

        portalCamera.transform.position = front.position;
        portalCamera.transform.rotation = Quaternion.LookRotation(Forward, Up);

        VisiblePortals = FindObjectsOfType<Portal>(true);
        VisiblePortals = VisiblePortals.Where(p =>
        {
            if (this == p)
                return false;

            return CanSee(portalCamera, p, false);
        }).ToArray();
    }

    void OnDestroy()
    {
        surfaceTarget.Release();
        Destroy(surfaceTarget);
    }

    static bool CanSee(Camera from, Portal other, bool raycast = false)
    {
        var diff = from.transform.position - other.front.position;

        // is the other portal in front of the camera
        if (Vector3.Dot(diff, from.transform.forward) >= 0)
            return false;

        // is the other portal facing the camera
        if (Vector3.Dot(diff, other.front.forward) < 0)
            return false;

        // is the portal in the camera's view frustum
        if (!Utility.InCamerasFrustum(from, other.surface)) // this should be done automatically
            return false;

        if (raycast)
        { 
            // todo
            //if (!Physics.Raycast(front.position, (p.front.position - front.position).normalized, out var hit))
            //    return false;

            // hit.transform == p.transform
        }

        return true;
    }

    int lastRenderFrame = 0;

    void OnPreRender()
    {
        // no idea if this is good or bad
        //if (lastRenderFrame == Time.frameCount)
        //    return;
        //lastRenderFrame = Time.frameCount;

        if (LinkedPortal == null)
            return;
        
        var viewCamera = Camera.main;

        // don't render this portal if it's not in view (since apparently unity won't do this)
        if (!CanSee(viewCamera, this, false))
            return;

#if UNITY_EDITOR && DEBUG
        if (frame != Time.frameCount)
        {
            frame = Time.frameCount;
            PortalDebug.RenderedPortals.Clear();
        }
        PortalDebug.RenderedPortals.Add(this);
#endif

        // render the linked portals of all subrenders
        subrenders.Push(new Subrender
        {
            portal = this,
            position = GetTargetRelativePosition(viewCamera.transform.position),
            rotation = GetTargetRelativeRotation(viewCamera.transform.rotation),
            depth = 0,
        });

        for (int i = 0; i <= MaxRecursion; ++i)
        {
            var top = subrenders.Peek();

            int startingPortals = subrenders.Count;
            foreach (var visible in top.portal.LinkedPortal.VisiblePortals)
            {
                if (visible.LinkedPortal == null || !visible.enabled)
                    continue; // disabled texture?

                // portal might not be visible at some angles
                portalCamera.transform.SetPositionAndRotation(top.position, top.rotation); // moving camera not ideal
                if (!Utility.InCamerasFrustum(portalCamera, visible.surface))
                    continue;

                subrenders.Push(new Subrender
                {
                    portal = visible,
                    position = visible.GetTargetRelativePosition(top.position),
                    rotation = visible.GetTargetRelativeRotation(top.rotation),
                    depth = i,
                });
            }

            if (subrenders.Count == startingPortals)
                break;
        }

        SubRenderCount = subrenders.Count;
        while (subrenders.Count > 0)
        {
            var top = subrenders.Pop();
            if (top.depth >= MaxRecursion)
            {
                if (MaxRecursionTexture != null)
                    Graphics.Blit(MaxRecursionTexture, top.portal.surfaceTarget);
                continue;
            }

            var p = top.portal;
            p.portalCamera.transform.SetPositionAndRotation(top.position, top.rotation);
            p.portalCamera.projectionMatrix = p.GetCameraClipMatrix(viewCamera, top.portal.LinkedPortal.portalPlane);
            p.portalCamera.Render();
        }
        Debug.Log($"{name} {portalCamera.transform.position} {portalCamera.transform.rotation}");
    }

    Matrix4x4 GetCameraClipMatrix(Camera viewCamera, Vector4 portalPlane)
    {
        var clipMatrix = Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * portalPlane;
        return viewCamera.CalculateObliqueMatrix(clipMatrix);
    }

    Vector3 GetTargetRelativePosition(Vector3 position)
    {
        if (Mirror)
        {
            position -= Vector3.Project(position - front.position, Up) * 2;
            var reflected = front.position - Vector3.Reflect(position - front.position, Right);
            return reflected;
        }
        
        return LinkedPortal.back.TransformPoint(front.InverseTransformPoint(position));
    }

    Quaternion GetTargetRelativeRotation(Quaternion rotation)
    {
        if (Mirror)
        {
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

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (LinkedPortal != null)
        {
            Handles.color = new Color(0.25f, 0, 1f);
            Handles.DrawAAPolyLine(3, front.position, LinkedPortal.front.position);
        }

        Handles.color = new Color(0, 1, 0.25f);
        if (VisiblePortals != null)
        {
            foreach (var visible in VisiblePortals)
                Handles.DrawAAPolyLine(1, front.position, visible.front.position);
        }

        var pct = portalCamera.transform;
        EditorDrawUtils.DrawArrow(4, pct.position, pct.position + pct.forward, pct.up, Color.black);

    }

    void OnDrawGizmos()
    {
        front ??= transform.Find("front");
        EditorDrawUtils.DrawArrow(6, front.position, front.position + Up, Forward, Color.green); // up
        EditorDrawUtils.DrawArrow(3, front.position, front.position + Forward, Up, Color.blue); // forward
    }
#endif
}