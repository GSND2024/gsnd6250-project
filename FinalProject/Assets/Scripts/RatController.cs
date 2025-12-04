using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RatController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float turnSpeed = 180f;

    private Rigidbody rb;
    private bool canMove = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
        if (!canMove)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        if (!canMove) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Turn left/right
        transform.Rotate(0f, horizontal * turnSpeed * Time.fixedDeltaTime, 0f);

        // Move forward/back
        Vector3 move = transform.forward * vertical * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }
}
