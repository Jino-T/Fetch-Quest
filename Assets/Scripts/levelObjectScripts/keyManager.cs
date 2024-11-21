using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class keyManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    public GameObject levelManager;
    public bool isPickedUp;

    private UnityEngine.Vector2 vel;

    //controls speed that keys follow player
    public float smoothTime;

    //horizontal offset for key; change when you have multiple keys
    public float xOffset;

    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPickedUp) {
            UnityEngine.Vector3 offset = new UnityEngine.Vector3(xOffset, 1, 0);
            transform.position = UnityEngine.Vector2.SmoothDamp(transform.position, player.transform.position + offset, ref vel, smoothTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
       if (other.gameObject.CompareTag("Player") && !isPickedUp) {
            isPickedUp = true;
            levelManager.GetComponent<levelManagerScript>().numKeysHeld += 1;
       }
    }
}
