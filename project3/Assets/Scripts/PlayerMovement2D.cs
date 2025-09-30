using UnityEngine;
using UnityEngine.InputSystem; // New Input System
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
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

    [Header("Attacks")]
    [SerializeField] private GameObject attackJ;
    [SerializeField] private GameObject attackK;
    [SerializeField] private GameObject attackL;
    [SerializeField] private GameObject attackI;

    [Header("Attack Lifetime")]
    [SerializeField] private float attackActiveTime = 0.1f;
    
    [Header("Health")]
    [SerializeField] private int maxHealth = 5;     
    private int currentHealth;
    [SerializeField] private Slider healthSlider; 

    [Header("Debug")]
    [SerializeField] private bool debugLogs = false;

    private Rigidbody2D rb;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private bool isGrounded;
    private float inputX;

    // Input system
    private Keyboard kb;
    private Gamepad pad;

    private InputAction jumpAction;
    private InputAction jAction;
    private InputAction kAction;
    private InputAction lAction;
    private InputAction iAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        currentHealth = maxHealth; 

        // Jump (Space) -> sets a buffer that Update() will consume with coyote time
        jumpAction = new InputAction(type: InputActionType.Button);
        jumpAction.AddBinding("<Keyboard>/space");
        jumpAction.performed += _ =>
        {
            jumpBufferCounter = jumpBufferTime;
            if (debugLogs) Debug.Log("[Jump] performed -> buffer set");
        };

        // J
        jAction = new InputAction(type: InputActionType.Button);
        jAction.AddBinding("<Keyboard>/j");
        jAction.performed += _ => ActivateAttackTemporarily(attackJ, "J");

        // K
        kAction = new InputAction(type: InputActionType.Button);
        kAction.AddBinding("<Keyboard>/k");
        kAction.performed += _ => ActivateAttackTemporarily(attackK, "K");

        // L
        lAction = new InputAction(type: InputActionType.Button);
        lAction.AddBinding("<Keyboard>/l");
        lAction.performed += _ => ActivateAttackTemporarily(attackL, "L");

        // I
        iAction = new InputAction(type: InputActionType.Button);
        iAction.AddBinding("<Keyboard>/i");
        iAction.performed += _ => ActivateAttackTemporarily(attackI, "I");
    }

    private void OnEnable()
    {
        jumpAction.Enable();
        jAction.Enable();
        kAction.Enable();
        lAction.Enable();
        iAction.Enable();

        kb = Keyboard.current;
        pad = Gamepad.current;
    }

    private void OnDisable()
    {
        jumpAction.Disable();
        jAction.Disable();
        kAction.Disable();
        lAction.Disable();
        iAction.Disable();
    }

    private void Start()
    {
        if (groundCheck == null)
            Debug.LogWarning("PlayerMovement2D: GroundCheck is not assigned.");
        if (groundMask.value == 0)
            Debug.LogWarning("PlayerMovement2D: Ground Mask is 0.");
        if (rb.gravityScale < 0.5f)
            Debug.LogWarning("PlayerMovement2D: Gravity Scale seems low.");
        
        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = maxHealth;
            healthSlider.wholeNumbers = true;
            healthSlider.value = currentHealth;
        }
    }

    private void Update()
    {
        // Horizontal input
        inputX = 0f;
        if ((kb?.aKey?.isPressed ?? false) || (kb?.leftArrowKey?.isPressed ?? false)) inputX -= 1f;
        if ((kb?.dKey?.isPressed ?? false) || (kb?.rightArrowKey?.isPressed ?? false)) inputX += 1f;
        if (pad != null)
        {
            float stick = Mathf.Abs(pad.leftStick.x.ReadValue()) > 0.1f ? pad.leftStick.x.ReadValue() : 0f;
            if (Mathf.Abs(stick) > Mathf.Abs(inputX)) inputX = stick;
        }

        // Jump buffer & coyote time
        jumpBufferCounter -= Time.unscaledDeltaTime;

        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);
        else
            isGrounded = false;

        if (isGrounded) coyoteCounter = coyoteTime;
        else coyoteCounter -= Time.unscaledDeltaTime;

        // Consume buffered jump if allowed by coyote
        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            DoJumpImpulse(); // same takeoff behavior used by K-on-hit below
            if (debugLogs) Debug.Log("[Jump] fired");
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
        }

        // Better fall (extra gravity when falling)
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

    // --- Public API for attack relays ---
    // Called by AttackTriggerRelay2D on each attack object when it hits a "Boss"
    public void OnAttackHit(string key, Collider2D other)
    {
        if (other == null) return;
        if (other.CompareTag("Boss"))
        {
            if (debugLogs) Debug.Log($"[Attack {key}] hit Boss: {other.name}");

            // Special case: K-hit triggers the same action as pressing Jump (immediate takeoff)
            if (key == "K")
            {
                DoJumpImpulse(); // no coyote/buffer checks; immediate jump-like impulse
                if (debugLogs) Debug.Log("[Attack K] applied jump-like impulse");
            }

            // TODO: add damage, stun, hitstop, etc.
        }
    }

    // Apply the same "crisp takeoff" as your buffered jump
    private void DoJumpImpulse()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    // Activate one attack object for attackActiveTime seconds (does not affect others)
    private void ActivateAttackTemporarily(GameObject attackObj, string keyName)
    {
        if (attackObj == null) return;
        if (debugLogs) Debug.Log($"[{keyName}] attack triggered");

        // Ensure it's active for a fixed duration without interfering with other attacks
        StartCoroutine(ActivateForSeconds(attackObj, attackActiveTime));
    }

    private IEnumerator ActivateForSeconds(GameObject go, float seconds)
    {
        go.SetActive(true);
        yield return new WaitForSeconds(seconds);
        // Only deactivate if still active (avoid conflicts if reused/retriggered elsewhere)
        if (go != null) go.SetActive(false);
    }
    
    private void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (debugLogs) Debug.Log($"[Health] Player HP = {currentHealth}");

        if (currentHealth <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            if (debugLogs) Debug.Log("[Health] Player Dead!");
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Boss"))
        {
            TakeDamage(1);
        }
    }
}