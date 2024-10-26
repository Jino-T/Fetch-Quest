using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabScript : MonoBehaviour
{
    public bool isTig = false;

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
