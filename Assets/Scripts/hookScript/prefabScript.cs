using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabScript : MonoBehaviour
{
    public bool isTig = false;

    public int objectLayer;      // The layer of the object with the Rigidbody2D
    public int ignoredLayer;

    void Start()
    {
        // Ensure the object is on the specified layer
        gameObject.layer = 7;

        // Ignore collisions between the object's layer and the ignored layer
        Physics2D.IgnoreLayerCollision(7, 10, true);
    }

    

    void OnTriggerStay2D(Collider2D other)
    {
        // Set isTig to true if triggered by an object on the "ground" layer
        if (other.gameObject.layer == LayerMask.NameToLayer("ground"))
        {
            isTig = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Set isTig to false when exiting an object on the "ground" layer
        if (other.gameObject.layer == LayerMask.NameToLayer("ground"))
        {
            //Debug.Log("Left ground trigger");
            isTig = false;
        }
    }
}
