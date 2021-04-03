using UnityEngine;

public class DrawArrowInEditor : MonoBehaviour
{
    public Color Color = new Color(0, 0.4f, 1, 1);
    public float Thickness = 10;
    public float Length = 2;

    private void OnDrawGizmos()
    {
        EditorDrawUtils.DrawArrow(Thickness, transform.position, transform.position + transform.forward * Length, transform.up, Color);
    }
}
