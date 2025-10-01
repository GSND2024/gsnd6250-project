// File: CameraFollow2D.cs
// Attach to: Main Camera

using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;
    public float smooth = 8f;
    public Vector2 offset = new Vector2(0f, 0.5f);

    private void LateUpdate()
    {
        if (!target) return;
        Vector3 goal = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, goal, smooth * Time.deltaTime);
    }
}
