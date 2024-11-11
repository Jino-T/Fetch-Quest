using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GrappleController : MonoBehaviour
{
    public GameObject test;
    public GameObject playerObj;
    public GameObject jointPrefab;        // Prefab of the joint GameObject
    public GameObject hookPointPrefab;    // Prefab for the hook point
    public LayerMask collisionLayer;      // Layer to check for collisions
    public float maxJointDistance = 1.0f; // Maximum distance between nodes

    private NewBetterHook grappleHook;    // Instance of the hook class
    public Camera mainCamera;

    public float minDist;

    public float widthCast;

    public int slj=0;

    public bool isActiv = false;

    public float scalingFactor;

    NewBetterHook.Node sentail;

    NewBetterHook.Node currNode;

    NewBetterHook.Node rover;

    NewBetterHook.Node Printrover;

    public GrappleManager grap;

    public float ropeLength;

    bool ispressed;

    private SpriteRenderer spriteRenderer;




    private bool isHooked = true;

    void Start()
    {
        //mainCamera = Camera.main;
        CreateGrappleHook(hookPointPrefab, jointPrefab);
        //ActivateHookConnection(test);

        sentail = grappleHook.GetTail().GetRightNode();
        currNode = sentail.GetRightNode();
        rover = currNode.GetRightNode();
        Printrover = currNode;
        
        spriteRenderer = GetComponent<SpriteRenderer>();

        
        
    }

    void Update()
    {
        // Handle input for grapple hook creation (if needed)
        /*
        if (Input.GetKeyDown(KeyCode.G)) // Example key press for testing
        {
            CreateGrappleHook(hookPointPrefab);
        }
        */

        if ((Input.GetMouseButtonUp(0) || Input.GetKeyDown(KeyCode.K)) && isActiv)
        {
            ispressed = false;
        }

        if ((Input.GetMouseButtonDown(0)|| Input.GetKeyDown(KeyCode.K))&& !isActiv)
        {
            ispressed = true;
        }

        if (Input.GetKeyDown(KeyCode.B)){
            Debug.Break();
        }


       
    }

void FixedUpdate()
    {
        grap.hooked = isActiv;
        //Debug.Log(isActiv);
        
        if (!ispressed)
        {

            
            DeactivateHookConnection();

            /*
            if (Printrover.getName() == null){
                Debug.Log(Printrover.GetObj().name);
            }else{
                Debug.Log(rover.getName());
            }
            
            Debug.Log(Printrover.index);
            Printrover =Printrover.GetRightNode();
            */

        }

        if (ispressed)
        {

            ActivateHookConnection(test);

            //Debug.Log( grappleHook.GetTail().GetRightNode());
            //Debug.Log(sentail.GetRightNode());
            //Debug.Log(currNode.GetRightNode());
            //Debug.Log(currNode);

            /*
            if (Printrover.getName() == null){
                Debug.Log(Printrover.GetObj().name);
            }else{
                Debug.Log(rover.getName());
            }
            
            Debug.Log(Printrover.index);
            Printrover =Printrover.GetRightNode();
            */

        }

        

        

        //    Debug.Log(grappleHook.GetTail().GetObj().name);
        if (isActiv){
            //CheckAndAdjustDistanceSum(ropeLength);

        

            if (!rover.Equals(sentail) && !currNode.Equals(sentail)  ) {


                if (currNode.GetObj() != null && rover.GetObj() != null) {
                    Vector2? collisionPoint = CheckLayerOverlap(
                        currNode.GetObj().transform.position, 
                        rover.GetObj().transform.position,
                        6, 
                        widthCast, GetWorldHeight(currNode.GetObj())/2
                    );
                    //Debug.Log(Vector2.Distance(currNode.GetObj().transform.position, rover.GetObj().transform.position));
                    
                    if (collisionPoint != null && posDistCheck((Vector2)collisionPoint, minDist)) {
                        GameObject newhingObj = Instantiate(jointPrefab, (Vector2)collisionPoint, Quaternion.identity);
                        NewBetterHook.Node sllk = grappleHook.AddNewNode(currNode, rover, newhingObj);
                        sllk.GetObj().name = sllk.GetObj().name + slj;
                        slj = slj+1;

                        Debug.Log(currNode.GetObj().name + ", " + sllk.GetObj().name + ", " + rover.GetObj().name);
                        
                        makeDistJpoint(currNode.GetObj(), newhingObj);
                        makeDistJpoint(newhingObj, rover.GetObj());


                    } 

                    NewBetterHook.Node roverRov = rover.GetRightNode();

                    if (roverRov.GetObj() != null){
                        Vector2? needed = CheckLayerOverlap(
                        currNode.GetObj().transform.position, 
                        roverRov.GetObj().transform.position, 
                        6, 
                        widthCast, 0f);

                        if (needed == null){
                            float dist = currNode.GetObj().GetComponent<DistanceJoint2D>().distance +rover.GetObj().GetComponent<DistanceJoint2D>().distance;
                            grappleHook.popNode(rover);
                            if (dist > ropeLength || currNode.Equals(grappleHook.GetHead()) && roverRov.Equals(grappleHook.GetTail()) ){
                                dist = ropeLength;
                            }
                            makeDistJpoint(currNode.GetObj(), roverRov.GetObj(), dist);
                        }

                    }
                    
                    
                    
                    if (collisionPoint != null && !rover.Equals(playerObj) && !currNode.Equals(hookPointPrefab)){


                    }
                }
                
            }
            currNode = currNode.GetRightNode();
            rover = currNode.GetRightNode();
        }
    }




    
    public static float GetWorldHeight(GameObject gameObject)
    {
        // Get the renderer's bounds in world space
        Renderer renderer = gameObject.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("The GameObject does not have a Renderer component.");
            return 0;
        }
        
        // Calculate the height by taking the difference between max and min y values of the bounds
        float worldHeight = renderer.bounds.size.y;

        return worldHeight;
    }




    public void CreateGrappleHook(GameObject hookPoint, GameObject jointPrefab)
    {
        grappleHook = new NewBetterHook(  jointPrefab);
        grappleHook.AddNewNode(playerObj);
        //grappleHook.PrintNodeNames();
        grappleHook.AddNewNode(hookPoint);
        //grappleHook.PrintNodeNames();
    }
