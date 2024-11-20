using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class hookPullManager : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D rb;
    public GameObject playerObject;

    private GameInput1 controls;

    private Vector2 storedDirection;

    public bool isHooked;

    public float widthHook;
    public float distHook;

    public LayerMask hookLayer;

    public LineRenderer lineRenderer;

    private void Start()
    {

        rb = playerObject.GetComponent<Rigidbody2D>();
        
    }
    private void OnEnable()
    {
        controls.Enable();
        controls.Player.HookShot.performed += InputCheak; 
        controls.Player.HookShot.canceled += DeInputCheak;

    }

    private void OnDisable()
    {
        controls.Disable();
        controls.Player.HookShot.performed -= InputCheak; 
        controls.Player.HookShot.canceled -= DeInputCheak;
    }

    private void InputCheak(InputAction.CallbackContext value)
    {
        storedDirection = value.ReadValue<Vector2>();
    }

    private void DeInputCheak(InputAction.CallbackContext value){
         
         storedDirection = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        isHooked = this.GetComponent<GrappleManager>().hooked;
        if ( Math.Abs(storedDirection.x) == 1f || Math.Abs(storedDirection.y) == 1f && !isHooked){
            //castHook(storedDirection);
            
            

        }
        
    }

    private void castHook(){

    }


    private RaycastHit2D PerformBoxCast(Vector2 inputDirection)
    {
        lineRenderer.startWidth =widthHook;
        lineRenderer.endWidth =widthHook;
        RaycastHit2D hit = Physics2D.BoxCast(rb.position, new Vector2(widthHook, widthHook), 0f, inputDirection,distHook, hookLayer);
        return hit;
    }
}
