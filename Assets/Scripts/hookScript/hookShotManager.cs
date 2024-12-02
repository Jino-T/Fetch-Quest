using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;

public class hookShotManager : MonoBehaviour
{
    //public bool hooked = false;

    //public bool grappledHooked = false;

    //public bool canHook = true;

    public float distHook;

    public float widthHook;

    public float dashBust = 5f;


    private Vector2 storedDirection;

    //private Vector2 movStoredDirection;
    public LayerMask hooks;
     //public LayerMask ground;

    private Vector2 hookcastDir;

    private Vector2 dashDireciton;

    private Rigidbody2D rb;
    private GameInput1 controls;
    public Movescript playerMoveState; // Updated from Movescript to PlayerMovement

    //public AutoGrappleManager autoManager;
    public GameObject playerObject;

    private Vector2 hookedPoz;

    private GameObject hookObj;

    public float hookAcc = 0.2f;

    private float magVelocity;

    public float boxCastDuration = 3f; // Duration of the BoxCast
    private float boxCastTimer = 0f; // Timer to track the BoxCast duration
    public bool isBoxCasting = false; // Flag to track whether BoxCast is active

    private bool isPaused = false;

    public bool hasJumped =false;

    // LineRenderer to visualize the BoxCast
    private LineRenderer lineRenderer;

    private playerStateScript playerState;


    private void Start()
    {

        playerMoveState =this .GetComponent<Movescript>();
        //autoManager = this.GetComponent<AutoGrappleManager>();

        playerState = this.GetComponent<playerStateScript>();

        rb = playerObject.GetComponent<Rigidbody2D>();

        lineRenderer = this.GetComponent<LineRenderer>();
        
        lineRenderer.startWidth =widthHook;
        lineRenderer.endWidth =widthHook;

    
    }

        private void OnEnable()
    {
        controls.Enable();
        controls.Player.HookShot.performed += _ => Hook();

        controls.Player.Movement.performed += ActiveMove;
        controls.Player.Movement.canceled += DeActiveMove;

        controls.Player.Jump.performed += AcitveJump;
        //controls.Player.Jump.canceled += DeActiveJump;



        
    }

    private void OnDisable()
    {
        controls.Disable();
        

        controls.Player.Movement.performed -= ActiveMove; 
        controls.Player.Movement.canceled -= DeActiveMove;

        controls.Player.Jump.performed -= AcitveJump;
        //controls.Player.Jump.canceled -= DeActiveJump;
    }

    

