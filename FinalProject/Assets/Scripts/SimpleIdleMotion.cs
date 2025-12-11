using UnityEngine;

public class SimpleIdleMotion : MonoBehaviour
{
    [Header("Vertical Bob")]
    [SerializeField] float bobAmplitude = 0.02f;   // how much up/down (meters)
    [SerializeField] float bobSpeed = 1.2f;        // how fast

    [Header("Sway (Lean)")]
    [SerializeField] float swayAngle = 2.5f;       // degrees left/right
    [SerializeField] float swaySpeed = 0.7f;       // how fast

    [Header("Twist (Subtle Turn)")]
    [SerializeField] float twistAngle = 2f;        // tiny yaw twist
    [SerializeField] float twistSpeed = 0.4f;

    Vector3 _baseLocalPos;
    Quaternion _baseLocalRot;
    float _phaseOffset;

    void Awake()
    {
        _baseLocalPos = transform.localPosition;
        _baseLocalRot = transform.localRotation;
        _phaseOffset = Random.Range(0f, 100f);   // so multiple NPCs are desynced
    }

    void LateUpdate()
    {
        float t = Time.time + _phaseOffset;

        // --- Bob up and down (breathing / slight shifting) ---
        float bob = Mathf.Sin(t * bobSpeed) * bobAmplitude;

        Vector3 pos = _baseLocalPos;
        pos.y += bob;

        // --- Lean side to side (as if shifting weight) ---
        float sway = Mathf.Sin(t * swaySpeed) * swayAngle;

        // --- Tiny torso twist ---
        float twist = Mathf.Sin(t * twistSpeed) * twistAngle;

        Quaternion rot =
            _baseLocalRot *
            Quaternion.Euler(sway, twist, 0f);

        transform.localPosition = pos;
        transform.localRotation = rot;
    }

    // Call this if you ever teleport / re-position the character in code
    public void ResetBasePose()
    {
        _baseLocalPos = transform.localPosition;
        _baseLocalRot = transform.localRotation;
    }
}
