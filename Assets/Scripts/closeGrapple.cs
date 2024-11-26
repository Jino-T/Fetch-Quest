using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class closeGrapple : MonoBehaviour
{

    private playerStateScript playerState;
    // Start is called before the first frame update
    void Start()
    {
        playerState = this.GetComponent<playerStateScript>();
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D[] colliders = PerformBoxCast(this.GetComponent<holdAutoGrappleManager>().widthHook);
        Collider2D[] corrColorCollid = PerformBoxCast(this.GetComponent<holdAutoGrappleManager>().widthHook*2);

        foreach (Collider2D collider in corrColorCollid){
            collider.gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f);
        }

        if (playerState.canGrappleHook && colliders.Length > 0){
            GetClosest(colliders).gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.5f, 0.0f) ;

        }

        
        
    }

    private Collider2D[] PerformBoxCast(float inputWidthHook)
    {
        //Debug.Log("dik");
        // Define the width and height of the box for the BoxCast
        //float width = 2f; // Set as needed for the width of the box
        //float height = 1f; // Set as needed for the height of the box

        // Perform the BoxCast using stored direction and size
        Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, inputWidthHook, this.GetComponent<holdAutoGrappleManager>().hooks );
        return colliders;

        
    }

    private Collider2D GetClosest(Collider2D[] colliders){
        Collider2D closestCollider = null;
        float closestDistance = Mathf.Infinity;
        
        foreach (Collider2D collider in colliders)
        {
            collider.gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f);
            float distance = Vector2.Distance(this.transform.position, collider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCollider = collider;
            }
        }
        return closestCollider;
    }
}
