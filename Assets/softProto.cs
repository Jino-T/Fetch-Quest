using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

public class softProto : MonoBehaviour
{
    public GameObject segmentPrefab; // Prefab for the rope segment
    //private int segmentCount ; // Number of segments in the rope
    private float segmentLength; // Length of each segment
    public Transform startPoint; // Transform for the starting point of the rope
    public Vector2 endPoint; // Transform for the ending point of the rope
    public Camera mainCamera;

     private GameObject last;

    //public DistanceJoint2D _distanceJoint;
    

    private List<GameObject> ropeSegments = new List<GameObject>(); // List to keep track of the rope segments

    public LineRenderer _lineRenderer;
    public DistanceJoint2D _distanceJoint;

    public LayerMask groundLayer;

    public Movescript player;

    public GrappleManager grappleManager;

    private bool softRope;
    public bool active = false;
    public bool start = false;

    public bool hardWhenGround = false;

    public bool fitWhenGround = false;
    private bool softSlack = false;

    private bool newColid = false;
    public int coliderInRope = 2;

    public float scalingFactor;

    public float perimLean;

    

    private Vector2 pointA;
    private Vector2 pointB;

    //public GameObject playerObj;

    private float currlength;

    private Vector2 pointActive;

    public bool mouseAct;




    Vector2 mousePos;
    Vector3 mousePosition;
    Vector3 cirPlayerPos;

    private bool over;

    private bool overremeb = false;