    private void AcitveJump(InputAction.CallbackContext context)
    {
        hasJumped = true;
    }
    private void DeActiveJump(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    private void FixedUpdate()
    {

        //grappledHooked = playerState.isHookShot;
        /*
        if ( storedDirection.magnitude >0.1f){
            Debug.Log("idk it happening");
            Hook();
        }
        */
        
        if (playerState.conSidedGround  ){
            //StopBoxCast();
            //playerState.canHookShot = true;
        }

        if( playerState.canHookShot){
            dashDireciton = hookcastDir;
            hasJumped = false;
            magVelocity  = 0.5f;
        }
 



        if ( playerState.isHookShot){
            
            //Vector2 dashDireciton = hookcastDir;
            //dashDireciton = hookcastDir;

            if ( hookObj.tag == "PlayerToObj"){

                playerState.canHookShot =false;
                Debug.Log(dashDireciton);

                Vector2 direction = ( this.hookObj.transform.position - this.rb.transform.position ).normalized;

                Vector2 dirVec =  direction * magVelocity;

                if ( !hasJumped){

                    if (  Vector2.Distance(this.rb.transform.position,  hookObj.transform.position) >  hookObj.GetComponent<SpriteRenderer>().bounds.size.x){
                        if ( dirVec.magnitude <20){
                            magVelocity = hookAcc * magVelocity;
                            this.rb.velocity= dirVec;
                        }
                        
                    }else if ( Vector2.Distance(this.rb.transform.position,  hookObj.transform.position) < hookObj.GetComponent<SpriteRenderer>().bounds.size.x){
                        this.rb.transform.position =  hookObj.transform.position;
                        playerState.isHookShot = false;
                        //playerState.isHookShot = false;
                        playerState.canGrappleHook = true;
                        
                        
                    }

                }else{
                    playerState.isHookShot = false;
                    playerState.canGrappleHook = true;
                }

            }

            if (hookObj.tag == "ObjToPlayer"){
                //this.rb.transform.position =  hookObj.transform.position;
                Dash( dashDireciton,8f);

            }
            
            
            
             



            /*
            Debug.Log("movingVelo");
            float distance = Vector2.Distance(this.rb.transform.position, hookObj.transform.position);
            this.rb.transform.position =  Vector2.MoveTowards(transform.position, hookObj.transform.position, distance%100);
            this.rb.transform.position = hookObj.transform.position;
            //Hook();
            */
            
        }
        
    }

    private void Awake()
    {
        controls = new GameInput1();
        //controls.Player.JHook.performed += _ => Hook();
    }

       public void DeActiveMove(InputAction.CallbackContext value)
    {
        storedDirection = Vector2.zero;
    }


    public void ActiveMove(InputAction.CallbackContext value)
    {
        // Read the input vector
        //movStoredDirection = value.ReadValue<Vector2>();

        storedDirection = value.ReadValue<Vector2>();
        //dashDireciton =  storedDirection;
        hookcastDir = storedDirection.normalized;


        /*
        if (isPaused && playerState.isHookShot)
        {
            // Normalize the direction vector to ensure consistent movement speed
            if (storedDirection.magnitude > 0.1f) // Add a threshold to avoid jitter from small input
            {
                Vector2 normalizedDirection = storedDirection.normalized;
                Dash(normalizedDirection, rb.velocity); // Call Dash with normalized direction
                ResumeGame(); // Resume the game after handling input
                
            }
            else
            {
                Debug.Log("No significant input detected.");
            }
        } //Debug.Log("applied storedDirection: " + storedDirection);
        */
        
    }





    private void Hook()
    { 
        Debug.Log("hooking, "+ playerState.canHookShot);
        

        if ( !playerState.isHookShot  && !playerState.isGrappleHook && playerState.canHookShot){

            RaycastHit2D hit = PerformBoxCast(hookcastDir);

            //DrawBoxCast(hookcastDir);


            if (hit.collider != null ){

                Debug.Log(hit.collider.gameObject.name);
                if ( hit.collider.gameObject.GetComponent<Rigidbody2D>() == null){
                    Rigidbody2D collidRigd = hit.collider.gameObject.AddComponent<Rigidbody2D>();
                    collidRigd.freezeRotation= true;
                    collidRigd.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
                    
                }
                lineRenderer.SetPosition(1, hit.collider.gameObject.GetComponent<Rigidbody2D>().transform.position);
            
                //GameObject hitPosition = hit.collider.gameObject;
                hookedPoz = hit.collider.gameObject.GetComponent<Rigidbody2D>().transform.position;
                hookObj = hit.collider.gameObject;
                //actHook(hitPosition);
                playerState.isHookShot =true;
                playerState.canHookShot = false;
            }

            

        }



        Debug.Log("isBoxCaseing, "+ playerState.canHookShot);
        // Start the BoxCast for 3 seconds
        hookcastDir = storedDirection.normalized;
        //canHook = false;
        boxCastTimer = 0f ; // Reset timer
        isBoxCasting = true; 

        lineRenderer.enabled = false;

        
        
        
    }

    // Start the BoxCast duration timer
    private void StartBoxCast()
    {
        boxCastTimer = 0f ; // Reset timer
        isBoxCasting = true; // Start casting
        //Debug.Log("somthings");
    }

    private void StopBoxCast()
    {
        boxCastTimer = 0f ; // Reset timer
        isBoxCasting = false; // Start casting
        //Debug.Log("somthings");
    }


    // Perform a BoxCast and return the hit result
    private RaycastHit2D PerformBoxCast(Vector2 inputDirection)
    {
        DrawBoxCast(inputDirection);
        // Perform the BoxCast using stored direction and size
        RaycastHit2D hit = Physics2D.BoxCast(rb.position, new Vector2(widthHook, widthHook), 0f, inputDirection,distHook, hooks);
        return hit;
    }


    // Visualize the BoxCast with a LineRenderer
    private void DrawBoxCast(Vector2 dirHook)
    {
        lineRenderer.enabled = true;

        lineRenderer.startWidth =widthHook;
        lineRenderer.endWidth =widthHook;

        lineRenderer.SetPosition(0, rb.position);
        if (playerState.isHookShot){
            //lineRenderer.SetPosition(1, hookedPoz);
            
            lineRenderer.SetPosition(1, hookObj.transform.position);
        }else{
            lineRenderer.SetPosition(1, transform.position + (Vector3)(dirHook.normalized * (distHook+1.35f)));

        }
    }
    void PauseGame()
    {
        Time.timeScale = 0f;
        AudioListener.pause = true;
        isPaused = true;
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        isPaused = false;
    }

    private void Dash(Vector2 direction, float dashBoost)
    {
        // Pause the game and wait for input
        //rb.velocity = Vector2.zero;

        //ResumeGame();
        


        // ResumeGame is called from InputCheck once input is detected
        rb.velocity = rb.velocity + direction.normalized * dashBoost ;
        //direction.normalized * dashBust + playerObject.

        playerState.isHookShot = false;

        //playerState.isHookShot = false;
        playerState.canGrappleHook = true;

    }

}
