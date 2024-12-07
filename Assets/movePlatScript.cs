using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEditor;
using UnityEngine;

public class movePlatScript : MonoBehaviour
{

    public float leftMoveMax;
    public float rightMoveMax;
    public float downMoveMax;
    public float upMoveMax;

    public float distPerMov;

    public float movSpeed;

    private bool isMovingTo = false;

    public Vector3 movToPoz;



    // Start is called before the first frame update
    void Start()
    {
        leftMoveMax = this.gameObject.transform.position.x - leftMoveMax;
        rightMoveMax += this.gameObject.transform.position.x;

        downMoveMax = this.gameObject.transform.position.y- downMoveMax ;
        upMoveMax += this.gameObject.transform.position.y;

        movToPoz = this.gameObject.transform.position;


        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void FixedUpdate()
    {
        if (isMovingTo){
            
            this.transform.position = Vector3.MoveTowards(this.transform.position, movToPoz , movSpeed  );
        }
        
    }
    


    public void MovePlatform(Vector2 moveVector){
        // expects a normalized vector
        
        moveVector = moveVector.normalized;
        Debug.Log(moveVector);
        moveVector *= distPerMov;
        float xMovPos = moveVector.x + this.gameObject.transform.position.x;
        float yMovPos = moveVector.y + this.gameObject.transform.position.y;

        xMovPos = Mathf.Clamp(xMovPos , leftMoveMax, rightMoveMax);
        yMovPos = Mathf.Clamp(yMovPos, downMoveMax, upMoveMax);

        
        movToPoz = new Vector3 (xMovPos, yMovPos, this.transform.position.z  );


        if (this.transform.position != movToPoz ){
            isMovingTo = true;


        }

        //Debug.Break();



    }

    void OnDrawGizmos()
    {
        
        //Gizmos.DrawSphere(movToPoz, 0.1f);
        
    }
}
