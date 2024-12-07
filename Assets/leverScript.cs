using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class leverScript : MonoBehaviour
{

    public bool isActive = false;

    public Sprite redLever;
    public Sprite greenLever;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void activate(){
        isActive = !isActive;

        //TODO fix sprite renderer for lever
        if (isActive) {
            Debug.Log("Active");
            //SpriteRenderer
            this.gameObject.GetComponent<SpriteRenderer>().sprite = greenLever;
        } else if (!isActive) {
            Debug.Log("Not Active");
            this.gameObject.GetComponent<SpriteRenderer>().sprite = redLever;
        }

    }

    
}
