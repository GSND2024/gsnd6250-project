using UnityEngine;

public class PingPongMover : MonoBehaviour
{
    [Header("Path (required)")]
    public Transform pointA;
    public Transform pointB;

    [Header("Motion")]
    [SerializeField] float speed = 3f;     // units per second
    [SerializeField] bool smooth = true;   // smooth in/out
    [SerializeField] bool startAtA = true; // where to start

    float _t;           // 0..1 position along the path
    int _dir = 1;       // +1 A->B, -1 B->A

    void Start()
    {
        // Initialize at A or B
        _t = startAtA ? 0f : 1f;
        SnapToPath();
    }

    void Update()
    {
        if (pointA == null || pointB == null) return;

        float delta = (speed * Time.deltaTime) / Vector3.Distance(pointA.position, pointB.position);
        _t += delta * _dir;

        if (_t >= 1f) { _t = 1f; _dir = -1; }
        else if (_t <= 0f) { _t = 0f; _dir = 1; }

        float t = smooth ? Mathf.SmoothStep(0f, 1f, _t) : _t;
        transform.position = Vector3.Lerp(pointA.position, pointB.position, t);
    }

    void SnapToPath()
    {
        if (pointA != null && pointB != null)
            transform.position = Vector3.Lerp(pointA.position, pointB.position, _t);
    }

    // Visualize in editor
    void OnDrawGizmos()
    {
        if (pointA && pointB)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawSphere(pointA.position, 0.1f);
            Gizmos.DrawSphere(pointB.position, 0.1f);
        }
    }
}
