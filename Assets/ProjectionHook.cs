using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionHook : MonoBehaviour
{
    public Camera mainCamera;
    public LineRenderer _lineRenderer;
    public DistanceJoint2D _distanceJoint;
    public PlayerMovement player;
    public LayerMask groundLayer;
    public float scalingFactor;
    private Vector2 mousePos;

    // Start is called before the first frame update
    void Start()
    {
        _distanceJoint.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        bool over = CheckLayerOverlap(transform.position, mousePos, 6);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            mousePos = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);

            if (!player.isGrounded && !over)
            {
                _lineRenderer.SetPosition(0, mousePos);
                _lineRenderer.SetPosition(1, transform.position);
                _distanceJoint.connectedAnchor = mousePos;
                _distanceJoint.enabled = true;
                _lineRenderer.enabled = true;
                RedirectVelocityAlongCircle(mousePos, this.gameObject);
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0) || player.isGrounded || over)
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

        // Find tangent directions (clockwise and counterclockwise)
        Vector2 tangentClockwise = new Vector2(directionToCenter.y, -directionToCenter.x).normalized;
        Vector2 tangentCounterclockwise = new Vector2(-directionToCenter.y, directionToCenter.x).normalized;

        // Determine which direction player is moving
        float dotClockwise = Vector2.Dot(playerVelocity.normalized, tangentClockwise);
        float dotCounterclockwise = Vector2.Dot(playerVelocity.normalized, tangentCounterclockwise);
        bool isClockwise = dotClockwise > dotCounterclockwise;

        // Redirect velocity along the tangent
        Vector2 newVelocity;
        if (isClockwise)
        {
            newVelocity = tangentClockwise * playerVelocity.magnitude;
        }
        else
        {
            newVelocity = tangentCounterclockwise * playerVelocity.magnitude;
        }

        // Modify momentum using projection: Project current velocity onto the new tangent and scale
        newVelocity = ModifyMomentum(playerRigidbody, newVelocity, 1 + scalingFactor);  // You can adjust the scaling factor as needed

        // Apply the new velocity to the player
        playerRigidbody.velocity = newVelocity;
    }

    // Function to modify momentum using projection
    Vector2 ModifyMomentum(Rigidbody2D playerRigidbody, Vector2 targetVelocity, float scalingFactor)
    {
        // Get the player's current velocity
        Vector2 currentVelocity = playerRigidbody.velocity;

        // Normalize the target velocity direction (the new tangent)
        Vector2 normalizedTargetDir = targetVelocity.normalized;

        // Project the current velocity onto the target velocity direction
        Vector2 projectedVelocity = Vector2.Dot(currentVelocity, normalizedTargetDir) * normalizedTargetDir;

        // Scale the projection to add or subtract momentum (depending on the scaling factor)
        projectedVelocity *= scalingFactor;

        // Return the updated velocity by adding the modified projection to the original velocity
        return currentVelocity + projectedVelocity;
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
