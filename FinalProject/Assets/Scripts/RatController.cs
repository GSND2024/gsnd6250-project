using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RatController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float turnSpeed = 180f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Make sure your Project Settings → Player → Active Input Handling is set to "Both" or "Input Manager"
        float horizontal = Input.GetAxis("Horizontal"); // A / D or Left / Right
        float vertical = Input.GetAxis("Vertical");   // W / S or Up / Down

        // Turn left/right
        transform.Rotate(0f, horizontal * turnSpeed * Time.fixedDeltaTime, 0f);

        // Move forward/back
        Vector3 move = transform.forward * vertical * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }
}
