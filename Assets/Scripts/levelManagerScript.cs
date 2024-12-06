using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

public class levelManagerScript : MonoBehaviour
{
    
    public bool hasKeys = false;
    public bool hasLevers = false;



    //number of keys in the level
    public int numKeys;
    //number of keys the player has picked up
    public int numKeysHeld;
    
    private bool collAllKeys;

    
    private bool hitAllLevers;

    public bool canExit;

    private GameObject[] hookableObjs;

    private LinkedList<leverScript> leverScripts = new LinkedList<leverScript>();

    



    // Start is called before the first frame update
    void Start()
    {

        hookableObjs  = GameObject.FindGameObjectsWithTag("ObjToPlayer"); 
        numKeysHeld = 0;

        foreach (GameObject obj in hookableObjs ){
            
            if ( hasLevers && obj.name == "lever"){
                //Debug.Log("idk");
                //Debug.Log(obj.GetComponent<leverScript>());
                leverScripts.AddLast(obj.GetComponent<leverScript>());
                //Debug.Log(leverScripts.First);
            }

            if (obj.name == "BestPlayer"){
                Debug.Log("idk");
                var hookShotManager = obj.GetComponent<hookShotManager>();
                hookShotManager.levelManger = this.gameObject;
            }
            
        }

        

        
    }

    // Update is called once per frame
    void Update()
    {
        


        if (hasKeys){
            if ( numKeys == numKeysHeld){
                collAllKeys = true;
            }
        }

        if (hasLevers){
            bool hitLevers = true;
            foreach (leverScript script in leverScripts ){
                hitLevers = hitLevers && script.isActive;
            }
            hitAllLevers = hitLevers;
        }

        
        


        canExit = metAllConstion();
    }

    private bool metAllConstion(){
        bool metConstion = true;
        
        if (hasKeys){
            metConstion =  metConstion && collAllKeys; 
        }

        if ( hasLevers){
            metConstion = metConstion && hitAllLevers;
        }

        return metConstion;

    }

    public void activHookedObj(GameObject hookedObj){


        if (hookedObj.name == "lever"){
            hookedObj.GetComponent<leverScript>().activate();
        }

        if (hookedObj.name == "moveablePlatform"){

        }

        
    }
}
