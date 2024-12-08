using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class leverScript : MonoBehaviour
{

    public bool isActive = false;

    public Sprite redLever;
    public Sprite greenLever;

    public bool activeByPlayerCollision = false;

    private  SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr =this. GetComponent<SpriteRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            //Debug.Log("Active");
            sr.sprite = greenLever;
        }
        else
        {
            //Debug.Log("Not Active");
            sr.sprite = redLever;
        }
        
    }

    public void activate()
    {
        isActive = !isActive;
        Debug.Log("switch");
        // Update sprite based on activation state
       

        
    }


/*
    void OnTriggerEnter2D(Collider2D other)
{
    
    

    if ((other.CompareTag("Player") || 
         other.gameObject.layer == LayerMask.NameToLayer("Player")) 
         && activeByPlayerCollision)
    {
        //Debug.Log("switch");
        Debug.Log($"Collided with: {other.name}, Tag: {other.tag}, Layer: {other.gameObject.layer}");
        this.activate();
    }
    
   
}
*/





    
}
