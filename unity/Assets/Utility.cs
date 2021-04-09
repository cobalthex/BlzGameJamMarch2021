using UnityEngine;

public static class Utility
{
    public static Vector3 VectorMultiply(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static bool InCamerasFrustum(Camera camera, Renderer renderer)
    {
        // calculate once per frame?
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
    }
}
