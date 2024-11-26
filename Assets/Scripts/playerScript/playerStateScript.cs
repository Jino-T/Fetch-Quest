using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerStateScript : MonoBehaviour
{

    
    public bool isJumping;

    public bool pushingJump;

    public bool isSliding;

    public bool canSliding;

    public bool isGrounded;

    public bool conSidedGround;

    public bool canGrappleHook;
    
    public bool isGrappleHook; 

    public bool canHookShot;

    public bool isHookShot;

    public int cytotecFames;
    private int currCFames =0;

    private GameObject groundCheckPoint;


    // Start is called before the first frame update
    void Start()
    {
        groundCheckPoint = this.GetComponentInChildren<groundCheckScript>().gameObject;

        
        
        
    }

    // Update is called once per frame
    void Update()
    {

        isGrounded = groundCheckPoint.GetComponent<groundCheckScript>().isGrouned;

        if (isGrounded)
        {
            conSidedGround = true;
            currCFames = cytotecFames;
        }
        else
        {
            if (currCFames > 0 && conSidedGround && !isJumping)
            {
                currCFames -= 1;
            }
            else
            {
                conSidedGround = false;
            }
        }

        canSliding = (conSidedGround  || isGrappleHook  ) && !isSliding && Mathf.Sign(this.gameObject.GetComponent<Rigidbody2D>().velocity.y) <0 || isHookShot;
        
    }

    void FixedUpdate()
    {
        if ( conSidedGround && !isGrappleHook && !isHookShot){
            canHookShot = true;
            canGrappleHook = true;
        }
    }
}
