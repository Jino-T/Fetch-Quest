using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class Movescript : MonoBehaviour
{
    public float initialAcceleration;
    public float accelerationSpeed;
    public float airSpeed;

    //public float logMultiplier;
    public float jumpForce;
    //public float maxJumpForce;
    public float jumpHoldForceScale;
    public float maxJumpHoldTime;

    public float slideDuration = 0.3f; // Duration for which player retains air speed on ground while sliding
    private float slideTimer = 0f;
    private bool isSliding = false;

    //public float jumpFloat;
    
    public GameObject groundCheckPoint;
    public float maxAirSpeed;
    public float maxRunSpeed;
    public float minSpeed;
    public float groundDrag;
    public float airDrag;

    //public bool isMoving = false;
    public bool hooked;

    public GrappleManager grappleManager;
    public Rigidbody2D rb;

    private GameInput1 controls;

    /// <summary>
    //public LayerMask groundLayer;
    /// </summary>
    //public float groundCheckRadius = 2f;
    //public Vector2 groundCheckOffset = new Vector2(0, -0.5f);

    private Vector2 storedDirection = Vector2.zero;

    public bool isJumping = false;
    public bool holdJump = false;
    private bool pushingJump = false;
    private bool slideRequested = false;
    private float jumpHoldTimer = 0f;

    public string killTag;   
    public LayerMask killLayer;

    private Timer jumpBuffTimer;
    public float timeBuffTime;


    // cytotec time

    public int cytotecFames;
    private int currCFames =0;
    public bool conSidedGround = false;
    


    private void Awake()
    {
        controls = new GameInput1();
    }

    private void Start()
    {
        grappleManager = this.GetComponent<GrappleManager>();
        jumpBuffTimer = this.AddComponent<Timer>();
        jumpBuffTimer.Initialize(true, timeBuffTime );
        
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
    bool grounded = isGrounded();

    // Update jumping state
    if (pushingJump  )
    {
        if (!conSidedGround && isJumping){
            HandleJump();

        }
    }else{
        if(grounded ){
            isJumping = false;
            jumpHoldTimer = maxJumpHoldTime;

        }
    }

    if (conSidedGround && !jumpBuffTimer.IsFinished()){
        rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Apply initial jump force
        isJumping =true;
        jumpHoldTimer = 0f;
        isSliding = false;
        jumpBuffTimer.EndTimer();
        holdJump = true;

    }
    

    // Update "considered ground" frames for leniency
    if (grounded)
    {
        conSidedGround = true;
        currCFames = cytotecFames;
    }
    else
    {
        if (currCFames > 0 && conSidedGround && !isJumping)
        {
            currCFames -= 1;
        }
        else
        {
            conSidedGround = false;
        }
    }

    HandleMovement();
}



    public bool isGrounded()
    {
        // Check if player is within ground detection radius and not falling down rapidly
        //bool isTouchingGround = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        //bool isFalling = rb.velocity.y < -0.1f;  // Adjust threshold as needed

        // Only return true if actually grounded, and not just close to the ground
        //bool grounded = isTouchingGround; //&& !isFalling;
        //Debug.Log("Is Grounded: " + grounded + " | Is Touching Ground: " + isTouchingGround + " | Is Falling: " + isFalling);

        return groundCheckPoint.GetComponent<groundCheckScript>().isGrouned;
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
        if (conSidedGround ){
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Apply initial jump force
            isJumping =true;
            jumpHoldTimer = 0f;
            isSliding = false;
            jumpBuffTimer.EndTimer();
            holdJump = true;


        }else{

            jumpBuffTimer.ResetTimer();
            jumpBuffTimer.StartTimer();

        }

        pushingJump = true;
        
    }

    public void DeActiveJump(InputAction.CallbackContext context)
    {
        pushingJump = false;
        holdJump = false;
    }

    public void StartSlide(InputAction.CallbackContext context)
    {
        if (conSidedGround)
        {
            slideRequested = true;
            slideTimer = slideDuration; // Start the slide timer
            isSliding = true;
            Debug.Log("Slide started. Slide timer: " + slideTimer);
        }
    }

    private void HandleMovement()
    {

        if (conSidedGround)
        {
            HandleGroundedMovement();
            //HandleJump();
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
        
        
        if (Mathf.Abs(storedDirection.x) > 0.1f)
        {
            if ( Mathf.Abs(rb.velocity.x) > 0f){
                if (Mathf.Sign(storedDirection.x) == Mathf.Sign(rb.velocity.x) && Mathf.Abs(rb.velocity.x) > maxRunSpeed)
                {
                    // Apply drag to reduce speed
                    rb.velocity = new Vector2(rb.velocity.x - (groundDrag * Mathf.Sign(rb.velocity.x)), rb.velocity.y);
                }
                else
                {
                    // Apply acceleration when under maxRunSpeed or changing direction
                    rb.velocity = new Vector2(rb.velocity.x + (accelerationSpeed * storedDirection.x), rb.velocity.y);
                }

            }else{
                rb.velocity = new Vector2(rb.velocity.x + (initialAcceleration * storedDirection.x), rb.velocity.y);
            }
            
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
        // Check if there's input
        if (Mathf.Abs(storedDirection.x) > 0.1f)
        {
            // If input matches velocity direction and velocity exceeds maxAirSpeed, maintain velocity
            if (Mathf.Sign(storedDirection.x) == Mathf.Sign(rb.velocity.x) && Mathf.Abs(rb.velocity.x) > maxAirSpeed)
            {
                // Maintain current velocity, do nothing
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
            }
            else
            {
                // Otherwise, add velocity based on input
                rb.velocity = new Vector2(rb.velocity.x + (airSpeed * storedDirection.x), rb.velocity.y);
            }
        }
        else
        {
            // Apply air drag if there's no input
            rb.velocity = new Vector2(rb.velocity.x - (airDrag * Mathf.Sign(rb.velocity.x)), rb.velocity.y);

            // Prevent overshooting to zero
            if (Mathf.Abs(rb.velocity.x) < airDrag)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
    }


    private void HandleJump()
    {
        if(holdJump && isJumping && rb.velocity.y >0 &&  jumpHoldTimer < maxJumpHoldTime){


            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + jumpHoldForceScale * Time.fixedDeltaTime);
            jumpHoldTimer += Time.fixedDeltaTime;
            //Debug.Log("Jump held. Hold timer: " + jumpHoldTimer);
        }
        
    }

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    private void OnTriggerEnter2D(Collider2D other)
{
    if (killLayerOrTag(other.gameObject))
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

       
    }
    
}

private void OnTriggerStay2D(Collider2D other)
{
    //Debug.Log( other.CompareTag("wall"));
 

   if (  isJumping &&  ((other.CompareTag("ground") && other.transform.position.y >= (this.transform.position.y +  GetSpecificCapsuleCollider(0).bounds.size.y)) || other.CompareTag("wall")) )
    {
        

        // Calculate the difference in positions
        Vector2 diff = other.transform.position - this.transform.position;
        //if ( this.rb.velocity.normalized.x == diff. normalized)

        // Adjust the position slightly in the x-direction
        this.transform.position = new Vector2(((GetSpecificCapsuleCollider(1).bounds.size.x - GetSpecificCapsuleCollider(0).bounds.size.x)/2 ) * diff.normalized.x + this.transform.position.x, this.transform.position.y);
        //Debug.Log("when off");
    }
    
}

public bool killLayerOrTag(GameObject obj)
    {
        if (obj == null) return false;

        
        bool layerMatches = ((1 << obj.layer) & killLayer) != 0;
        bool tagMatches = obj.CompareTag(killTag);

        return layerMatches || tagMatches;
    }

CapsuleCollider2D GetSpecificCapsuleCollider(int index)
{
    CapsuleCollider2D[] colliders = GetComponents<CapsuleCollider2D>();
    if (index >= 0 && index < colliders.Length)
    {
        return colliders[index];
    }
    else
    {
        Debug.LogWarning("Invalid collider index!");
        return null;
    }
}
}
