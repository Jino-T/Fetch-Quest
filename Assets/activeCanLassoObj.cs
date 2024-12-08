using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class activeCanLassoObj : MonoBehaviour
{

    private playerStateScript playerState;

    private SpriteRenderer spriteRenderer;

    public float transparency = 0.5f;
    private Color newColor;


    // Start is called before the first frame update
    void Start()
    {
        playerState = this.gameObject.GetComponentInParent<playerStateScript>();
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();

        newColor = spriteRenderer.color;
       

        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerState.canGrappleHook){
            newColor.a = 1f; 
            spriteRenderer.color = newColor;

        


            
        }else{
            newColor.a = transparency; 
            spriteRenderer.color = newColor;
        }
        
    }
}
