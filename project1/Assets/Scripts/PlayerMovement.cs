using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public GameObject body;
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    [Header("Respawn Settings")]
    public float fallThreshold = -50f;          // Y level at which player will respawn
    public Transform defaultSpawnPoint;         // Assign in Inspector (optional)
    public bool resetLookDirectionOnRespawn = true;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0f;
    private CharacterController characterController;
    private bool canMove = true;

    // Internals for respawn
    private Vector3 currentRespawnPoint;
    private Quaternion spawnYawOnly;            // Keep only Y rotation for a typical FPS feel

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialize respawn point
        if (defaultSpawnPoint != null)
        {
            currentRespawnPoint = defaultSpawnPoint.position;
            spawnYawOnly = Quaternion.Euler(0f, defaultSpawnPoint.rotation.eulerAngles.y, 0f);
        }
        else
        {
            currentRespawnPoint = transform.position;
            spawnYawOnly = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        }
    }

    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0f;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0f;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        // --- FALL / RESPAWN CHECK ---
        if (transform.position.y < fallThreshold)
        {
            Respawn();
        }

        /*
        // Old lose-screen logic (kept for reference)
        if (body.transform.position.y < -100)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            SceneManager.LoadScene("Lose");
        }
        */
    }

    /// <summary>
    /// Teleport player to currentRespawnPoint and clear velocity.
    /// </summary>
    void Respawn()
    {
        // Temporarily disable controller to safely set position
        characterController.enabled = false;

        transform.position = currentRespawnPoint;

        // Optionally reset yaw/look
        if (resetLookDirectionOnRespawn)
        {
            transform.rotation = spawnYawOnly;
            rotationX = 0f; // look straight ahead
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        }

        // Clear vertical motion so we don't keep "falling"
        moveDirection = Vector3.zero;

        characterController.enabled = true;
    }

    /// <summary>
    /// Call this from a checkpoint trigger to update the respawn point.
    /// </summary>
    public void SetRespawnPoint(Transform newPoint)
    {
        currentRespawnPoint = newPoint.position;
        spawnYawOnly = Quaternion.Euler(0f, newPoint.rotation.eulerAngles.y, 0f);
    }
}
