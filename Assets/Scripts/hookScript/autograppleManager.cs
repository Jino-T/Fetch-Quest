using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using System;

public class AutoGrappleManager : MonoBehaviour
{
    public float hookDrag;
    public int hookType;
    //public bool hooked;

    //public bool canHook;
    public bool mouseAct = false;

    //public float distHook;

    public float widthHook;

    public GameObject jointPrefab;

    private Vector2 storedDirection;
    public LayerMask hooks;
     //public LayerMask ground;

    private Vector2 hookcastDir;

    private Rigidbody2D rb;
    private GameInput1 controls;
    public Movescript playerMoveState; // Updated from Movescript to PlayerMovement

    private playerStateScript playerState;
    public GameObject playerObject;

    private Vector2 hookedPoz;

    private GameObject hookObj;

    public float boxCastDuration = 3f; // Duration of the BoxCast
    private float boxCastTimer = 0f; // Timer to track the BoxCast duration
    private bool isBoxCasting = false; // Flag to track whether BoxCast is active

    // LineRenderer to visualize the BoxCast
    public LineRenderer lineRenderer;


    private void Start()
    {
        playerMoveState =this.GetComponent<Movescript>();

        rb = playerObject.GetComponent<Rigidbody2D>();

        playerState = this.GetComponent<playerStateScript>();

        
        //playerObject.GetComponent<softProto>().mouseAct = mouseAct;

        //playerObject.GetComponent<GrappleController>().CreateGrappleHook( jointPrefab);

        // Set up the LineRenderer
        lineRenderer = this.GetComponent<LineRenderer>();
        //lineRenderer.positionCount = 5; // 4 points for the box corners, and 1 to close the loop
        //lineRenderer.startWidth = widthHook;
        //lineRenderer.endWidth = widthHook;
        //lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        //lineRenderer.startColor = Color.red;
        //lineRenderer.endColor = Color.red;
    }

        private void OnEnable()
    {
        controls.Enable();
        controls.Player.Movement.performed += InputCheak; 
        controls.Player.Movement.canceled += DeInputCheak;

        
    }

    private void OnDisable()
    {
        controls.Disable();
        controls.Player.Movement.performed -= InputCheak; 
        controls.Player.Movement.canceled -= DeInputCheak;
    }

    

    private void FixedUpdate()
    {
        
        if (playerState.conSidedGround && !playerState.isGrappleHook ){
            StopBoxCast();
            //playerState.canGrappleHook = true;
        }


        
        
        //lineRenderer = playerObject.AddComponent<LineRenderer>();
        //playerMoveState.isGrounded();
        //isGrounded() = GrappleManager
        // Update BoxCast timer if active
        if (isBoxCasting )
        {
            
            

            if ( ! playerState.isGrappleHook ){

                
                boxCastTimer += Time.deltaTime;
                if (boxCastTimer >= boxCastDuration)
                {
                    isBoxCasting = false; // Stop casting after duration
                }

                Collider2D hit = PerformBoxCast();


                if (hit != null ){
                    DrawBoxCast();
                    lineRenderer.enabled= true;
                    playerState.canGrappleHook = false;
                    Debug.Log(hit.gameObject.name);
                    if ( hit.gameObject.GetComponent<Rigidbody2D>() == null){
                        Rigidbody2D collidRigd = hit.gameObject.AddComponent<Rigidbody2D>();
                        collidRigd.freezeRotation= true;
                        collidRigd.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
                        
                    }
                    lineRenderer.SetPosition(1, hit.gameObject.GetComponent<Rigidbody2D>().transform.position);
                
                    GameObject hitPosition = hit.gameObject;
                    hookedPoz = hit.gameObject.GetComponent<Rigidbody2D>().transform.position;
                    hookObj = hit.gameObject;
                    actHook(hitPosition);
                    playerState.isGrappleHook =true;
                }
                //DrawBoxCast();  

            }else{
                isBoxCasting = false;
            }
        }

        if ( playerState.isGrappleHook){
            DrawBoxCast();
            lineRenderer.enabled= true;
        }
    }

    private void Awake()
    {
        controls = new GameInput1();
        controls.Player.Jump.performed += _ => Hook();
    }

    private void InputCheak(InputAction.CallbackContext value)
    {
        storedDirection = value.ReadValue<Vector2>();
        hookcastDir = storedDirection.normalized;
    }

    private void DeInputCheak(InputAction.CallbackContext value){
         
         storedDirection = Vector2.zero;
    }




    private void Hook()
    { //Debug.Log("hooking");
        if (playerState.isGrappleHook)
        {
            playerObject.GetComponent<GrappleController>().DeactivateHookConnection();
            playerState.isGrappleHook =  false;
        }
        else if (!playerState.isGrappleHook && !playerState.conSidedGround && !isBoxCasting && playerState.canGrappleHook )
        {
            // Start the BoxCast for 3 seconds
            hookcastDir = storedDirection.normalized;
            //playerState.canGrappleHook = false;
            StartBoxCast();
            
            
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
    private Collider2D PerformBoxCast()
    {
        lineRenderer.startWidth =1f;
        lineRenderer.endWidth =1f;
        //Debug.Log("dik");
        // Define the width and height of the box for the BoxCast
        //float width = 2f; // Set as needed for the width of the box
        //float height = 1f; // Set as needed for the height of the box

        // Perform the BoxCast using stored direction and size
        Collider2D[] colliders = Physics2D.OverlapCircleAll(rb.position, widthHook, hooks );

        Collider2D closestCollider = null;
        float closestDistance = Mathf.Infinity;
        
        foreach (Collider2D collider in colliders)
        {
            float distance = Vector2.Distance(rb.position, collider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCollider = collider;
            }
        }
        return closestCollider;
    }

    private void actHook(GameObject endPos)
    {
        //Debug.Log("Using Soft Proto Hook");
        
        
        playerObject.GetComponent<GrappleController>().ActivateHookConnection(endPos);
    }

    // Visualize the BoxCast with a LineRenderer
    private void DrawBoxCast()
    {
        lineRenderer.SetPosition(0, rb.position);
        if (playerState.isGrappleHook){
            //lineRenderer.SetPosition(1, hookedPoz);
            
            lineRenderer.SetPosition(1, hookObj.transform.position);
        }else{
            //lineRenderer.SetPosition(1, transform.position + (Vector3)(hookcastDir.normalized * (widthHook+1.35f)));

        }

        


    // Define the width and height of the box for the BoxCast
    /*
    float width = widthHook;
    float height = 1f;

    // Get the corners of the BoxCast rectangle
    Vector2 boxCenter = rb.position;


    Vector2[] corners = new Vector2[4];

    corners[0] = boxCenter + (storedDirection  * new Vector2(width / 2, height));
    corners[1] = boxCenter + (storedDirection  * new Vector2(-width / 2, height ));
    corners[2] = boxCenter +  (storedDirection  * new Vector2(-width / 2, height ));
    corners[3] = boxCenter + (storedDirection  * new Vector2(-width / 2, height ));

    // Set the positions of the LineRenderer
    lineRenderer.SetPosition(0, corners[0]);
    lineRenderer.SetPosition(1, corners[1]);
    lineRenderer.SetPosition(2, corners[2]);
    lineRenderer.SetPosition(3, corners[3]);
    lineRenderer.SetPosition(4, corners[0]); // Close the loop
    */
    }

}
