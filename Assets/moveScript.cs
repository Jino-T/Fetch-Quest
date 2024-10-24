using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;      // Movement speed of the player
    public float jumpForce;      // Jump force

    public float topSpeed;
    public Transform groundCheck;     // Position to check if player is grounded
    public LayerMask groundLayer;     // Define what is considered "ground"
    
    private Rigidbody2D rb;
    public bool isGrounded;
    private float fallSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
{
    isGrounded = Physics2D.OverlapCircle(new Vector3(this.transform.position.x, this.transform.position.y, 0), 0.5f, groundLayer);
    /*
    if (isGrounded && Input.GetKeyDown(KeyCode.S))
    {
        // Reset fallSpeed when grounded
        Debug.Log(rb.velocity.x);
        rb.velocity = new Vector2(rb.velocity.x + ( (Mathf.Sign(rb.velocity.x) *-1) * fallSpeed), 0);
        fallSpeed = 0;
    }
    else
    {
        // Update fallSpeed if not grounded
        fallSpeed = rb.velocity.y;
        
        // Adjust horizontal velocity based on current movement direction and fallSpeed
        //float moveInput = Input.GetAxis("Horizontal");
        
    }
    */
}

    void Update()
    {
        
        if (isGrounded){
            Move();
            Jump();
        }

        
        if (Input.GetKeyDown(KeyCode.R)){
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        

    }

    void Move()
    {
       
            //float moveInput = Input.GetAxis("Horizontal"); // Get input from the horizontal axis (A/D or Left/Right keys)
            float moveInput = Input.GetAxis("Horizontal"); // Get input from the horizontal axis (A/D or Left/Right keys)
            if (rb.velocity.x <  topSpeed){
            //rb.velocity = new Vector2( 10f, rb.velocity.y);

            rb.velocity =  new Vector2 (rb.velocity.x + (moveSpeed * Time.deltaTime * moveInput), rb.velocity.y );
            }

        
    }

    void Jump()
    {
        //isGrounded = Physics2D.OverlapCircle(new Vector3( this.transform.position.x , this.transform.position.y, this.transform.position.z), 1.001f, groundLayer); // Check if grounded

        if ( Input.GetKeyDown(KeyCode.Space))  // Check for jump input and grounded status
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Apply jump force
        }
    }
    /*
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == groundLayer){
            isGrounded = true;
        }
    }
    private void OnTriggerStay2D(Collider2D other) {
         if (other.gameObject.layer == groundLayer){
            isGrounded = true;
        }else{
            isGrounded = false;
        }
        
    }
    */
}