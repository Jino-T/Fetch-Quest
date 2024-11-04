using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Movescript : MonoBehaviour
{
    public float initialAcceleration;
    public float accelerationSpeed;
    public float airSpeed;
    public float jumpForce;

    public float jumpFloat;
    
    public float maxAirSpeed;
    public float maxRunSpeed;
    public float minSpeed;
    public float groundDrag;
    public float airDrag;

    public bool isMoving = false;
    public bool isGrounded = false;

    public bool hooked;

    public GrappleManager grappleManager;

    //private Vector2 storedDirection;
    public Rigidbody2D rb;
    private GameInput controls;

    public LayerMask groundLayer;
    private Vector2 storedDirection = Vector2.zero;
    private bool activeJump = false;

    private void Start()
    {
        
       
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(transform.position, 0.5f, groundLayer);
        hooked = grappleManager.hooked;
        
    }
    private void Awake() {
        controls = new GameInput();
        //controls.Player.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());  
    }

     private void OnEnable()
    {
        controls.Enable();
        controls.Player.Movement.performed += ActvieMove; 
        controls.Player.Movement.canceled += DeActiveMove;

        controls.Player.JHook.performed += ActvieJump; 
        controls.Player.JHook.canceled += DeActiveJump;
    }

    private void OnDisable()
    {
        controls.Disable();
        controls.Player.Movement.performed -= ActvieMove; 
        controls.Player.Movement.canceled -= DeActiveMove;
    }


    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate()
    {
        //Debug.Log(storedDirection);
        isGrounded = Physics2D.OverlapCircle(new Vector3(rb.transform.position.x, rb.transform.position.y, 0), 0.5f, groundLayer);
        HandleMovement();
       
    }

   

    public void ActvieMove(InputAction.CallbackContext value)
    {
        storedDirection = value.ReadValue<Vector2>();
        Debug.Log("applied storedDirection: "+ storedDirection);
        //moveVect = value.ReadValue<Vector2>();
    }
    public void DeActiveMove ( InputAction.CallbackContext value){
        storedDirection = Vector2.zero;
    }

    public void ActvieJump(InputAction.CallbackContext context){
        activeJump = true;
    }

    public void DeActiveJump(InputAction.CallbackContext context){
        activeJump = false;
    }



    private void HandleMovement()
    {
        //Debug.Log("storedDirection: "+ storedDirection);
       if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        
        if ( isGrounded){
            //Debug.Log("grounded");
            if (Mathf.Abs(storedDirection.x)> 0.1f){
                Debug.Log("storedDirection: " + storedDirection);
                if (maxRunSpeed >= Mathf.Abs(rb.velocity.x)){
                    if ( !isMoving){
                    rb.velocity = new Vector2(rb.velocity.x + (initialAcceleration * storedDirection.x), rb.velocity.y);
                    isMoving = true;
                    }else{
                        rb.velocity = new Vector2(rb.velocity.x + (accelerationSpeed * storedDirection.x), rb.velocity.y);
                    }
                }else{
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
                }
                
            }else{
                if ( rb.velocity.x <= minSpeed){
                    rb.velocity = new Vector2 (0f, rb.velocity.y);
                    isMoving =false;
                }else{
                    rb.velocity = new Vector2(rb.velocity.x - (groundDrag * Mathf.Sign(rb.velocity.x)), rb.velocity.y);
                }
                

            }

            if ( activeJump){
                //this might cause problems if add to y velocity on ground then jump
                //rb.velocity = new Vector2(rb.velocity.x , rb.velocity.y + jumpForce);
                rb.velocity = new Vector2(rb.velocity.x ,  jumpForce);
               
            }
            

        }else {
            if (!hooked){
            
                if (Mathf.Abs(storedDirection.x)> 0.1f){
                    if (maxAirSpeed >= Mathf.Abs(rb.velocity.x)){
                        rb.velocity = new Vector2(rb.velocity.x + (airSpeed * storedDirection .x ), rb.velocity.y );
                    }else{
                        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
                    }
                    
                }else{
                    rb.velocity = new Vector2(rb.velocity.x - (airDrag * Mathf.Sign(rb.velocity.x)), rb.velocity.y);

                }

                if ( activeJump && rb.velocity.y < 0){
                    rb.velocity = new Vector2(rb.velocity.x , rb.velocity.y + jumpFloat);
                }
            }
            
        }
        //isGrounded = Physics2D.OverlapCircle(new Vector3(rb.transform.position.x, rb.transform.position.y, 0), 0.5f, groundLayer);
        
    
    }
}
