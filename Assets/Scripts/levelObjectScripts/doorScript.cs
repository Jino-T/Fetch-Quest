using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;        

public class DoorScript : MonoBehaviour
{

    public Collider2D doorCollider;
    public int nextScene;

    //Gets called when something collides with the door. This is where we can add scene change logic.
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {

            Debug.Log("Test");
            SceneManager.LoadScene(nextScene);        
        }
    }
        
}
