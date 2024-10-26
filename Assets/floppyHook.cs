using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public GameObject segmentPrefab; // Prefab for the rope segment
    public int segmentCount ; // Number of segments in the rope
    private float segmentLength; // Length of each segment
    public Transform startPoint; // Transform for the starting point of the rope
    //public Vector2 endPoint; // Transform for the ending point of the rope
    public Camera mainCamera;

    //public DistanceJoint2D _distanceJoint;
    public Movescript player;

    private List<GameObject> ropeSegments = new List<GameObject>(); // List to keep track of the rope segments

    private GameObject last;

    public LineRenderer _lineRenderer;
    public DistanceJoint2D _distanceJoint;

    public LayerMask groundLayer;

    public int detal;


    private bool activ = false;

    public bool mouseAct;

    void Start()
    {
        segmentLength = segmentPrefab.GetComponent<Renderer>().bounds.size.y;
        //_distanceJoint.enabled = false;
        
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate()
    {
        // Update the start point to the player's position
        startPoint = player.transform;

        // Constantly update the connected anchor of the DistanceJoint2D when active
        if (activ && ropeSegments.Count > 0)
        {
            player.transform.position = last.transform.position;
            //_distanceJoint.connectedAnchor = 
        }
        
        // Create the rope and activate the DistanceJoint2D when the mouse button is pressed
        if (mouseAct){
            if (Input.GetKeyDown(KeyCode.Mouse0) )
            {
                Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                Vector2 endPoint = new Vector2(mousePosition.x, mousePosition.y);
                player.GetComponent<Collider2D>().isTrigger = true;

                player.gameObject.GetComponent<Renderer>().enabled = false;
                CreateRope(endPoint,detal);
                //player.GetComponent<Collider2D>().isTrigger = true;
                
                //_distanceJoint.enabled = true; // Enable the joint
                activ =true;
            }  
            if ((Input.GetKeyUp(KeyCode.Mouse0) )&& ropeSegments.Count > 0)
            {
                
                List<GameObject> copy = ropeSegments;
                player.GetComponent<Rigidbody2D>().velocity  = copy.First<GameObject>().GetComponent<Rigidbody2D>().velocity;
                player.gameObject.GetComponent<Renderer>().enabled = true;
                player.GetComponent<Collider2D>().isTrigger = false;
                deleRope();
                activ = false;

                //_distanceJoint.enabled = false; // Disable the joint after deleting the rope
            }
        
        }
        

        // Delete the rope and disable the DistanceJoint2D when the mouse button is released
        
        
    }


    public void CreateRope(Vector2 endPoint, int detal)
 {
    if (startPoint == null || endPoint == null)
    {
        //Debug.LogError("StartPoint and EndPoint must be assigned!");
        return;
    }

    GameObject previousSegment = null;

    //Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    //endPoint = new Vector3(endPoint.transform.position.x, endPoint.transform.position.y, endPoint.position.z);

    float distance = Vector2.Distance(this.transform.position, endPoint);
    float segmentCount = distance / segmentLength;
    int wholeSegmentCount = Mathf.FloorToInt(segmentCount);

    // Define layer indices (assuming RopeLayer and LastSegmentLayer have been created)
    int ropeLayer = LayerMask.NameToLayer("RopeLayer");
    int lastSegmentLayer = LayerMask.NameToLayer("LastSegmentLayer");

    for (int i = 0; i < wholeSegmentCount; i++)
    {
        Vector2 segmentPosition = Vector2.Lerp(startPoint.position, endPoint, (float)i / (wholeSegmentCount - 1));
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
        if (i % detal == 0)
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
    last.AddComponent<Movescript>();

    // Set the layer for the 'last' segment
    last.layer = lastSegmentLayer;

    // Prevent collision between the rope and the last segment
    Physics2D.IgnoreLayerCollision(ropeLayer, lastSegmentLayer, true);

    // Create the final joint connecting to the endPoint
    DistanceJoint2D finalJoint = previousSegment.AddComponent<DistanceJoint2D>();
    finalJoint.connectedAnchor = endPoint;
    finalJoint.distance = 0.5f;
    finalJoint.maxDistanceOnly = true;
    finalJoint.autoConfigureDistance = false;
}

    public void deleRope(){
        if (ropeSegments.Count > 0){
            // Delete previously created rope segments
            foreach (GameObject segment in ropeSegments)
            {
                Destroy(segment);
            }
            ropeSegments.Clear(); // Clear the list after deletion
        }
        
    }
}
