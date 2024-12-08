using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;


//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class ropeScript : MonoBehaviour
{
    //public GameObject test;

    public GameObject hingObj;
    public GameObject endObj;
    public GameObject jointPrefab;        // Prefab of the joint GameObject
    //public GameObject hookPointPrefab;    // Prefab for the hook point
    //public LayerMask collisionLayer;      // Layer to check for collisions
    //public float maxJointDistance = 1.0f; // Maximum distance between nodes

    public NewBetterHook grappleHook;    // Instance of the hook class
    

    public float minDist;

    public float widthCast;
    public float activeWidthCast;

    public int slj=0;

    //public bool isActiv = false;

    NewBetterHook.Node sentail;

    NewBetterHook.Node currNode;

    //NewBetterHook.Node rover;

    NewBetterHook.Node Printrover;

    //public GrappleManager grap;

    public float ropeLength;


    private SpriteRenderer spriteRenderer;
    //private playerStateScript playerState;




    //private bool isHooked = true;

    void Start()
    {
        //mainCamera = Camera.main;
        
        //ActivateHookConnection(test);
        //grappleHook = this.GetComponent<NewBetterHook>();
        //grap = this.GetComponent<GrappleManager>();
        //playerState = this.GetComponent<playerStateScript>();

        grappleHook = new NewBetterHook( jointPrefab);
        grappleHook.AddNewNode(endObj);
        currNode = sentail;

        
        //grappleHook = new NewBetterHook(jointPrefab);

        DistanceJoint2D baseRope = this.gameObject.AddComponent<DistanceJoint2D>();
        baseRope.autoConfigureDistance = false;
        baseRope.maxDistanceOnly = true;
        baseRope.distance = Vector2.Distance(endObj.transform.position, hingObj.transform.position);

        Rigidbody2D endRigidbody = endObj.GetComponent<Rigidbody2D>();
        if (endRigidbody == null)
        {
            endRigidbody = endObj.AddComponent<Rigidbody2D>();
        }

        baseRope.connectedBody = endRigidbody;


        sentail = grappleHook.getSentinel();
        
        Printrover = currNode;
        
        spriteRenderer = GetComponent<SpriteRenderer>();

        

        //grappleHook.AddNewNode(this.gameObject);
        /**/

        //ActivateHookConnection()

        ActivateHookConnection(hingObj);
        
        
    }

    void Update()
    {
        

  
        
        //grap.hooked = isActiv;
        //Debug.Log(isActiv);
        /*
        if (!ispressed)
        {

            
            DeactivateHookConnection();

            */
            
            
            /*

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
            

        }
         */

         if (Input.GetKeyDown(KeyCode.P)){
            printList();
         }

         

        

        

        //    Debug.Log(grappleHook.GetTail().GetObj().name);
        //if (playerState.isGrappleHook){
            //CheckAndAdjustDistanceSum(ropeLength);

            NewBetterHook.Node rover = currNode.GetRightNode();

            if (!rover.Equals(sentail) && !currNode.Equals(sentail) && !currNode.Equals(grappleHook.GetTail())&& !rover.Equals(grappleHook.GetHead())) {
                //Debug.Log("it happening");

                if (currNode.GetObj() != null && rover.GetObj() != null) {
                    //Debug.Log("ok");
                    Vector2? collisionPoint;
                    if (currNode.Equals(grappleHook.GetHead())){
                        //Debug.Log("head");
                        collisionPoint = CheckLayerOverlap(
                        currNode.GetObj().transform.position, 
                        rover.GetObj().transform.position,
                        6, 
                        widthCast, GetWorldHeight(currNode.GetObj())/2,  GetWorldHeight(rover.GetObj())/2, true);

                        //Debug.Log(currNode.GetObj().name);
                        //Debug.Log(rover.GetObj().name);
                        //Debug.Log(collisionPoint);
                    }else{
                        //Debug.Log("else");
                        collisionPoint = CheckLayerOverlap(
                        currNode.GetObj().transform.position, 
                        rover.GetObj().transform.position,
                        6, 
                        activeWidthCast, GetWorldHeight(currNode.GetObj())/2, GetWorldHeight(rover.GetObj())/2, true);
                        

                    }
                    
                    if ( collisionPoint != null){
                        //Debug.Log(currNode.GetObj().name);
                    }else{
                        //Debug.Log("NO collition");
                        //Debug.Break();
                    }
                    
                    
                    //Debug.Log(Vector2.Distance(currNode.GetObj().transform.position, rover.GetObj().transform.position));
                    
                    if (collisionPoint != null && posDistCheck((Vector2)collisionPoint, minDist)) {
                        
                        Vector2 directionNormalized = (currNode.GetObj().transform.position- rover.GetObj().transform.position).normalized;
                        Vector2 offset = directionNormalized * (jointPrefab.GetComponent<Renderer>().bounds.size.x) ;

                        Vector2 point1 = (Vector2)collisionPoint + offset;
                        Vector2 point2 = (Vector2)collisionPoint - offset;

                        if( grappleHook.count == 2){
                            NewBetterHook.Node createdNode =makeHing(currNode,rover, (Vector2)collisionPoint );
                            makeHing(createdNode.GetLeftNode(),createdNode,point1);
                            makeHing(createdNode, createdNode.GetRightNode(),point2);
                            currNode = createdNode;
                            rover = currNode.GetRightNode();

                            /*

                            NewBetterHook.Node createdNode =makeHing(currNode,rover, (Vector2)collisionPoint );
                            makeHing(createdNode.GetLeftNode(),createdNode,point2);
                            makeHing(createdNode, createdNode.GetRightNode(),point1);
                            currNode = createdNode;
                            rover = currNode.GetRightNode();
                            
                            */

                        }else{
                            makeHing(currNode,rover, (Vector2)collisionPoint );
                        }
                        
                        
                        



                    } 

                    NewBetterHook.Node roverRov = rover.GetRightNode();

                    if (roverRov.GetObj() != null){

                        Vector2? needed = CheckLayerOverlap(
                        currNode.GetObj().transform.position, 
                        roverRov.GetObj().transform.position, 
                        6, 
                        widthCast, 0f, false);

                        if (needed == null && grappleHook.count > 3 && rover.GetObj().GetComponent<DistanceJoint2D>() != null){
                            float dist = currNode.GetObj().GetComponent<DistanceJoint2D>().distance +rover.GetObj().GetComponent<DistanceJoint2D>().distance;
                            grappleHook.popNode(rover);
                            if ( !rover.Equals(grappleHook.GetHead()) && !rover.Equals(grappleHook.GetTail())){
                                Destroy(rover.GetObj());
                            }
                            if (dist > ropeLength || currNode.Equals(grappleHook.GetHead()) && roverRov.Equals(grappleHook.GetTail()) ){
                                dist = ropeLength;
                            }
                            makeDistJpoint(currNode.GetObj(), roverRov.GetObj(), dist);
                        }

                    }
                    
                    
                    

                }
                
            }


           



            currNode = currNode.GetRightNode();
            
        
    }

    private NewBetterHook.Node makeHing( NewBetterHook.Node left,  NewBetterHook.Node  right, Vector2 point ){
        //Debug.Log("creating new node");
        GameObject newhingObj = Instantiate(jointPrefab, point, Quaternion.identity);
        NewBetterHook.Node sllk = grappleHook.AddNewNode(left, right, newhingObj);
        sllk.GetObj().name = sllk.GetObj().name + slj;
        slj = slj+1;

        //Debug.Log(left.GetObj().name + ", " + sllk.GetObj().name + ", " + right.GetObj().name);
        
        makeDistJpoint(left.GetObj(), newhingObj);
        makeDistJpoint(newhingObj, right.GetObj());
        return sllk;
    }

    private Vector2 idk(Vector2 collisionPoint, Vector2 currPoz){
        Vector2 direction = (collisionPoint - currPoz).normalized;
        return direction * 20;
        

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


/*

    public void CreateGrappleHook(GameObject hookPoint, GameObject jointPrefab)
    {
        if (isActiv){
            currNode = sentail.GetRightNode();
            //rover = currNode.GetRightNode();
            
        }else{
            grappleHook = new NewBetterHook( jointPrefab);
            grappleHook.AddNewNode(endObj);
            //grappleHook.PrintNodeNames();
            //grappleHook.AddNewNode(hookPoint);
            //grappleHook.PrintNodeNames();
            currNode = sentail.GetRightNode();
            //this.ActivateHookConnection(hookPoint);
            

        }
        
        

        
    }

     public void CreateGrappleHook( GameObject jointPrefab)
    {
        if (isActiv){
            currNode = sentail.GetRightNode();
            
            
        }else{
            grappleHook = new NewBetterHook( jointPrefab);
            grappleHook.AddNewNode(endObj);
            //grappleHook.PrintNodeNames();
            //grappleHook.AddNewNode(hookPoint);
            //grappleHook.PrintNodeNames();
            currNode = sentail.GetRightNode();
            //rover = currNode.GetRightNode();

        }
        
        

        
    }


    */
public static Vector2 ModifyVectorStart(Vector2 startPoint, Vector2 endPoint, float offset)
{
    Vector2 direction = (endPoint - startPoint).normalized;
    return startPoint + direction * offset;
}

public static Vector2? CheckLayerOverlap(Vector2 pointA, Vector2 pointB, int layerIndex, float width, float disCir, bool draw)
{
    LayerMask layerMask = 1 << layerIndex;
    Vector2 direction = pointB - pointA;
    float distance = Vector2.Distance(pointA, pointB) - disCir;
    Vector2 directionNormalized = direction.normalized;

    // Calculate the perpendicular vector to add thickness
    Vector2 perpendicular = Vector2.Perpendicular(directionNormalized) * (width / 2);

    pointA = ModifyVectorStart(pointA, pointB, disCir);

    // Calculate the corners of the box
    Vector2 topLeft = pointA + perpendicular;
    Vector2 topRight = pointA - perpendicular;
    Vector2 bottomLeft = pointB + perpendicular;
    Vector2 bottomRight = pointB - perpendicular;

    if (draw)
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
        //Debug.Log("Collided with: " + hit.collider.gameObject.name);

        // Calculate the point along the center line at the same distance as the hit point
        float hitDistance = Vector2.Distance(pointA, hit.point);
        Vector2 centerLinePoint = pointA + directionNormalized * hitDistance;

        return centerLinePoint;
    }

    // Return null if no collision was detected
    return null;
}


public static Vector2? CheckLayerOverlap(Vector2 pointA, Vector2 pointB, int layerIndex, float width, float disStartCir, float disEndCir, bool draw)
{
    LayerMask layerMask = 1 << layerIndex;
    Vector2 direction = pointB - pointA;
    float distance = Vector2.Distance(pointA, pointB) - (disEndCir+ disStartCir);
    Vector2 directionNormalized = direction.normalized;

    // Calculate the perpendicular vector to add thickness
    Vector2 perpendicular = Vector2.Perpendicular(directionNormalized) * (width / 2);

    pointA = ModifyVectorStart(pointA, pointB, disStartCir);
    pointB = ModifyVectorStart ( pointA, pointB, disEndCir);

    // Calculate the corners of the box
    Vector2 topLeft = pointA + perpendicular;
    Vector2 topRight = pointA - perpendicular;
    Vector2 bottomLeft = pointB + perpendicular;
    Vector2 bottomRight = pointB - perpendicular;

    if (draw)
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
        //Debug.Log("Collided with: " + hit.collider.gameObject.name);

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
        
        //CreateGrappleHook(this.gameObject, targetHookPoint);
        //grappleHook.AddNewNode(targetHookPoint);

        spriteRenderer.color = new Color(1, 0, 0);
        ropeLength = Vector2.Distance(endObj.gameObject.transform.position, targetHookPoint.transform.position);

         //printList();
        
        grappleHook.AddNewNode(targetHookPoint);

        //Debug.Log( "///////");

         //printList();


        if (endObj != null && targetHookPoint != null)
        {
            //isHooked = true;
            // Check if the player already has a DistanceJoint2D and remove it if it exists
            DistanceJoint2D existingJoint = endObj.GetComponent<DistanceJoint2D>();
            if (existingJoint != null)
            {
                Destroy(existingJoint);
            }

            // Add Rigidbody2D components if they don't exist
            Rigidbody2D playerRigidbody = endObj.GetComponent<Rigidbody2D>();
            if (playerRigidbody == null)
            {
                playerRigidbody = endObj.AddComponent<Rigidbody2D>();
            }

            Rigidbody2D hookRigidbody = targetHookPoint.GetComponent<Rigidbody2D>();
            if (hookRigidbody == null)
            {
                hookRigidbody = targetHookPoint.AddComponent<Rigidbody2D>();
                hookRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
                
            }

            // Create a new DistanceJoint2D on the player object
            
            DistanceJoint2D playerJoint = endObj.AddComponent<DistanceJoint2D>();
            playerJoint.connectedBody = hookRigidbody;
            playerJoint.autoConfigureDistance = false;
            playerJoint.maxDistanceOnly =true;

            //playerState.isGrappleHook = true;

            sentail = grappleHook.GetTail().GetRightNode();
            currNode = sentail.GetRightNode();
            //rover = currNode.GetRightNode();

            Printrover = currNode;

            //Debug.Log("Grapple activated.");

            
            

            //playerJoint.distance = Vector2.Distance(endObj.transform.position, targetHookPoint.transform.position);
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


// Function to modify momentum using projection


    void printList()
{
    // Iterates through each node using the IterateNodes() method.
    foreach (NewBetterHook.Node node in grappleHook.IterateNodes())
    {
        // Check if the node has a name in the object.
        if (node.GetObj() != null && node.GetObj().name != null)
        {
            Debug.Log(node.GetObj().name);
        }
        else
        {
            Debug.Log(node.getName());
        }
    }
}









}

