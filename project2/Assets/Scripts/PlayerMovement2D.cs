// File: Assets/Scripts/PlayerMovement2D.cs
// Attach to: Player (Rigidbody2D + CapsuleCollider2D)

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // New Input System
using UnityEngine.InputSystem.LowLevel;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Activate")]
    [SerializeField]public GameObject[] objectsToActivate;
    
    [Header("Move")]
    [SerializeField] private float moveSpeed = 9f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.12f;
    [SerializeField] private LayerMask groundMask;

    [Header("Jump Quality")]
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = false;
    
   

    private Rigidbody2D rb;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private bool isGrounded;
    private float inputX;

    // New Input System devices/actions
    private Keyboard kb;
    private Gamepad pad;
    private InputAction jumpAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Create a dedicated Jump action that listens to Space (and optional extras)
        jumpAction = new InputAction(type: InputActionType.Button);
        jumpAction.AddBinding("<Keyboard>/space");
        // Optional extras (uncomment if you want):
        // jumpAction.AddBinding("<Keyboard>/w");
        // jumpAction.AddBinding("<Keyboard>/upArrow");
        // jumpAction.AddBinding("<Gamepad>/buttonSouth");

        jumpAction.performed += _ =>
        {
            jumpBufferCounter = jumpBufferTime;
            if (debugLogs) Debug.Log("[Jump] performed ? buffer set");
        };
    }

    private void OnEnable()
    {
        jumpAction.Enable();
        kb = Keyboard.current;
        pad = Gamepad.current;
    }

    private void OnDisable()
    {
        jumpAction.Disable();
    }

    private void Start()
    {
        // Common setup pitfalls ? warn once at start
        if (groundCheck == null)
            Debug.LogWarning("PlayerMovement2D: GroundCheck is not assigned. Drag the GroundCheck child into the script slot.");
        if (groundMask.value == 0)
            Debug.LogWarning("PlayerMovement2D: Ground Mask is 0. Set it to the 'Ground' layer in the Inspector.");
        if (rb.gravityScale < 0.5f)
            Debug.LogWarning("PlayerMovement2D: Gravity Scale seems low. Try 3 for platformer feel.");
    }

    private void Update()
    {
        // Horizontal input (WASD + Arrows)
        inputX = 0f;
        if ((kb?.aKey?.isPressed ?? false) || (kb?.leftArrowKey?.isPressed ?? false)) inputX -= 1f;
        if ((kb?.dKey?.isPressed ?? false) || (kb?.rightArrowKey?.isPressed ?? false)) inputX += 1f;
        if (pad != null)
        {
            float stick = Mathf.Abs(pad.leftStick.x.ReadValue()) > 0.1f ? pad.leftStick.x.ReadValue() : 0f;
            if (Mathf.Abs(stick) > Mathf.Abs(inputX)) inputX = stick; // prefer stick if larger
        }

        jumpBufferCounter -= Time.unscaledDeltaTime;

        // Ground check
        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);
        else
            isGrounded = false;

        if (isGrounded) coyoteCounter = coyoteTime;
        else coyoteCounter -= Time.unscaledDeltaTime;

        // Perform jump if buffered + allowed by coyote
        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            // Reset vertical speed for a crisp takeoff
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            if (debugLogs) Debug.Log("[Jump] fired");
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
        }

        // Better fall
        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * 1.5f * Time.unscaledDeltaTime;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocity.y);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
    
     private void OnTriggerEnter2D(Collider2D other)
     {
         if (other.CompareTag("Win"))
         {
             SceneManager.LoadScene("WinScreen");
         }
     }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag($"Trap"))
        {
            ActivateObjects();
        }
    }
    
    private void ActivateObjects()
    {
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }
}
