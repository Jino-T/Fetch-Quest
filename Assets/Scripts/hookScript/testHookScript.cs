using UnityEngine;

public class FollowMouseUntilClick : MonoBehaviour
{
    private bool isFollowingMouse = true;
    private Camera mainCamera;

    void Start()
    {
        // Get the main camera reference
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Check if the mouse button is pressed
        if (Input.GetMouseButtonDown(0)) // Left mouse button click
        {
            isFollowingMouse = false; // Stop following the mouse
        }

        // Check if the mouse button is released
        if (Input.GetMouseButtonUp(0) ) // Left mouse button release
        {
            isFollowingMouse = true; // Start following the mouse again
        }

        // Follow the mouse if the flag is set
        if (isFollowingMouse)
        {
            // Convert mouse position to world position
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = transform.position.z; // Keep the original z position

            // Set the object's position to the mouse's world position
            transform.position = mousePosition;
        }
    }
}
