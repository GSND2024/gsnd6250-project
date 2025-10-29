using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController3D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float gravity = -20f;

    [Header("Mouse Look")]
    [SerializeField] Transform cameraPivot;    // Assign your Camera transform here (child of Player)
    [SerializeField] float mouseSensitivity = 2.0f;
    [SerializeField] float minPitch = -80f;
    [SerializeField] float maxPitch = 80f;

    float _pitch = 0f;
    float _yVel = 0f;
    CharacterController _cc;

    void Awake()
    {
        _cc = GetComponent<CharacterController>();
        // Lock cursor for FPS feel; press Esc to release while in Play mode
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Look();
        Move();
    }

    void Look()
    {
        // Old Input Manager axes ("Mouse X", "Mouse Y")
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Yaw on player body
        transform.Rotate(Vector3.up * mouseX);

        // Pitch on camera (clamped)
        _pitch -= mouseY;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }

    void Move()
    {
        // WASD on flat plane
        float h = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        float v = Input.GetAxisRaw("Vertical");   // W/S or Up/Down

        Vector3 input = new Vector3(h, 0f, v).normalized;

        // Transform local input to world space (relative to player yaw)
        Vector3 move = transform.TransformDirection(input) * moveSpeed;

        // Simple gravity
        if (_cc.isGrounded)
            _yVel = -1f; // small downward force to stick to ground
        _yVel += gravity * Time.deltaTime;

        move.y = _yVel;

        _cc.Move(move * Time.deltaTime);
    }
}
