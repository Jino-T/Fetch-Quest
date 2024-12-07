using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pullHook : MonoBehaviour
{
    [HideInInspector]
    public GameObject hookBase;
    // Start is called before the first frame update
    void Start()
    {
        hookBase = transform.parent.gameObject;

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void activate(){
        hookBase.GetComponent<movePlatScript>().MovePlatform(this.transform.up);

    }


}
