using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolideHook : MonoBehaviour
{
    public Camera mainCamera;
    public LineRenderer _lineRenderer;
    public DistanceJoint2D _distanceJoint;
    public Movescript player;
    public LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        _distanceJoint.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        //bool isGrounded()() = Physics2D.OverlapCircle(new Vector3( this.transform.position.x , this.transform.position.y, this.transform.position.z), 1.001f, groundLayer);
        //Debug.Log(player.isGrounded());
        if (Input.GetKeyDown(KeyCode.Mouse0) && !player.isGrounded() )
        {
            Vector2 mousePos = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
            _lineRenderer.SetPosition(0, mousePos);
            _lineRenderer.SetPosition(1, transform.position);
            _distanceJoint.connectedAnchor = mousePos;
            _distanceJoint.enabled = true;
            _lineRenderer.enabled = true;
            RedirectVelocityAlongCircle(mousePos, this.gameObject);
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0) ||player.isGrounded() )
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

}