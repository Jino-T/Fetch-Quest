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

public class hookShotManager : MonoBehaviour
{
    public bool hooked = false;

    public bool grappledHooked = false;

    public bool canHook = true;

    public float distHook;

    public float widthHook;

    public float dashBust = 5f;


    private Vector2 storedDirection;

    private Vector2 movStoredDirection;
    public LayerMask hooks;
     //public LayerMask ground;

    private Vector2 hookcastDir;

    private Rigidbody2D rb;
    private GameInput1 controls;
    public Movescript playerMoveState; // Updated from Movescript to PlayerMovement

    public AutoGrappleManager autoManager;
    public GameObject playerObject;

    private Vector2 hookedPoz;

    private GameObject hookObj;

    public float hookSpeed = 10f;

    public float boxCastDuration = 3f; // Duration of the BoxCast
    private float boxCastTimer = 0f; // Timer to track the BoxCast duration
    public bool isBoxCasting = false; // Flag to track whether BoxCast is active

    private bool isPaused = false;

    // LineRenderer to visualize the BoxCast
    public LineRenderer lineRenderer;


    private void Start()
    {

        playerMoveState =this .GetComponent<Movescript>();
        autoManager = this.GetComponent<AutoGrappleManager>();

        rb = playerObject.GetComponent<Rigidbody2D>();

    
    }

        private void OnEnable()
    {
        controls.Enable();
        controls.Player.HookShot.performed += InputCheak; 
        controls.Player.HookShot.canceled += DeInputCheak;

        controls.Player.Movement.performed += ActiveMove;
        controls.Player.Movement.canceled += DeActiveMove;

        
    }

    private void OnDisable()
    {
        controls.Disable();
        controls.Player.HookShot.performed -= InputCheak; 
        controls.Player.HookShot.canceled -= DeInputCheak;
    }

    

    private void FixedUpdate()
    {

        grappledHooked = autoManager.hooked;

        if ( storedDirection.magnitude >0.1f){
            Debug.Log("idk it happening");
            Hook();
        }
        
        if (playerMoveState.conSidedGround  ){
            StopBoxCast();
            canHook = true;
        }
 



        if ( hooked){
            canHook =false;
            Debug.Log("movingVelo");
            float distance = Vector2.Distance(this.rb.transform.position, hookObj.transform.position);
            this.rb.transform.position =  Vector2.MoveTowards(transform.position, hookObj.transform.position, distance%100);
            this.rb.transform.position = hookObj.transform.position;

            // Check if the object is close enough to the target
            
            if (distance <= 0.1f) // Use a small threshold, e.g., 0.1 units
            {
                PauseGame();

            }
        }
        
    }

    private void Awake()
    {
        controls = new GameInput1();
        //controls.Player.JHook.performed += _ => Hook();
    }

    private void InputCheak(InputAction.CallbackContext value)
    {
        lineRenderer.enabled = true;
        Debug.Log(value.ReadValue<Vector2>());
        storedDirection = value.ReadValue<Vector2>();
        hookcastDir = storedDirection.normalized;

         if ( !hooked  && ! grappledHooked && canHook){

                RaycastHit2D hit = PerformBoxCast(hookcastDir);


                if (hit.collider != null ){
                    Debug.Log(hit.collider.gameObject.name);
                    if ( hit.collider.gameObject.GetComponent<Rigidbody2D>() == null){
                        Rigidbody2D collidRigd = hit.collider.gameObject.AddComponent<Rigidbody2D>();
                        collidRigd.freezeRotation= true;
                        collidRigd.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
                        
                    }
                    lineRenderer.SetPosition(1, hit.collider.gameObject.GetComponent<Rigidbody2D>().transform.position);
                
                    GameObject hitPosition = hit.collider.gameObject;
                    hookedPoz = hit.collider.gameObject.GetComponent<Rigidbody2D>().transform.position;
                    hookObj = hit.collider.gameObject;
                    //actHook(hitPosition);
                    hooked =true;
                    canHook = false;
                }
                DrawBoxCast(value.ReadValue<Vector2>());  

            }

        
    }

    private void DeInputCheak(InputAction.CallbackContext value){
        storedDirection =  Vector2.zero;
        lineRenderer.enabled = false;
        
         
    }

       public void DeActiveMove(InputAction.CallbackContext value)
    {
        movStoredDirection = Vector2.zero;
    }


    public void ActiveMove(InputAction.CallbackContext value)
    {
    // Read the input vector
    movStoredDirection = value.ReadValue<Vector2>();

    if (isPaused)
    {
        // Normalize the direction vector to ensure consistent movement speed
        if (movStoredDirection.magnitude > 0.1f) // Add a threshold to avoid jitter from small input
        {
            Vector2 normalizedDirection = movStoredDirection.normalized;
            Dash(normalizedDirection, rb.velocity); // Call Dash with normalized direction
            ResumeGame(); // Resume the game after handling input
            canHook = true;
        }
        else
        {
            Debug.Log("No significant input detected.");
        }
    } //Debug.Log("applied storedDirection: " + storedDirection);
    }



    private void Hook()
    { 
        Debug.Log("hooking, "+ canHook);

        if (!hooked && !isBoxCasting && canHook )
        {
            Debug.Log("isBoxCaseing, "+ canHook);
            // Start the BoxCast for 3 seconds
            hookcastDir = storedDirection.normalized;
            //canHook = false;
            boxCastTimer = 0f ; // Reset timer
            isBoxCasting = true; 

            
            
        }
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
        lineRenderer.startWidth =widthHook;
        lineRenderer.endWidth =widthHook;
        // Perform the BoxCast using stored direction and size
        RaycastHit2D hit = Physics2D.BoxCast(rb.position, new Vector2(widthHook, widthHook), 0f, inputDirection,distHook, hooks);
        return hit;
    }


    // Visualize the BoxCast with a LineRenderer
    private void DrawBoxCast(Vector2 dirHook)
    {
        lineRenderer.SetPosition(0, rb.position);
        if (hooked){
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

    private void Dash(Vector2 direction, Vector2 storeVelcoiy)
    {
        // Pause the game and wait for input
        rb.velocity = Vector2.zero;

        ResumeGame();
        hooked = false;

        autoManager.hooked = false;
        autoManager.canHook = true;


        // ResumeGame is called from InputCheck once input is detected
        this.rb.velocity = direction.normalized * (storeVelcoiy.magnitude+ dashBust);

    }

}
