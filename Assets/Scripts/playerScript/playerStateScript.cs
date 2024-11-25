using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerStateScript : MonoBehaviour
{

    
    public bool isJumping;

    public bool pushingJump;

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
        
    }

    void FixedUpdate()
    {
        if ( conSidedGround && !isGrappleHook && !isHookShot){
            canHookShot = true;
            canGrappleHook = true;
        }
    }
}
