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

    private GameObject[] hookableObjs;

    private LinkedList<leverScript> leverScripts = new LinkedList<leverScript>();


    public Collider2D doorCollider;
    public Collider2D playerCollider;
    public Animator circleWipe;
    public float transitionTime = 1f;


    // Start is called before the first frame update
    void Start()
    {

        hookableObjs  = GameObject.FindGameObjectsWithTag("ObjToPlayer"); 
        numKeysHeld = 0;

        foreach (GameObject obj in hookableObjs ){
            
            if ( hasLevers && obj.name == "lever"){
                Debug.Log("idk");
                Debug.Log(obj.GetComponent<leverScript>());
                leverScripts.AddLast(obj.GetComponent<leverScript>());
                Debug.Log(leverScripts.First);
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

        if (doorCollider.IsTouching(playerCollider)) {
            Debug.Log("DOOR IS TOUCHING PLAYER");
            LoadNextLevel();
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

    //Gets called when something collides with the door. This is where we can add scene change logic.
    private void LoadNextLevel() {
        if (canExit) {
            StartCoroutine(loadLevel(SceneManager.GetActiveScene().buildIndex + 1));
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