public static Vector2 ModifyVectorStart(Vector2 startPoint, Vector2 endPoint, float offset)
{
    Vector2 direction = (endPoint - startPoint).normalized;
    return startPoint + direction * offset;
}

public static Vector2? CheckLayerOverlap(Vector2 pointA, Vector2 pointB, int layerIndex, float width, float disCir)
{
    LayerMask layerMask = 1 << layerIndex;
    Vector2 direction = pointB - pointA;
    float distance = direction.magnitude;
    Vector2 directionNormalized = direction.normalized;

    // Calculate the perpendicular vector to add thickness
    Vector2 perpendicular = Vector2.Perpendicular(directionNormalized) * (width / 2);

    pointA = ModifyVectorStart(pointA, pointB, disCir);

    // Calculate the corners of the box
    Vector2 topLeft = pointA + perpendicular;
    Vector2 topRight = pointA - perpendicular;
    Vector2 bottomLeft = pointB + perpendicular;
    Vector2 bottomRight = pointB - perpendicular;

    if (true)
    {
        // Draw the outline of the box cast
        Debug.DrawLine(topLeft, bottomLeft, Color.red);
        Debug.DrawLine(bottomLeft, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, topRight, Color.red);
        Debug.DrawLine(topRight, topLeft, Color.red);
    }

    // Perform the BoxCast with the specified width
    RaycastHit2D hit = Physics2D.BoxCast(pointA, new Vector2(width, width), 0f, directionNormalized, distance, layerMask);

    if (hit.collider != null)
    {
        // Print the name of the object it collides with
        Debug.Log("Collided with: " + hit.collider.gameObject.name);

        // Calculate the point along the center line at the same distance as the hit point
        float hitDistance = Vector2.Distance(pointA, hit.point);
        Vector2 centerLinePoint = pointA + directionNormalized * hitDistance;

        return centerLinePoint;
    }

    // Return null if no collision was detected
    return null;
}




    // New activation function
    public void ActivateHookConnection(GameObject targetHookPoint)
    {
        if (!isActiv)
    {

        spriteRenderer.color = new Color(1, 0, 0);
        ropeLength = Vector2.Distance(playerObj.gameObject.transform.position, targetHookPoint.transform.position);


        if (playerObj != null && targetHookPoint != null)
        {
            isHooked = true;
            // Check if the player already has a DistanceJoint2D and remove it if it exists
            DistanceJoint2D existingJoint = playerObj.GetComponent<DistanceJoint2D>();
            if (existingJoint != null)
            {
                Destroy(existingJoint);
            }

            // Add Rigidbody2D components if they don't exist
            Rigidbody2D playerRigidbody = playerObj.GetComponent<Rigidbody2D>();
            if (playerRigidbody == null)
            {
                playerRigidbody = playerObj.AddComponent<Rigidbody2D>();
            }

            Rigidbody2D hookRigidbody = targetHookPoint.GetComponent<Rigidbody2D>();
            if (hookRigidbody == null)
            {
                hookRigidbody = targetHookPoint.AddComponent<Rigidbody2D>();
            }

            // Create a new DistanceJoint2D on the player object
            
            DistanceJoint2D playerJoint = playerObj.AddComponent<DistanceJoint2D>();
            playerJoint.connectedBody = hookRigidbody;
            playerJoint.autoConfigureDistance = false;
            playerJoint.maxDistanceOnly =true;

            isActiv = true;

            sentail = grappleHook.GetTail().GetRightNode();
            currNode = sentail.GetRightNode();
            rover = currNode.GetRightNode();

            Printrover = currNode;

            Debug.Log("Grapple activated.");

            if (!IsMovingTowards(playerRigidbody, targetHookPoint.transform.position)){
                RedirectVelocityAlongCircle(targetHookPoint.transform.position, playerObj, scalingFactor);
            }
            

            //playerJoint.distance = Vector2.Distance(playerObj.transform.position, targetHookPoint.transform.position);
        }
    }
    }


   public void DeactivateHookConnection()
{
    // Check if the grapple hook is active
    if (isActiv)
    {

        spriteRenderer.color = new Color(1, 1, 1);
        // Reset the 'isActiv' flag
        isActiv = false;

        // Disable the DistanceJoint2D on the player if it exists
        DistanceJoint2D playerJoint = playerObj.GetComponent<DistanceJoint2D>();
        if (playerJoint != null)
        {
            playerJoint.enabled = false; // Deactivate the joint on the player
        }

        // Optionally, you can disable the DistanceJoint2D on other nodes if needed
        NewBetterHook.Node point = grappleHook.GetHead().GetRightNode();
        NewBetterHook.Node next = point.GetRightNode();
        while (!point.Equals(grappleHook.GetHead()) && !point.Equals(grappleHook.GetTail()) ){
            grappleHook.popNode(point);
            point = next;
            next = next.GetRightNode();

        }
        

        // Optionally, reset other variables or states related to the grapple hook
        // For example, if you want to clear out any node references or reset node positions
        sentail = null;
        currNode = null;
        rover = null;
        Printrover = null;

        Debug.Log("Grapple deactivated.");
    }
}





    void makeDistJpoint(GameObject input, GameObject otherConnect) {
        DistanceJoint2D playerJoint = input.GetComponent<DistanceJoint2D>();
        if (playerJoint == null) {
            playerJoint = input.AddComponent<DistanceJoint2D>();
        }
        
        Rigidbody2D hookRigid = otherConnect.GetComponent<Rigidbody2D>();
        if (hookRigid == null) {
            hookRigid = otherConnect.AddComponent<Rigidbody2D>();
        }

        playerJoint.connectedBody = hookRigid;
        playerJoint.autoConfigureDistance = false; // Set this first
        playerJoint.maxDistanceOnly = true;

        // Calculate and set the distance between input and otherConnect objects
        playerJoint.distance = Vector2.Distance(input.transform.position, otherConnect.transform.position);

        // Optional: Debugging info
        //Debug.Log("DistanceJoint2D created with distance: " + playerJoint.distance);
    }

    void makeDistJpoint(GameObject input, GameObject otherConnect, float distance) {
        DistanceJoint2D playerJoint = input.GetComponent<DistanceJoint2D>();
        if (playerJoint == null) {
            playerJoint = input.AddComponent<DistanceJoint2D>();
        }
        
        Rigidbody2D hookRigid = otherConnect.GetComponent<Rigidbody2D>();
        if (hookRigid == null) {
            hookRigid = otherConnect.AddComponent<Rigidbody2D>();
        }

        playerJoint.connectedBody = hookRigid;
        playerJoint.autoConfigureDistance = false; // Set this first
        playerJoint.maxDistanceOnly = true;

        // Calculate and set the distance between input and otherConnect objects
        playerJoint.distance = distance;

        // Optional: Debugging info
        //Debug.Log("DistanceJoint2D created with distance: " + playerJoint.distance);
    }



    public bool posDistCheck(Vector2 nodoObj, float minDist) {
        foreach (NewBetterHook.Node node in grappleHook.IterateNodes()) {
            if (Vector2.Distance(nodoObj, node.GetObj().transform.position) <= minDist) {
                return false; // Return immediately if any node is too close
            }
        }
        return true; // All nodes are sufficiently far
    }


    public bool CheckAndAdjustDistanceSum(float threshold)
{
    float totalDistance = 0f;
    bool retu = false;

    // Get the first node in the grapple hook (head)
    NewBetterHook.Node point = grappleHook.GetHead();
    NewBetterHook.Node next = point.GetRightNode();
    
    // Go through all the nodes in the grapple hook
    while (!point.Equals(grappleHook.GetTail()))
    {
        // Get the current DistanceJoint2D
        DistanceJoint2D joint = point.GetObj().GetComponent<DistanceJoint2D>();
        
        // Calculate the actual distance between the objects connected by the joint
        Vector2 objPos = point.GetObj().transform.position;
        Vector2 nextPos = next.GetObj().transform.position;
        float actualDistance = Vector2.Distance(objPos, nextPos);

        // Add the actual distance to the total
        totalDistance += actualDistance;

        // Move to the next node
        point = next;
        next = next.GetRightNode();
    }

    // Compare the total distance with the threshold
    if (totalDistance <= threshold)
    {
        retu = true; // Total distance is less than or equal to the threshold
        float difference = threshold - totalDistance;

        // Adjust the head's DistanceJoint2D distance to fit the threshold
        DistanceJoint2D headJoint = grappleHook.GetHead().GetObj().GetComponent<DistanceJoint2D>();
        if (headJoint != null)
        {
            // Adjust the head's joint distance
            headJoint.distance += difference;
            //Debug.Log("Adjusted the head's DistanceJoint2D distance by: " + difference);
        }
    }
    else
    {
        // If total distance exceeds threshold, no adjustment needed, just return false
        return false; // Total distance is greater than the threshold
    }

    return retu; // Return true if the total distance is within the threshold
}



