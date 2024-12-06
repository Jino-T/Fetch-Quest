using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

public class movePlatScript : MonoBehaviour
{
    LinkedList<Transform> listTrans = new LinkedList<Transform>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++) 
        {
            

        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
