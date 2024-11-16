using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using System;

public class GrappleManager : MonoBehaviour
{
    public float hookDrag;
    public int hookType;
    public bool hooked;
    public bool mouseAct = false;

    public float distHook;

    public float widthHook;

    public GameObject jointPrefab;

    private Vector2 storedDirection;
    public LayerMask hooks;

    private Rigidbody2D rb;
    private GameInput1 controls;
    public Movescript playerMoveState; // Updated from Movescript to PlayerMovement
    public GameObject playerObject;

    private float boxCastDuration = 3f; // Duration of the BoxCast
    public float boxCastTimer = 0f; // Timer to track the BoxCast duration
    private bool isBoxCasting = false; // Flag to track whether BoxCast is active

    // LineRenderer to visualize the BoxCast
    private LineRenderer lineRenderer;


    private void Start()
    {

        rb = playerObject.GetComponent<Rigidbody2D>();
        //playerObject.GetComponent<softProto>().mouseAct = mouseAct;

        //playerObject.GetComponent<GrappleController>().CreateGrappleHook( jointPrefab);

        // Set up the LineRenderer
        lineRenderer = playerObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 5; // 4 points for the box corners, and 1 to close the loop
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
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

    

    private void Update()
    {
        //lineRenderer = playerObject.AddComponent<LineRenderer>();
        //playerMoveState.isGrounded();
        //isGrounded() = GrappleManager
        // Update BoxCast timer if active
        if (isBoxCasting)
        {
            boxCastTimer += Time.deltaTime;
            if (boxCastTimer >= boxCastDuration)
            {
                isBoxCasting = false; // Stop casting after duration
            }

            // Draw the BoxCast visualization
            DrawBoxCast();
        }
    }

    private void Awake()
    {
        controls = new GameInput1();
        controls.Player.JHook.performed += _ => Hook();
    }

    private void InputCheak(InputAction.CallbackContext value)
    {
        storedDirection = value.ReadValue<Vector2>();
    }

    private void DeInputCheak(InputAction.CallbackContext value){
         
         storedDirection = Vector2.zero;
    }




    private void Hook()
    { //Debug.Log("hooking");
        if (hooked)
        {
            playerObject.GetComponent<GrappleController>().DeactivateHookConnection();
            hooked =  false;
        }
        else if (!hooked && !playerMoveState.isGrounded())
        {
            // Start the BoxCast for 3 seconds
            StartBoxCast();
            RaycastHit2D hit = PerformBoxCast();

            // Perform BoxCast after 3 seconds
            if (isBoxCasting && hit.collider == null)
            {
                // Debug.DrawRay(rb.position, storedDirection * Mathf.Infinity, Color.red, 2f);

                hit = PerformBoxCast();
                if ( hit.collider != null){
                    Debug.Log(hit.collider.gameObject.name);
                }else{
                    Debug.Log("miss");
                }
                
                 

                
            }

            if (hit.collider != null)
                {
                    Debug.Log(hit.collider.gameObject.name);
                    GameObject hitPosition = hit.collider.gameObject;
                   
                    actHook(hitPosition);
                    hooked =true;
                }
        }
    }

    // Start the BoxCast duration timer
    private void StartBoxCast()
    {
        boxCastTimer = 0f; // Reset timer
        isBoxCasting = true; // Start casting
        //Debug.Log("somthings");
    }

    // Perform a BoxCast and return the hit result
    private RaycastHit2D PerformBoxCast()
    {
        //Debug.Log("dik");
        // Define the width and height of the box for the BoxCast
        //float width = 2f; // Set as needed for the width of the box
        //float height = 1f; // Set as needed for the height of the box

        // Perform the BoxCast using stored direction and size
        RaycastHit2D hit = Physics2D.BoxCast(rb.position, new Vector2(widthHook, widthHook), 0f, storedDirection,distHook, hooks);
        return hit;
    }

    private void actHook(GameObject endPos)
    {
        Debug.Log("Using Soft Proto Hook");
        
        
        playerObject.GetComponent<GrappleController>().ActivateHookConnection(endPos);
    }

    // Visualize the BoxCast with a LineRenderer
    private void DrawBoxCast()
{
    if (lineRenderer == null)
    {
        lineRenderer = playerObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 5; // 4 points for the box corners, and 1 to close the loop
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
       
        
    }

    if (rb == null)
    {
        Debug.LogError("Rigidbody2D is not assigned!");
        return;
    }

    // Define the width and height of the box for the BoxCast
    float width = 2f;
    float height = 1f;

    // Get the corners of the BoxCast rectangle
    Vector2 boxCenter = rb.position;
    Vector2[] corners = new Vector2[4];

    corners[0] = boxCenter + storedDirection * 4f + new Vector2(width / 2, height / 2);
    corners[1] = boxCenter + storedDirection * 4f + new Vector2(-width / 2, height / 2);
    corners[2] = boxCenter + storedDirection * 4f + new Vector2(-width / 2, -height / 2);
    corners[3] = boxCenter + storedDirection * 4f + new Vector2(width / 2, -height / 2);

    // Set the positions of the LineRenderer
    lineRenderer.SetPosition(0, corners[0]);
    lineRenderer.SetPosition(1, corners[1]);
    lineRenderer.SetPosition(2, corners[2]);
    lineRenderer.SetPosition(3, corners[3]);
    lineRenderer.SetPosition(4, corners[0]); // Close the loop
}

}