void RedirectVelocityAlongCircle(Vector2 circleCenter, GameObject playerObject, float scalingFactor)
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

    // Determine which direction the player is moving
    float dotClockwise = Vector2.Dot(playerVelocity.normalized, tangentClockwise);
    float dotCounterclockwise = Vector2.Dot(playerVelocity.normalized, tangentCounterclockwise);
    bool isClockwise = dotClockwise > dotCounterclockwise;

    // Redirect velocity along the tangent
    Vector2 newVelocity = isClockwise
        ? tangentClockwise * playerVelocity.magnitude
        : tangentCounterclockwise * playerVelocity.magnitude;

    // Modify momentum if scaling factor is non-zero
    if (scalingFactor != 0)
    {
        Debug.Log("modifyting");
        newVelocity = ModifyMomentum(playerRigidbody, newVelocity, 1 + scalingFactor);
    }

    // Apply the new velocity to the player
    playerRigidbody.velocity = newVelocity;
}

// Function to modify momentum using projection
Vector2 ModifyMomentum(Rigidbody2D playerRigidbody, Vector2 targetVelocity, float scalingFactor)
{
    Debug.Log("working");

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

    public bool IsMovingTowards(Rigidbody2D rb, Vector2 targetPoint)
    {
        // Get the current position and velocity of the Rigidbody2D
        Vector2 currentPosition = rb.position;
        Vector2 velocity = rb.velocity;

        // Calculate the direction to the target point
        Vector2 directionToTarget = (targetPoint - currentPosition).normalized;

        // Calculate the normalized velocity direction
        Vector2 normalizedVelocity = velocity.normalized;

        // Check if the velocity direction is similar to the direction to the target
        return Vector2.Dot(normalizedVelocity, directionToTarget) > 0;
    }








}
