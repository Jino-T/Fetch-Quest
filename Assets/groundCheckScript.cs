using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundCheckScript : MonoBehaviour
{

    [SerializeField] private LayerMask inputLayer; // Assign in the Inspector, can be left empty
    [SerializeField] private string inputTag;     // Assign in the Inspector, can be left empty or null

    public bool isGrouned;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ensure the triggering object is not the same as this object
        if (other.gameObject != this.gameObject && CheckLayerOrTag(other))
        {
            isGrouned = true;
            // Handle the trigger event here
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Ensure the triggering object is not the same as this object
        if (other.gameObject != this.gameObject && CheckLayerOrTag(other))
        {
           isGrouned = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Ensure the triggering object is not the same as this object
        if (other.gameObject != this.gameObject && CheckLayerOrTag(other))
        {
            isGrouned = false;
        }
    }

    private bool CheckLayerOrTag(Collider2D collider)
    {
        // Check if the layer is set and matches
        bool layerMatches = inputLayer == 0 || ((1 << collider.gameObject.layer) & inputLayer) != 0;

        // Check if the tag is set and matches
        bool tagMatches = string.IsNullOrEmpty(inputTag) || collider.CompareTag(inputTag);

        // Return true if both conditions are satisfied
        return layerMatches && tagMatches;
    }





}