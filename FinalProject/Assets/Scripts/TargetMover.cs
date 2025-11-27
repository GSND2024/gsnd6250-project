using UnityEngine;

public class TargetMover : MonoBehaviour
{
    public float speed = 2f;
    public float amplitude = 2f;
    public float phaseOffset = 0f;

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * speed + phaseOffset) * amplitude;
        transform.position = startPos + transform.right * offset;
    }
}