    // Start is called before the first frame update
    void Start()
    {
        _distanceJoint.enabled = false;
        _distanceJoint.maxDistanceOnly = true;
        segmentLength =segmentPrefab.GetComponent<Renderer>().bounds.size.y;


        if ( player.isGrounded && hardWhenGround ){
            _distanceJoint.maxDistanceOnly = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        // Assume all segments are false initially
        newColid = false;

        // Check each segment for isTig status
        foreach (GameObject seg in ropeSegments)
        {
            if (seg.GetComponent<PrefabScript>().isTig)
            {
                newColid = true;
                break;  // Stop checking further if any isTig is true
            }
        }

        //Debug.Log(newColid); // Continuously logs the status of newColid
    }

void FixedUpdate()
{
    // Check if the player is grounded and the rope is active
    bool over = CheckLayerOverlap(this.transform.position, endPoint, 6);
    
    // Update startPoint position to match the player's position
    //startPoint.transform.position = player.transform.position;
    /*
    if (active && ropeSegments.Count > 0)
    {
        player.transform.position = last.transform.position;
    }
    */
    if (mouseAct){
        if (Input.GetKeyDown(KeyCode.Mouse0) && !over)
        {
            mousePos = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
            HandleMouseDown(mousePos);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            HandleMouseUp();
        }
    }

    if (active)
    {
        UpdateRopeBehavior(over);
    }

    // Update the LineRenderer position if the distance joint is enabled
    if (_distanceJoint.enabled)
    {
        _lineRenderer.SetPosition(1, transform.position);
    }
    //Debug.Log(newColid);
    //newColid =false;
    //bool LocalColid = false;
}





        public void HandleMouseDown(Vector2 hookpoz)
    {
        grappleManager.hooked = true;
        //Debug.Log("hjgj");
        //GameObject tempObj = new GameObject("TempEndpointObject");
        //tempObj.transform.position = hookpoz;
        pointActive = this.transform.position;
        endPoint = hookpoz;

        //Destroy(tempObj);
        //mousePos = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
        //Vector3 mousePosition = new Vector3( endPoint.transform.position.x, endPoint.transform.position.y, 0f);
        cirPlayerPos = this.GetComponent<Rigidbody2D>().position;

        currlength = Vector2.Distance(this.transform.position, endPoint);

        //Debug.Log("recongised mouse Down" );

        if (player.isGrounded){
            if (hardWhenGround){
                Debug.Log("k");
                turnHard(false, true,currlength);
                softRope =false;
            }else{
                //Debug.Log("i want to sleep");
                turnSoft();
                softRope =true;
            }
        }else{ 
            Debug.Log("A");
            turnHard(true, true, currlength);
            

        }

        active =true;
        //newColid = false;
    }

    public void HandleMouseUp()
    {
        grappleManager.hooked = false;
        ////Debug.Log("mouse up" );
        active = false;
        _distanceJoint.enabled = false;
        _lineRenderer.enabled = false;
        this.GetComponent<Rigidbody2D>().mass = 1f;

        if ( ropeSegments.Count > 0){
            List<GameObject> copy = ropeSegments;
            //this.GetComponent<Rigidbody2D>().velocity  = copy.First<GameObject>().GetComponent<Rigidbody2D>().velocity;
            this.gameObject.GetComponent<Renderer>().enabled = true;
            this.GetComponent<Collider2D>().isTrigger = false;
            deleRope();
        }
        

        overremeb = false;

    }

    private void UpdateRopeBehavior(bool over)
    {
        grappleManager.hooked = true;
        ////Debug.Log(player.isGrounded);
            
            

            if (!overremeb && over){
                    Debug.Log("t");
                        turnSoft();
                        overremeb = true;
                        //newColid = true;
            }
            if ( !player.isGrounded){

                if ( !over){
                    //Debug.Log(isOnCircumference(endPoint, pointActive, this.transform.position, 2f ));
                    if (hardWhenGround && softRope && !over && !newColid && isOnCircumference(endPoint, pointActive, this.transform.position, perimLean ) ){
                        Debug.Log("0");

                        turnHard(false, true, currlength);
                    }

                    
                    //if want to have soft rope when not at max curcufances

                    /*
                    if (softSlack){
                        if (!moveTowordPoz(mousePos) && softRope == true && !newColid && isOnCircumference( mousePos, cirPlayerPos, this.GetComponent<Rigidbody2D>().position, 0.5f) && !over  ){
                        //Debug.Log("o");
                        
                        turnHard(false, hardWhenGround);
                        }
        

                    
                    


                    if ( moveTowordPoz(mousePos) && !overremeb && over){
                        ////Debug.Log("h");

                        turnSoft();
                        overremeb = true;
                    }

                    }*/
                    
                    
                    
                    


                }else{

                }
            } else{
                ////Debug.Log("active");
                //Debug.Log(isOnCircumference(endPoint, pointActive, this.transform.position, 2f ));
                if (hardWhenGround && softRope && !over && !newColid ){
                    Debug.Log("0");

                    turnHard(false, true, currlength);
                    //overremeb = false;
                }

                /*
                if ( !over  && fitWhenGround && !softRope &&  this.GetComponent<Rigidbody2D>().velocity.magnitude < 1f  ){
                    Debug.Log("l");
                    turnHard(false, true, currlength);
                }
                */

                if(!hardWhenGround && !overremeb) {
                    Debug.Log("7");

                    turnSoft();
                    overremeb = true;
                }
                if ( fitWhenGround){
                    currlength = Vector2.Distance(this.transform.position, endPoint);
                }
            }
            if (player.isGrounded && fitWhenGround && !softRope){
                Debug.Log("apple");
                turnHard(false, true,currlength);
                currlength = Vector2.Distance(this.transform.position, endPoint);
            }
            

    }

    void CreateRope()
{
    
    if (startPoint == null || endPoint == null)
    {
        //Debug.LogError("StartPoint and EndPoint must be assigned!");
        return;
    }
    this.GetComponent<Rigidbody2D>().mass = 0f;

    GameObject previousSegment = null;

    //Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    //endPoint = new Vector3(endPoint.transform.position.x, endPoint.transform.position.y, endPoint.position.z);

    Vector2 playerHeight = this.GetComponent<Renderer>().bounds.size;

    float distance = currlength;
    float segmentCount = (distance) / segmentLength;
    int wholeSegmentCount = Mathf.CeilToInt(segmentCount);
    wholeSegmentCount = wholeSegmentCount- 4;

    // Define layer indices (assuming RopeLayer and LastSegmentLayer have been created)
    int ropeLayer = LayerMask.NameToLayer("RopeLayer");
    int lastSegmentLayer = LayerMask.NameToLayer("LastSegmentLayer");
    
    

    for (int i = 0; i < wholeSegmentCount; i++)
    {
        Vector2 segmentPosition = Vector2.Lerp(
    new Vector2(startPoint.position.x, startPoint.position.y + playerHeight.y), 
    endPoint, 
    (float)i / (wholeSegmentCount - 1)
);
        GameObject segment = Instantiate(segmentPrefab, segmentPosition, Quaternion.identity);
        segment.name = "RopeSegment_" + i;
        ropeSegments.Add(segment);

        // Set the layer for the rope segments
        segment.layer = ropeLayer;

        Rigidbody2D rb = segment.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = segment.AddComponent<Rigidbody2D>();
            if (i == wholeSegmentCount - 1)
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
            }
            rb.gravityScale = 1f;
        }

        if (previousSegment != null)
        {
            segment.GetComponent<Rigidbody2D>().mass = 0;
            DistanceJoint2D joint = segment.AddComponent<DistanceJoint2D>();
            joint.connectedBody = previousSegment.GetComponent<Rigidbody2D>();
            joint.autoConfigureDistance = false; // Disable automatic distance configuration
            joint.distance = segmentLength; // Set the initial distance
            joint.maxDistanceOnly = true; // Allow shortening but prevent growing
        }
        if (segment.name.Equals( "RopeSegment_0") ){
            DistanceJoint2D joint = segment.AddComponent<DistanceJoint2D>();
            joint.connectedBody = this.GetComponent<Rigidbody2D>();
            joint.autoConfigureDistance = false; // Disable automatic distance configuration
            joint.distance = playerHeight.y/2; // Set the initial distance
            joint.maxDistanceOnly = true;
            last =segment;

            //ConnectPlayerToRope(this.gameObject, segment.gameObject);
        }
        

        previousSegment = segment;

        // Set collider trigger behavior
        if (i % 2 == 0)
        {
            segment.GetComponent<Collider2D>().isTrigger = false;
        }
        else
        {
            segment.GetComponent<Collider2D>().isTrigger = true;
        }
    }

