using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using System;

public class GrappleManager : MonoBehaviour
{
    public float hookDrag;
    public int hookType;
    public bool hooked;
    public bool mouseAct = false;

    private Vector2 storedDirection;
    public LayerMask hooks;

    private Rigidbody2D rb;
    private GameInput controls;
    public Movescript playerMoveState; // Updated from Movescript to PlayerMovement
    public GameObject playerObject;

    private void Start()
    {
        rb = playerObject.GetComponent<Rigidbody2D>();
        playerObject.GetComponent<softProto>().mouseAct = mouseAct;
    }

    private void Update() {
        //isGrounded = GrappleManager
    }

    private void Awake()
    {
        controls = new GameInput();
        controls.Player.JHook.performed += _ => Hook();
        //controls.Player.Movment.performed += cxt => InputCheak(cxt.ReadValue<Vector2>());
    }

    

    private void OnEnable()
    {
        controls.Enable();
        //controls.Player.Movment.performed += InputCheak; 
        //controls.Player.Movment.canceled += DeInputCheak;
    }

    private void OnDisable()
    {
        controls.Disable();
        //controls.Player.Movment.performed -= InputCheak; 
        //controls.Player.Movment.canceled -= DeInputCheak;
    }

    private void Hook()
    {
        if (hooked)
        {
            //hooked = false;
            playerObject.GetComponent<softProto>().HandleMouseUp();
        }
        else if (!hooked && !playerMoveState.isGrounded)
        {
            Debug.DrawRay(rb.position, storedDirection * Mathf.Infinity, Color.red, 2f);
            RaycastHit2D hit = Physics2D.Raycast(rb.position, storedDirection, Mathf.Infinity, hooks);
            if (hit.collider != null)
            {
                //hooked = true;
                Vector2 hitPosition = hit.point;
                UseSoftProto(hitPosition);
                //Debug.Log("Hook hit object position: " + hitPosition);
            }
            else
            {
                //Debug.Log("No target hit in the hook direction.");
            }
        }
    }

    //can be use to set reset length when on the ground but its buggy --> can make soft rope go throught
    
    /*
    private void InputCheak(InputAction.CallbackContext vector2)
    {
        if (vector2.ReadValue<Vector2>().y < 0 && hooked && playerMoveState.isGrounded   ){
            softProto ropescript = playerObject.GetComponent<softProto>();
            playerObject.GetComponent<softProto>().fitWhenGround = true;
            if (ropescript.softRope && !playerObject.GetComponent<softProto>().over){
                ropescript.turnHard(false, true);
            }
        }
    }

    private void DeInputCheak(InputAction.CallbackContext vector2){
         playerObject.GetComponent<softProto>().fitWhenGround = false;
    }
    */
    
    


    private void UseSoftProto(Vector2 endPos)
    {
        Debug.Log("Using Soft Proto Hook");
        playerObject.GetComponent<softProto>().HandleMouseDown(endPos);
    }
}
