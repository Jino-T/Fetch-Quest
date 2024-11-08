using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
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
    private Camera mainCamera;

    public float minDist;

    public float widthCast;

    public int slj=0;

    public bool isActiv = false;

    NewBetterHook.Node sentail;

    NewBetterHook.Node currNode;

    NewBetterHook.Node rover;

    NewBetterHook.Node Printrover;

    public GrappleManager grap;




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

       
    }

void FixedUpdate()
    {
        grap.hooked = isActiv;
        //Debug.Log(isActiv);
        
        if (Input.GetKeyDown(KeyCode.K))
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

        if (Input.GetKeyDown(KeyCode.J))
        {

            ActivateHookConnection(test);

            Debug.Log( grappleHook.GetTail().GetRightNode());
            Debug.Log(sentail.GetRightNode());
            Debug.Log(currNode.GetRightNode());
            Debug.Log(currNode);

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

        

            if (!rover.Equals(sentail) && !currNode.Equals(sentail)  ) {


                if (currNode.GetObj() != null && rover.GetObj() != null) {
                    Vector2? collisionPoint = CheckLayerOverlap(
                        currNode.GetObj().transform.position, 
                        rover.GetObj().transform.position, 
                        6, 
                        widthCast
                    );
                    Debug.Log(Vector2.Distance(currNode.GetObj().transform.position, rover.GetObj().transform.position));
                    
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
                        widthCast);

                        if (needed == null){
                            float dist = currNode.GetObj().GetComponent<DistanceJoint2D>().distance +rover.GetObj().GetComponent<DistanceJoint2D>().distance;
                            grappleHook.popNode(rover);
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




    
    




    public void CreateGrappleHook(GameObject hookPoint, GameObject jointPrefab)
    {
        grappleHook = new NewBetterHook(  jointPrefab);
        grappleHook.AddNewNode(playerObj);
        //grappleHook.PrintNodeNames();
        grappleHook.AddNewNode(hookPoint);
        //grappleHook.PrintNodeNames();
    }

    public static Vector2? CheckLayerOverlap(Vector2 pointA, Vector2 pointB, int layerIndex, float width, bool drawDebug = false)
    {
        LayerMask layerMask = 1 << layerIndex;
        Vector2 direction = pointB - pointA;
        float distance = direction.magnitude;
        Vector2 directionNormalized = direction.normalized;

        // Calculate the perpendicular vector to add thickness
        Vector2 perpendicular = Vector2.Perpendicular(directionNormalized) * (width / 2);

        // Calculate the corners of the box
        Vector2 topLeft = pointA + perpendicular;
        Vector2 topRight = pointA - perpendicular;
        Vector2 bottomLeft = pointB + perpendicular;
        Vector2 bottomRight = pointB - perpendicular;

        if (drawDebug)
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
            

            //playerJoint.distance = Vector2.Distance(playerObj.transform.position, targetHookPoint.transform.position);
        }
    }
    }


   public void DeactivateHookConnection()
{
    // Check if the grapple hook is active
    if (isActiv)
    {
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





}

