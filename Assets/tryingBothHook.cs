using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class tryingBothHook : MonoBehaviour
{
    public GameObject segmentPrefab; // Prefab for the rope segment
    //private int segmentCount ; // Number of segments in the rope
    private float segmentLength; // Length of each segment
    public Transform startPoint; // Transform for the starting point of the rope
    public Transform endPoint; // Transform for the ending point of the rope
    public Camera mainCamera;

     private GameObject last;

    //public DistanceJoint2D _distanceJoint;
    public PlayerMovement player;

    private List<GameObject> ropeSegments = new List<GameObject>(); // List to keep track of the rope segments

    public LineRenderer _lineRenderer;
    public DistanceJoint2D _distanceJoint;

    public LayerMask groundLayer;

    private bool softRope;
    private bool active = false;

    public bool hardWhenGround = false;

    public bool fitWhenGround = false;
    private bool softSlack = false;

    private bool newColid = false;
    public int coliderInRope = 2;

    private Vector2 pointA;
    private Vector2 pointB;

    private float currlength;




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
        

       
    }

    void FixedUpdate()
    {

        


        bool over = CheckLayerOverlap( this.transform.position, mousePos, 6 );
        pointA = this.transform.position;
        pointB = mousePos;
        //OnDrawGizmos();
        
        /*
        if (!softRope){
            overremeb = false;
        }
        */
        //bool isGrounded = Physics2D.OverlapCircle(new Vector3( this.transform.position.x , this.transform.position.y, this.transform.position.z), 1.001f, groundLayer);
        ////Debug.Log(player.isGrounded);

        startPoint.transform.position = player.transform.position;

        if (active && ropeSegments.Count > 0)
        {
            player.transform.position = last.transform.position;
            
            //_distanceJoint.connectedAnchor = 
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) )
        { 
            //Debug.Log("mouse Down " );
            //Debug.Log(!active);
            //Debug.Log(!over);
            //Debug.Log("//////////");
        }

        if (Input.GetKeyDown(KeyCode.Mouse0)  )
        {
            Debug.Log(" mouse Down hook" );

        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && !active && !over )
        {
            Debug.Log(" mouse  hook" );
            //newColid =false;
            HandleMouseDown();

            
        }
        /*

            if (!moveTowordPoz(mousePos)){
                turnHard(true, true);
                softRope = false;
            }else{
                //turnSoft();
                softRope = true;
            }
        */

        if (Input.GetKeyUp(KeyCode.Mouse0) && active   )
        {
            
            HandleMouseUp();
        }

        /*
        else if (  !moveTowordPoz(mousePos) && !player.isGrounded ){
                    turnHard(true,false);
        }
        */
        
        if(active){
            UpdateRopeBehavior(over);
        }else{
            newColid = false;
        }

            /*
             if (player.isGrounded){
                //Debug.Log("somthing");
                if (!hardWhenGround && softRope == false){
                    turnSoft();
                }else if ( hardWhenGround && softRope == true){
                    turnHard(false, true);
                }
            }else
            */
            

        




        if (_distanceJoint.enabled) 
        {
            _lineRenderer.SetPosition(1, transform.position);
        }
            
        



    }


        private void HandleMouseDown()
    {
        mousePos = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mousePosition = new Vector3( mousePos.x, mousePos.y, 0f);
        cirPlayerPos = this.GetComponent<Rigidbody2D>().position;

        //Debug.Log("recongised mouse Down" );

        if (player.isGrounded){
            if (hardWhenGround){
                //Debug.Log("k");
                turnHard(false, hardWhenGround);
                softRope =false;
            }else{
                //Debug.Log("i want to sleep");
                turnSoft();
                softRope =true;
            }
        }else{ 
            if (!moveTowordPoz(mousePos)){
                //Debug.Log("d");

                turnHard(true, true);
            }else{
                //Debug.Log("b");

                    turnHard(false, true);
            }

        }

        active =true;
        //newColid = false;
    }

    private void HandleMouseUp()
    {
        ////Debug.Log("mouse up" );
        active = false;
        _distanceJoint.enabled = false;
        _lineRenderer.enabled = false;

        if ( ropeSegments.Count > 0){
            List<GameObject> copy = ropeSegments;
            this.GetComponent<Rigidbody2D>().velocity  = copy.First<GameObject>().GetComponent<Rigidbody2D>().velocity;
            this.gameObject.GetComponent<Renderer>().enabled = true;
            this.GetComponent<Collider2D>().isTrigger = false;
            deleRope();
        }
        

        overremeb = false;

    }

    private void UpdateRopeBehavior(bool over)
    {
        ////Debug.Log(player.isGrounded);
            
            if (ropeSegments.Count>0){
                
                    foreach (GameObject seg in ropeSegments){
                        newColid = newColid || seg.GetComponent<prefabScript>().isTig;
                        }
            }

            if (!overremeb && over){
                        //Debug.Log("t");
                        turnSoft();
                        overremeb = true;
                        newColid = true;
            }
            

            if ( !player.isGrounded){

                if ( !over){

                    
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
                if (hardWhenGround && softRope && !over ){
                    //Debug.Log("0");

                    turnHard(false, hardWhenGround, currlength);
                    //overremeb = false;
                }

                if ( !over  && fitWhenGround && !softRope &&  this.GetComponent<Rigidbody2D>().velocity.magnitude < 1f  ){
                    //Debug.Log("l");
                    turnHard(false, hardWhenGround, currlength);
                }

                else if(!hardWhenGround && !overremeb) {
                    //Debug.Log("7");

                    turnSoft();
                    overremeb = true;
                }
            }
            if (player.isGrounded && fitWhenGround && !softRope){
                turnHard(false, hardWhenGround);
            }

    }

    void CreateRope()
{
    if (startPoint == null || endPoint == null)
    {
        //Debug.LogError("StartPoint and EndPoint must be assigned!");
        return;
    }

    GameObject previousSegment = null;

    Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    endPoint.position = new Vector3(mousePosition.x, mousePosition.y, endPoint.position.z);

    float distance = Vector2.Distance(this.transform.position, mousePosition);
    float segmentCount = distance / segmentLength;
    int wholeSegmentCount = Mathf.FloorToInt(segmentCount);

    // Define layer indices (assuming RopeLayer and LastSegmentLayer have been created)
    int ropeLayer = LayerMask.NameToLayer("RopeLayer");
    int lastSegmentLayer = LayerMask.NameToLayer("LastSegmentLayer");

    for (int i = 0; i < wholeSegmentCount; i++)
    {
        Vector2 segmentPosition = Vector2.Lerp(startPoint.position, endPoint.position, (float)i / (wholeSegmentCount - 1));
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
        ropeSegments[i].GetComponent<Rigidbody2D>().velocity = this.GetComponent<Rigidbody2D>().velocity * velocityFactor;
    }

    // Assign the 'last' object and set its properties
    last = ropeSegments.First<GameObject>();
    last.transform.localScale = new Vector3(1f, 1f, 1f);
    last.GetComponent<Collider2D>().isTrigger = false;
    last.GetComponent<Rigidbody2D>().mass = 0;
    last.GetComponent<Rigidbody2D>().gravityScale = 1;
    last.GetComponent<Rigidbody2D>().freezeRotation = true;
    last.AddComponent<PlayerMovement>();

    // Set the layer for the 'last' segment
    last.layer = lastSegmentLayer;

    // Prevent collision between the rope and the last segment
    Physics2D.IgnoreLayerCollision(ropeLayer, lastSegmentLayer, true);

    // Create the final joint connecting to the endPoint
    DistanceJoint2D finalJoint = previousSegment.AddComponent<DistanceJoint2D>();
    finalJoint.connectedAnchor = endPoint.position;
    finalJoint.distance = 0.5f;
    finalJoint.maxDistanceOnly = true;
    finalJoint.autoConfigureDistance = false;
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
            this.GetComponent<Rigidbody2D>().velocity = copyLast.GetComponent<Rigidbody2D>().velocity;
        }
        this.gameObject.GetComponent<Renderer>().enabled = true;
       
        
    }




    void turnSoft(){
        this.GetComponent<Collider2D>().isTrigger = true; // Reset collider to non-trigger
        this.gameObject.GetComponent<Renderer>().enabled = false;
        if (!softRope){
            //active = false;
            _distanceJoint.enabled = false;
            _lineRenderer.enabled = false;
        }
        endPoint.position = new Vector3(mousePosition.x, mousePosition.y, endPoint.position.z);
        this.GetComponent<Collider2D>().isTrigger = true;

        this.gameObject.GetComponent<Renderer>().enabled = false;
        CreateRope();
        active = true;
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
        _lineRenderer.SetPosition(0, mousePos);
        _lineRenderer.SetPosition(1, transform.position);
        _distanceJoint.connectedAnchor = mousePos;
        _distanceJoint.enabled = true;
        _lineRenderer.enabled = true;
        if (red){
            RedirectVelocityAlongCircle(mousePos, this.gameObject);
            //Debug.Log("perfict");
        }
        
        active = true;
        softRope = false;
        overremeb = false;
        _distanceJoint.maxDistanceOnly = maxdis;

        _distanceJoint.distance = dist ;
        //Debug.Log("_distanceJoint.distance: "+  _distanceJoint.distance);
    }

    void turnHard(bool red, bool maxdis){
        
        this.GetComponent<Collider2D>().isTrigger = false; // Reset collider to non-trigger
        this.gameObject.GetComponent<Renderer>().enabled = true;
        if (softRope){
            deleRope();
        }
        ////Debug.Log("bleee");
        _lineRenderer.SetPosition(0, mousePos);
        _lineRenderer.SetPosition(1, transform.position);
        _distanceJoint.connectedAnchor = mousePos;
        _distanceJoint.enabled = true;
        _lineRenderer.enabled = true;
        if (red){
            RedirectVelocityAlongCircle(mousePos, this.gameObject);
            //Debug.Log("perfict");
        }
        
        active = true;
        softRope = false;
        overremeb = false;
        _distanceJoint.maxDistanceOnly = maxdis;

       currlength = _distanceJoint.distance;
       //Debug.Log("getting currlength: " + currlength);
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

    bool moveTowordPoz(Vector3 hookPoz)
{
    Vector3 target = new Vector3(mainCamera.ScreenToWorldPoint(hookPoz).x, 
                                 mainCamera.ScreenToWorldPoint(hookPoz).y, 
                                 mainCamera.ScreenToWorldPoint(hookPoz).z);  // Convert hookPoz to world position
    
    Rigidbody2D rb = this.GetComponent<Rigidbody2D>();  // The Rigidbody2D of the player or object
    
    // Get the direction from the object to the target
    Vector2 directionToTarget = (target - transform.position).normalized;

    // Get the object's velocity direction
    Vector2 movementDirection = rb.velocity.normalized;

    // Calculate the dot product
    float dotProduct = Vector2.Dot(movementDirection, directionToTarget);

    // Check if the object is moving towards the target
    return dotProduct > 0;
}



    bool isOnCircumference(Vector2 centerPoint, Vector2 circumferencePoint, Vector2 objectPosition, float tolerance = 0.01f)
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
