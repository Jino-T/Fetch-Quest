using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;        

public class LockedDoorScript : MonoBehaviour
{

    public Collider2D doorCollider;
    public int nextScene;
    public bool locked;
    [SerializeField] GameObject player;
    public GameObject levelManager;


    void Start() {
        locked = true;
    } 

    void Update() {
        //Debug.Log(levelManager.GetComponent<levelManagerScript>().numKeys);
        //Debug.Log(levelManager.GetComponent<levelManagerScript>().numKeysHeld);
        if(!locked) {
            
        }
    } 

    //Gets called when something collides with the door. This is where we can add scene change logic.
    private void OnTriggerEnter2D(Collider2D other) {
        //if the collision is the key and the player is holding all the keys in the level
        if (other.gameObject.CompareTag("Player") && 
            levelManager.GetComponent<levelManagerScript>().canExit) {
            levelManager.GetComponent<levelManagerScript>().atExit = true;
        }
    }

    //Gets called when collider leaves door. We may need this for animation or scene change logic.
    /*
    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.CompareTag("Key")) { 
            locked = true;
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(92, 64, 51);
        }
    }
    */
        
}