    for (int i = 0; i < wholeSegmentCount; i++)
    {
        float velocityFactor = Mathf.Lerp(1f, 0.1f, (float)i / (segmentCount - 1));
        //ropeSegments[i].GetComponent<Rigidbody2D>().velocity = this.GetComponent<Rigidbody2D>().velocity * velocityFactor;
    }

    // Assign the 'last' object and set its properties
    /*
    last = ropeSegments.First<GameObject>();
    last.transform.localScale = new Vector3(1f, 1f, 1f);
    last.GetComponent<Collider2D>().isTrigger = false;
    last.GetComponent<Rigidbody2D>().mass = 0;
    last.GetComponent<Rigidbody2D>().gravityScale = 1;
    last.GetComponent<Rigidbody2D>().freezeRotation = true;
    last.AddComponent<PlayerMovement>();
    */

    // Set the layer for the 'last' segment
    this.gameObject.layer = lastSegmentLayer;
    

    // Prevent collision between the rope and the last segment
    Physics2D.IgnoreLayerCollision(ropeLayer, lastSegmentLayer, true);

    // Create the final joint connecting to the endPoint
    DistanceJoint2D finalJoint = previousSegment.AddComponent<DistanceJoint2D>();
    finalJoint.connectedAnchor = endPoint;
    finalJoint.distance = 0.5f;
    finalJoint.maxDistanceOnly = true;
    finalJoint.autoConfigureDistance = false;
    finalJoint.GetComponent<Rigidbody2D>().mass = 0;
    last.GetComponent<Rigidbody2D>().mass = 0;
}

