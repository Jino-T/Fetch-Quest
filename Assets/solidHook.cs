using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class solidHook : MonoBehaviour
{
    public Camera mainCamera;
    public LineRenderer _lineRenderer;
    public DistanceJoint2D _distanceJoint;
    public Movescript player;
    public LayerMask groundLayer;
    
    public bool mouseAct;
    private Vector2 mousePos;

    // Start is called before the first frame update
    void Start()
    {
        _distanceJoint.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        bool over = CheckLayerOverlap(transform.position,mousePos,6 );
        if (mouseAct){
            if (Input.GetKeyDown(KeyCode.Mouse0) ){
            mousePos = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
            
            //bool isGrounded() = Physics2D.OverlapCircle(new Vector3( this.transform.position.x , this.transform.position.y, this.transform.position.z), 1.001f, groundLayer);
            //Debug.Log(player.isGrounded());
            if ( !player.isGrounded() && !over)
            {
                
               handlehook(mousePos) ;
            }
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse0) ||player.isGrounded() || over ){
            dehook();
         }

    

        }
        

    public void handlehook(Vector2 endpoint){
        _lineRenderer.SetPosition(0, endpoint);
        _lineRenderer.SetPosition(1, this.transform.position);
        _distanceJoint.connectedAnchor = endpoint;
        _distanceJoint.enabled = true;
        _lineRenderer.enabled = true;
        RedirectVelocityAlongCircle(endpoint, this.gameObject);

    }
    public void dehook(){
        {
            _distanceJoint.enabled = false;
            _lineRenderer.enabled = false;
        }
        if (_distanceJoint.enabled) 
        {
            _lineRenderer.SetPosition(1, transform.position);
        }

    }


    void RedirectVelocityAlongCircle(Vector2 circleCenter, GameObject playerObject)
    {
        // Get the player's position and velocity
        Vector2 playerPos = playerObject.transform.position;
        Rigidbody2D playerRigidbody = playerObject.GetComponent<Rigidbody2D>();
        Vector2 playerVelocity = playerRigidbody.velocity;

        // Calculate direction to center and radius
        Vector2 directionToCenter = playerPos - circleCenter;
        float radius = directionToCenter.magnitude;

        // Find tangent directions
        Vector2 tangentClockwise = new Vector2(directionToCenter.y, -directionToCenter.x).normalized;
        Vector2 tangentCounterclockwise = new Vector2(-directionToCenter.y, directionToCenter.x).normalized;

        // Determine which direction player is moving
        float dotClockwise = Vector2.Dot(playerVelocity.normalized, tangentClockwise);
        float dotCounterclockwise = Vector2.Dot(playerVelocity.normalized, tangentCounterclockwise);
        bool isClockwise = dotClockwise > dotCounterclockwise;

        // Redirect velocity along the tangent
        if (isClockwise)
        {
            playerRigidbody.velocity = tangentClockwise * playerVelocity.magnitude;
        }
        else
        {
            playerRigidbody.velocity = tangentCounterclockwise * playerVelocity.magnitude;
        }
    }

    public bool CheckLayerOverlap(Vector2 pointA, Vector2 pointB, int layerIndex)
{
    // Create a LayerMask using the provided layer index
    LayerMask layerMask = 1 << layerIndex;

    // Get the direction and distance between the two points
    Vector2 direction = pointB - pointA;
    float distance = direction.magnitude;

    // Perform a raycast along the line between pointA and pointB using the layerMask
    RaycastHit2D hit = Physics2D.Raycast(pointA, direction, distance, layerMask);

    // Check if the ray hits something on the specified layer
    if (hit.collider != null)
    {
        string layerName = LayerMask.LayerToName(layerIndex); // Get the layer's name from the index
        Debug.Log("Overlap detected with layer: " + layerName);
        return true;
    }

    Debug.Log("No overlap detected.");
    return false;
}

    // Optionally, you can visualize the ray in the Scene view for debugging
    void OnDrawGizmos()
    {
        Vector2 pointA = new Vector2(0, 0);  // Replace with actual points
        Vector2 pointB = new Vector2(1, 1);  // Replace with actual points
        Gizmos.color = Color.green;
        Gizmos.DrawLine(pointA, pointB);
    }

}