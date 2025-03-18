using UnityEngine;

public class CreateLineRenderer : MonoBehaviour
{
    void Start()
    {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;

        Vector3[] positions = new Vector3[2];
        positions[0] = new Vector3(0, 0, 0); // Điểm đầu
        positions[1] = new Vector3(1, 1, 1); // Điểm cuối
        lineRenderer.SetPositions(positions);
    }
}
