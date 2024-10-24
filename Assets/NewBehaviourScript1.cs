using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NewBehaviourScript1 : MonoBehaviour

{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3 ( player.transform.position.x,  player.transform.position.y +0.5f,  player.transform.position.z);

        
    }
}
