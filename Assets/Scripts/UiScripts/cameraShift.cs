using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Camera mainCamera; // Assign your main camera
    public GameObject playerObj; // Assign your player object
    public bool active = false;


    public bool sides;
    public bool isLeft;
    public bool isTop;

    public float smoothSpeed = 0.2f; // Smoothness factor
    public Vector2 offset; // Offset for the camera position

    private Vector3 rightCurrentVelocity = Vector3.zero; // Used for SmoothDamp
    private Vector3 leftCurrentVelocity = Vector3.zero;




    private void FixedUpdate()
    {
        if (active)
        {
            // Determine the target position based on the player's position and offset
            Vector3 targetPosition = playerObj.transform.position;
            if (sides){
                targetPosition.x += (isLeft ? -offset.x : offset.x);
                targetPosition.y = mainCamera.transform.position.y;
            }else{
                targetPosition.x = mainCamera.transform.position.x;
                
                targetPosition.y += (isTop ? -offset.y : offset.y);
            }
            
            targetPosition.z = mainCamera.transform.position.z; // Keep the z-position constant

            // Adjust damping time dynamically
            //float dampingTime = isLeft ? smoothSpeed * 1.3f : smoothSpeed;

            if (isLeft){

                //Vector3 pos = Vector3.Lerp( mainCamera.transform.position, targetPosition, smoothSpeed);
                //mainCamera.transform.position = pos;
                mainCamera.transform.position = Vector3.SmoothDamp(
                mainCamera.transform.position,
                targetPosition,
                ref leftCurrentVelocity,
                smoothSpeed);

            }else{
                mainCamera.transform.position = Vector3.SmoothDamp(
                mainCamera.transform.position,
                targetPosition,
                ref rightCurrentVelocity,
                smoothSpeed);
            }
            // Smoothly transition to the target position using SmoothDamp
            
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            active = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            active = false;
        }
    }
}
