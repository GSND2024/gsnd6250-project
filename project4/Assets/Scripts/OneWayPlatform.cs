using UnityEngine;

/// <summary>
/// One-way collision using PlatformEffector2D that obeys rotation.
/// The side pointed by the object's local 'Up' (green Y axis) is the BLOCKED/solid side.
/// Rotate the object to choose which side is solid.
/// </summary>
[RequireComponent(typeof(PlatformEffector2D))]
[RequireComponent(typeof(Collider2D))]
public class OneWayPlatform2D : MonoBehaviour
{
    [Tooltip("Angular width (in degrees) of the solid side. 180 blocks a half-circle.")]
    [Range(1f, 180f)] public float surfaceArc = 180f;

    [Tooltip("Set true so Player can stand on it as Ground. Keep the collider on your Ground layer if needed.")]
    public bool markAsGroundLayer = true;

    PlatformEffector2D _effector;
    Collider2D _col;

    void Reset()
    {
        _effector = GetComponent<PlatformEffector2D>();
        _col = GetComponent<Collider2D>();

        // Collider must be "Used By Effector"
        _col.usedByEffector = true;

        _effector.useOneWay = true;
        _effector.useOneWayGrouping = true; // better stacking
        _effector.surfaceArc = surfaceArc;
        _effector.rotationalOffset = 0f; // we use transform rotation instead
    }

    void OnValidate()
    {
        if (!_effector) _effector = GetComponent<PlatformEffector2D>();
        if (_effector) _effector.surfaceArc = surfaceArc;
        if (!_col) _col = GetComponent<Collider2D>();
        if (_col) _col.usedByEffector = true;
    }

    void OnDrawGizmos()
    {
        // Draw an arrow for the BLOCKED side (local up)
        Gizmos.color = Color.green;
        Vector3 p = transform.position;
        Vector3 dir = transform.up;
        Gizmos.DrawLine(p, p + dir * 1.5f);
        Gizmos.DrawSphere(p + dir * 1.5f, 0.06f);
    }
}
