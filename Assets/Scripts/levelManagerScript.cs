using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement; 

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

    public bool atExit =false;

    public bool exiting =false;

    

    private GameObject[] hookableObjs;

    private LinkedList<leverScript> leverScripts = new LinkedList<leverScript>();

    private LinkedList<GameObject> keyObjcsList = new LinkedList<GameObject>();

    public Animator circleWipe;
    public float transitionTime = 1f;

    public int levelIndex = -1;

    



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

            /*
            if ( hasKeys && obj.name == "Key"){
                keyManager targetScript = obj.GetComponent<keyManager>();
                targetScript.levelManager = this.gameObject;
            }
            */

            if (obj.name == "BestPlayer"){
                Debug.Log("idk");
                hookShotManager hookShotManager = obj.GetComponent<hookShotManager>();
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

        if( atExit && !exiting){
            LoadNextLevel();
            exiting =true;

        }
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

        if (hookedObj.name.Length >= "PullHookPref".Length &&hookedObj.name.Substring(0, "PullHookPref".Length) == ( "PullHookPref")){
            hookedObj.GetComponent<pullHook>().activate();


        }

        
    }


    private void LoadNextLevel() {
        if (levelIndex == -1){
            StartCoroutine(loadLevel(SceneManager.GetActiveScene().buildIndex + 1));
        }else{
            StartCoroutine(loadLevel(levelIndex));
        }
        
    }

    IEnumerator loadLevel(int levelIndex) {
        //Play animation
        circleWipe.SetTrigger("Start");

        //wait
        yield return new WaitForSeconds(transitionTime);

        //load scene
        SceneManager.LoadScene(levelIndex);        
    } 


}
