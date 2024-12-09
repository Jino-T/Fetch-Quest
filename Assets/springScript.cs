using UnityEngine;

public class TriggerHandler : MonoBehaviour
{
    public GameObject playerObj; // Assign the player object in the Inspector
    public float springPower = 10f; // Force applied to the object
    public float playerXfloat = 5f; // Maximum allowed X velocity

    private bool canSpring;

    private playerStateScript stateScript;

    public Animator springAnimContrl;


    private void Start() {
        stateScript  = playerObj.GetComponent<playerStateScript>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger activated");

        // Check if the other object is on the same layer as the player
        if (other.gameObject.layer == playerObj.layer)
        {
            if (stateScript.isGrounded){
                playerObj.transform.position = new Vector3( playerObj.transform.position.x, this.transform.position.y,0f);
                //Debug.Log("itHappend");
                Debug.Log(stateScript.isGrounded);
                //stateScript.isGrounded = false;
            }

            //stateScript.isGrounded = false;
            stateScript.canGrappleHook = true;
            stateScript.canHookShot = true;
            stateScript.isJumping  =true;
            springAnimContrl.SetTrigger("springTrigger");
            //springAnimContrl.trigger
            //Debug.Log("Layer matched");

            // Try to get the Rigidbody2D component
        }
    }

    /// <summary>
    /// Sent each frame where another object is within a trigger collider
    /// attached to this object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerStay2D(Collider2D other)
    {
        stateScript.isGrounded = false;
        stateScript.canGrappleHook = true;
        stateScript.canHookShot = true;
        stateScript.isJumping  =true;

        if (other.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                // Apply upward force
                Debug.Log("itHappend");
                if ( Vector2.Dot(rb.velocity, this.transform.up) <8f && !stateScript.isGrounded ){
                    //canSpring = false;
                    float currentVelocityInDirection = Vector2.Dot(rb.velocity, this.transform.up.normalized);
                    float velocityDifference = springPower - currentVelocityInDirection;
                    rb.velocity += new Vector2 ((this.transform.up.normalized * velocityDifference).x, (this.transform.up.normalized * velocityDifference).y);
                }
                

                // Clamp X velocity
                if (Mathf.Abs(rb.velocity.x) > playerXfloat)
                {
                    rb.velocity = new Vector2(
                        Mathf.Sign(rb.velocity.x) * playerXfloat,
                        rb.velocity.y
                    );
                }
            }
            else
            {
                Debug.LogWarning($"{other.name} does not have a Rigidbody2D component.");
            }
        }
        
    

    /// <summary>
    /// Sent when another object leaves a trigger collider attached to
    /// this object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerExit2D(Collider2D other)
    {
        stateScript.isGrounded = false;
            stateScript.canGrappleHook = true;
            stateScript.canHookShot = true;
            stateScript.isJumping  =true;
        
    }
}
