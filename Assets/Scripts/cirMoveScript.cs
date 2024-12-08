using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirMoveScript : MonoBehaviour
{
    public GameObject playerObj;
    private NewBetterHook hookList;

    // Radii for x and y dimensions
    public float xRadius = 5f;
    public float yRadius = 3f;

    // Speed of the circular motion
    public float speed = 1f;

    // Movement control flags
    public bool movX = true;
    public bool movY = true;
    public bool stopHooked = false;

    public bool movHooked = false;

    public bool debugMode =false;

    // Internal time tracker
    private float time = 0f;

    private bool isHook;

    // Object's initial position
    private Vector2 initialPosition;

    // LineRenderer for drawing the trail
    private LineRenderer lineRenderer;
    private List<Vector3> trailPositions = new List<Vector3>();

    void Start()
    {
        // Store the initial position
        initialPosition = transform.position;

        // Initialize LineRenderer
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Set LineRenderer properties
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0;

        // Hook list setup
        //hookList = playerObj.GetComponent<NewBetterHook>();
    }

    void FixedUpdate()
    {
        hookList = playerObj.GetComponent<GrappleController>().grappleHook;
        
        // Increment time based on FixedDeltaTime
        if (stopHooked && !isHook)
        {
            time += Time.fixedDeltaTime * speed;
        }else if (movHooked && isHook){
            time += Time.fixedDeltaTime * speed;
        }

        if (!stopHooked && ! movHooked)
        {
            time += Time.fixedDeltaTime * speed;
        }

        // Calculate new position based on enabled axes
        float x = movX ? xRadius * Mathf.Cos(time) : 0f;
        float y = movY ? yRadius * Mathf.Sin(time) : 0f;

        // Check hook conditions
        if (stopHooked)
        {
            if (hookList.GetTail() != null && hookList.GetTail().GetObj() != null && hookList.GetTail().GetObj().Equals(this.gameObject))
            {
                // Stop moving if hooked to the current object
                isHook = true;
                return;
            }
            else
            {
                isHook = false;
            }
        }else if ( movHooked){
            if (hookList.GetTail() != null && hookList.GetTail().GetObj() != null && hookList.GetTail().GetObj().Equals(this.gameObject))
            {
                // Stop moving if hooked to the current object
                isHook = true;
                
            }
            else
            {
                isHook = false;
                return;
            }

        }

        // Update position
        Vector2 newPosition = initialPosition + new Vector2(x, y);
        transform.position = newPosition;

        if (debugMode){
            // Add the current position to the trail list
            trailPositions.Add(newPosition);

            // Update the LineRenderer's trail
            lineRenderer.positionCount = trailPositions.Count;
            lineRenderer.SetPositions(trailPositions.ToArray());
        }

        
    }
}
