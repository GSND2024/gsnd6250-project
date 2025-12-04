using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController3D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float gravity = -20f;

    [Header("Mouse Look")]
    [SerializeField] Transform cameraPivot;
    [SerializeField] float mouseSensitivity = 2.0f;
    [SerializeField] float minPitch = -80f;
    [SerializeField] float maxPitch = 80f;

    float _pitch = 0f;
    float _yVel = 0f;
    CharacterController _cc;
    
    public float externalRecoil = 0f;

    void Awake()
    {
        _cc = GetComponent<CharacterController>();
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
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        _pitch -= mouseY;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

        externalRecoil = Mathf.Lerp(externalRecoil, 0f, Time.unscaledDeltaTime * 2f);

        float finalPitch = _pitch - externalRecoil;

        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(finalPitch, 0f, 0f);
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(h, 0f, v).normalized;
        Vector3 move = transform.TransformDirection(input) * moveSpeed;

        if (_cc.isGrounded)
            _yVel = -1f;
        _yVel += gravity * Time.deltaTime;
        move.y = _yVel;

        _cc.Move(move * Time.deltaTime);
    }
}