void ConnectPlayerToRope(GameObject playerObject, GameObject ropeEndPoint)
{
    // Get the player's Rigidbody2D and DistanceJoint2D components
    Rigidbody2D playerRigidbody = playerObject.GetComponent<Rigidbody2D>();
    DistanceJoint2D distanceJoint = playerObject.GetComponent<DistanceJoint2D>();

    // Get the player's height (assuming height is measured along the y-axis)
    float playerHeight = playerObject.GetComponent<SpriteRenderer>().bounds.size.y;

    // Calculate the distance between the player and the rope endpoint
    Vector2 playerPosition = playerRigidbody.position;
    Vector2 endpointPosition = ropeEndPoint.transform.position;
    float distanceToEndpoint = Vector2.Distance(playerPosition, endpointPosition);

    // Enable the DistanceJoint2D if it's not already enabled
    if (!distanceJoint.enabled)
    {
        distanceJoint.enabled = true;
    }

    // Set the connected body of the DistanceJoint2D to the rope endpoint's Rigidbody2D
    Rigidbody2D endpointRigidbody = ropeEndPoint.GetComponent<Rigidbody2D>();
    distanceJoint.connectedBody = endpointRigidbody;


    // Adjust the distance to ensure the rope length matches the player's height
    //distanceJoint.distance = Mathf.Max(distanceToEndpoint, this.GetComponent<Renderer>().bounds.size.y/2);

    // Additional configuration for the joint, if necessary
    //distanceJoint.distance = Vector2.Distance(playerObject.transform.position, ropeEndPoint.transform.position); // Ensures custom distance setting
    distanceJoint.enableCollision = true; // Enables collision between the player and rope end
    distanceJoint.autoConfigureDistance = false;
    distanceJoint.maxDistanceOnly = true;
}




    
    void deleRope(){
        if (ropeSegments.Count > 0){
            // Delete previously created rope segments
            GameObject copyLast = ropeSegments.First<GameObject>();

            foreach (GameObject segment in ropeSegments)
            {
                Destroy(segment);
            }
            ropeSegments.Clear(); // Clear the list after deletion
            
            //this.GetComponent<Rigidbody2D>().velocity = copyLast.GetComponent<Rigidbody2D>().velocity;
        }
        this.gameObject.GetComponent<Renderer>().enabled = true;
        this.gameObject.layer = 0;
        this.GetComponent<Rigidbody2D>().mass = 1f;
        
       
        
    }




    void turnSoft(){
        this.GetComponent<Collider2D>().isTrigger = true; // Reset collider to non-trigger
        //this.gameObject.GetComponent<Renderer>().enabled = false;
        if (!softRope){
            //active = false;
            _distanceJoint.enabled = false;
            _lineRenderer.enabled = false;
        }
        //endPoint = new Vector3(endPoint.transform.position.x, endPoint.transform.position.y, endPoint.position.z);
        this.GetComponent<Collider2D>().isTrigger = false;

        //this.gameObject.GetComponent<Renderer>().enabled = false;
        CreateRope();
        //active = true;
        softRope = true;
        overremeb = true;
    }

    void turnHard(bool red, bool maxdis, float dist){
       //Debug.Log("apply dist:" + dist);
        this.GetComponent<Collider2D>().isTrigger = false; // Reset collider to non-trigger
        this.gameObject.GetComponent<Renderer>().enabled = true;
        if (softRope){
            deleRope();
        }
        ////Debug.Log("bleee");
        _lineRenderer.SetPosition(0, endPoint);
        _lineRenderer.SetPosition(1, endPoint);
        _distanceJoint.connectedAnchor = endPoint;
        _distanceJoint.enabled = true;
        _lineRenderer.enabled = true;
        if (red && !IsMovingTowards(this.GetComponent<Rigidbody2D>(),endPoint )){
            RedirectVelocityAlongCircle(endPoint, this.gameObject, scalingFactor);
            Debug.Log("perfict");
        }
        
        //active = true;
        softRope = false;
        overremeb = false;
        _distanceJoint.maxDistanceOnly = true;

        _distanceJoint.distance = dist ;
        //Debug.Log("_distanceJoint.distance: "+  _distanceJoint.distance);
    }

    /*
    void turnHard(bool red, bool maxdis ){
        
        this.GetComponent<Collider2D>().isTrigger = false; // Reset collider to non-trigger
        this.gameObject.GetComponent<Renderer>().enabled = true;
        if (softRope){
            deleRope();
        }
        ////Debug.Log("bleee");
        _lineRenderer.SetPosition(0, endPoint);
        _lineRenderer.SetPosition(1, transform.position);
        _distanceJoint.connectedAnchor = endPoint;
        _distanceJoint.enabled = true;
        _lineRenderer.enabled = true;
        if (red && !IsMovingTowards(this.GetComponent<Rigidbody2D>(),endPoint )){
            RedirectVelocityAlongCircle(endPoint, this.gameObject, scalingFactor);
            Debug.Log("perfict");
        }
        
        //active = true;
        softRope = false;
        overremeb = false;
        _distanceJoint.maxDistanceOnly = true;

       //currlength = _distanceJoint.distance;
       //Debug.Log("getting currlength: " + currlength);
    }
    */



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




    bool isOnCircumference(Vector2 centerPoint, Vector2 circumferencePoint, Vector2 objectPosition, float tolerance )
    {
        // Calculate the radius of the circle
        float radius = Vector2.Distance(centerPoint, circumferencePoint);
        
        // Calculate the distance between the center point and the object's position
        float distanceToObject = Vector2.Distance(centerPoint, objectPosition);
        
        // Check if the distance to the object is approximately equal to the radius, within a tolerance
        return Mathf.Abs(distanceToObject - radius) <= tolerance;
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
        ////Debug.Log("Overlap detected with layer: " + layerName);
        return true;
    }

    ////Debug.Log("No overlap detected.");
    return false;
}


    // Optionally, you can visualize the ray in the Scene view for debugging
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(pointA, pointB);
    }


    
}
