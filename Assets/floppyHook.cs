using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public GameObject segmentPrefab; // Prefab for the rope segment
    public int segmentCount ; // Number of segments in the rope
    private float segmentLength; // Length of each segment
    public Transform startPoint; // Transform for the starting point of the rope
    public Transform endPoint; // Transform for the ending point of the rope
    public Camera mainCamera;

    //public DistanceJoint2D _distanceJoint;
    public PlayerMovement player;

    private List<GameObject> ropeSegments = new List<GameObject>(); // List to keep track of the rope segments

    private GameObject last;

    public LineRenderer _lineRenderer;
    public DistanceJoint2D _distanceJoint;

    public LayerMask groundLayer;


    private bool activ = false;

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
        if (Input.GetKeyDown(KeyCode.Mouse0) )
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            endPoint.position = new Vector3(mousePosition.x, mousePosition.y, endPoint.position.z);
            player.GetComponent<Collider2D>().isTrigger = true;

            player.gameObject.GetComponent<Renderer>().enabled = false;
            CreateRope();
            //player.GetComponent<Collider2D>().isTrigger = true;
            
            //_distanceJoint.enabled = true; // Enable the joint
            activ =true;
        }

        // Delete the rope and disable the DistanceJoint2D when the mouse button is released
        
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


     void CreateRope()
    {
    if (startPoint == null || endPoint == null)
    {
        Debug.LogError("StartPoint and EndPoint must be assigned!");
        return;
    }

    GameObject previousSegment = null;

    Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    endPoint.position = new Vector3(mousePosition.x, mousePosition.y, endPoint.position.z);

    float distance = Vector2.Distance(this.transform.position, mousePosition);
    float segmentCount = distance / (segmentLength);
    int wholeSegmentCount = Mathf.FloorToInt(segmentCount);

    for (int i = 0; i < wholeSegmentCount; i++)
    {
        Vector2 segmentPosition = Vector2.Lerp(startPoint.position, endPoint.position, (float)i / (wholeSegmentCount - 1));
        GameObject segment = Instantiate(segmentPrefab, segmentPosition, Quaternion.identity);
        segment.name = "RopeSegment_" + i;
        ropeSegments.Add(segment);

        Rigidbody2D rb = segment.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = segment.AddComponent<Rigidbody2D>();
            rb.gravityScale = 1;
        }

        if (previousSegment != null)
        {
            DistanceJoint2D joint = segment.AddComponent<DistanceJoint2D>();
            joint.connectedBody = previousSegment.GetComponent<Rigidbody2D>();
            joint.autoConfigureDistance = false; // Disable automatic distance configuration
            joint.distance = segmentLength; // Set the initial distance
            joint.maxDistanceOnly = true; // Allow shortening but prevent growing
        }

        previousSegment = segment;

        // Set collider trigger behavior
        if (i % 4 == 0)
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

    last = ropeSegments.First<GameObject>();
    last.transform.localScale = new Vector3(1f, 1f, 1f);
    last.GetComponent<Collider2D>().isTrigger = false;
    last.GetComponent<Rigidbody2D>().mass = 1;

    DistanceJoint2D finalJoint = previousSegment.AddComponent<DistanceJoint2D>();
    finalJoint.connectedAnchor = endPoint.position;
    finalJoint.distance = 0f;
    finalJoint.maxDistanceOnly = true; // Ensure the final joint behaves the same
    finalJoint.autoConfigureDistance = false;
}

    void deleRope(){
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
