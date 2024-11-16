using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class Movescript : MonoBehaviour
{
    public float initialAcceleration;
    public float accelerationSpeed;
    public float airSpeed;
    public float jumpForce;
    public float jumpHoldForce;
    public float maxJumpHoldTime;

    public float slideDuration = 0.3f; // Duration for which player retains air speed on ground while sliding
    private float slideTimer = 0f;
    private bool isSliding = false;

    public float jumpFloat;
    
    public Transform groundCheckPoint;
    public float maxAirSpeed;
    public float maxRunSpeed;
    public float minSpeed;
    public float groundDrag;
    public float airDrag;

    public bool isMoving = false;
    public bool hooked;

    public GrappleManager grappleManager;
    public Rigidbody2D rb;

    private GameInput1 controls;

    public LayerMask groundLayer;
    public float groundCheckRadius = 2f;
    public Vector2 groundCheckOffset = new Vector2(0, -0.5f);

    private Vector2 storedDirection = Vector2.zero;
    private bool activeJump = false;
    private bool slideRequested = false;
    private float jumpHoldTimer = 0f;

    private void Awake()
    {
        controls = new GameInput1();
    }

    private void Start()
    {
    }

    private void Update()
    {
        hooked = grappleManager.hooked;
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Movement.performed += ActiveMove;
        controls.Player.Movement.canceled += DeActiveMove;

        controls.Player.JHook.performed += ActiveJump;
        controls.Player.JHook.canceled += DeActiveJump;

        controls.Player.Slide.performed += StartSlide;
    }

    private void OnDisable()
    {
        controls.Disable();
        controls.Player.Movement.performed -= ActiveMove;
        controls.Player.Movement.canceled -= DeActiveMove;

        controls.Player.Slide.performed -= StartSlide;
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    public bool isGrounded()
    {
        // Check if player is within ground detection radius and not falling down rapidly
        bool isTouchingGround = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        bool isFalling = rb.velocity.y < -0.1f;  // Adjust threshold as needed

        // Only return true if actually grounded, and not just close to the ground
        bool grounded = isTouchingGround && !isFalling;
        //Debug.Log("Is Grounded: " + grounded + " | Is Touching Ground: " + isTouchingGround + " | Is Falling: " + isFalling);

        return grounded;
    }


    public void ActiveMove(InputAction.CallbackContext value)
    {
        storedDirection = value.ReadValue<Vector2>();
        //Debug.Log("applied storedDirection: " + storedDirection);
    }
    
    public void DeActiveMove(InputAction.CallbackContext value)
    {
        storedDirection = Vector2.zero;
    }

    public void ActiveJump(InputAction.CallbackContext context)
    {
        if (isGrounded() || isSliding)
        {
            activeJump = true;
            jumpHoldTimer = 0f;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Initial jump force
            isSliding = false; // Stop sliding if player jumps
            //Debug.Log("Jump initiated. Sliding stopped.");
        }
    }

    public void DeActiveJump(InputAction.CallbackContext context)
    {
        activeJump = false;
    }

    public void StartSlide(InputAction.CallbackContext context)
    {
        if (isGrounded())
        {
            slideRequested = true;
            slideTimer = slideDuration; // Start the slide timer
            isSliding = true;
            Debug.Log("Slide started. Slide timer: " + slideTimer);
        }
    }

    private void HandleMovement()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (isGrounded())
        {
            HandleGroundedMovement();
            HandleJump();
        }
        else if (!hooked)
        {
            HandleAirborneMovement();
        }
    }

    private void HandleGroundedMovement()
    {
        if (isSliding)
        {
            slideTimer -= Time.fixedDeltaTime;

            if (slideTimer <= 0)
            {
                isSliding = false; // End slide if the duration has passed
                Debug.Log("Slide ended due to timer expiry.");
            }
        }

        if (isSliding && Mathf.Abs(rb.velocity.x) > maxRunSpeed)
        {
            // Maintain current speed during slide
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
            Debug.Log("Sliding with retained speed: " + rb.velocity.x);
        }
        else if (Mathf.Abs(storedDirection.x) > 0.1f)
        {
            rb.velocity = new Vector2(rb.velocity.x + (accelerationSpeed * storedDirection.x), rb.velocity.y);
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxRunSpeed, maxRunSpeed), rb.velocity.y);
        }
        else
        {
            // Apply drag or stop movement if below threshold
            if (Mathf.Abs(rb.velocity.x) < minSpeed)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x - (groundDrag * Mathf.Sign(rb.velocity.x)), rb.velocity.y);
            }
        }
    }

    private void HandleAirborneMovement()
    {
        // Apply horizontal acceleration based on input without clamping
        rb.velocity = new Vector2(rb.velocity.x + (airSpeed * storedDirection.x), rb.velocity.y);

        // Limit horizontal velocity only by maxAirSpeed while in the air
        float clampedXVelocity = Mathf.Clamp(rb.velocity.x, -maxAirSpeed, maxAirSpeed);
        rb.velocity = new Vector2(clampedXVelocity, rb.velocity.y);

        // Apply air drag only if there’s no input, and if not hooked for smoother airborne control

        if (Mathf.Abs(storedDirection.x) < 0.1f)
        {
            rb.velocity = new Vector2(rb.velocity.x - (airDrag * Mathf.Sign(rb.velocity.x)), rb.velocity.y);
        }

    }

    private void HandleJump()
    {
        if (activeJump && jumpHoldTimer < maxJumpHoldTime)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + jumpHoldForce * Time.fixedDeltaTime);
            jumpHoldTimer += Time.fixedDeltaTime;
            //Debug.Log("Jump held. Hold timer: " + jumpHoldTimer);
        }
    }
}
