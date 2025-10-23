using UnityEngine;

public class ChandelierSway : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] private float swayAngle = 5f;      // Max rotation angle in degrees
    [SerializeField] private float swaySpeed = 1f;      // Speed of sway
    [SerializeField] private Vector3 swayAxis = Vector3.forward; // Axis to sway around

    private float _timeOffset;

    void Start()
    {
        // Add a small random offset so multiple chandeliers don't sway identically
        _timeOffset = Random.Range(0f, 10f);
    }

    void Update()
    {
        float angle = Mathf.Sin((Time.time + _timeOffset) * swaySpeed) * swayAngle;
        transform.localRotation = Quaternion.Euler(swayAxis * angle);
    }
}
