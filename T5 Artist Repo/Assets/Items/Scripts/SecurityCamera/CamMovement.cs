using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMovement : MonoBehaviour
{
    private bool isBeingControlled;

    private Transform playerTransform;

    [SerializeField] Camera currentCam;

    
    [SerializeField] float distanceToActivate = 5f; // Distance that determines how close the player needs to be to activate the camera
                                                    // Could be made as a player attribute if the player has skill tree increment which
                                                    // increases the distance to activate the cameras

    public bool isMovable = true;

    // Movable Attributes
    public enum RotationAxes { XAndY = 0, X = 1, Y = 2 }
    public RotationAxes axes = RotationAxes.XAndY;
    public float sensitivityX = 0.5F;
    public float sensitivityY = 0.5F;

    // Lock X rotation
    public float minimumX = -60F;
    public float maximumX = 60F;

    // Lock Y rotation
    public float minimumY = -60F;
    public float maximumY = 60F;

    float rotationX = 0F;
    float rotationY = 0F;

    Quaternion currentRotation;

    // Could have UI Text to take control
    // public Text controlCameraText;
    // But since this would be a prefab, better the UI Text be adjusted within the Canvas Manager

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        currentCam.enabled = false;
        currentRotation = transform.localRotation;
        rotationX = currentRotation.eulerAngles.y;
        rotationY = currentRotation.eulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {
        // If player does not have control camera ability unlocked
        // return;

        // If player is in combat
        // return;

        // Preconditions to use : 
        // - Note that no two cameras should be closer to each other by the distanceToActivate
        // - Player should not be in combat

        if (Vector3.Distance(playerTransform.position, transform.position) <= distanceToActivate)
        {
            // Show UI Text to control Camera
            // Debug.Log("Show UI Text to control Camera");
            if(Input.GetKeyDown(KeyCode.E) && !isBeingControlled) // Needs to be remapped for InputController
            {
                // Show UI Text to take control
                Debug.Log("Update UI Text to deactivate control");
                ActivateCamera();
            }
            else if(Input.GetKeyDown(KeyCode.E) && isBeingControlled)
            {
                // Show UI Text to deactivate control
                Debug.Log("Update UI Text to activate control");
                DeactivateCamera();
            }

            // If player is in combat
            //  DeactivateCamera();
            //  Hide UI Text to take control
        }
        else
        {
            // Hide UI Text to take control
        }

        if (!isBeingControlled)
            return;

        // Control Camera Movement
        if(isMovable)
        {
            if (axes == RotationAxes.XAndY)
            {
                rotationX += Input.GetAxis("Horizontal") * sensitivityX;

                rotationY += Input.GetAxis("Vertical") * sensitivityY;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
                rotationX = Mathf.Clamp(rotationX, minimumX, maximumX);
                transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
            }
            else if (axes == RotationAxes.X)
            {
                rotationX += Input.GetAxis("Horizontal") * sensitivityX;
                rotationX = Mathf.Clamp(rotationX, minimumX, maximumX);
                
                transform.localEulerAngles = new Vector3(currentRotation.eulerAngles.x, rotationX, 0);
            }
            else
            {
                rotationY += Input.GetAxis("Vertical") * sensitivityY;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                transform.localEulerAngles = new Vector3(-rotationY, currentRotation.eulerAngles.y, 0);
            }
        }
    }

   

    public void ActivateCamera()
    {
        // Enable this camera
        isBeingControlled = true;
        currentCam.enabled = true;

        // Deactivate Player Control and Camera
        // Lerp/Animation to get to the camera
        playerTransform.GetComponent<PlayerController>().enableControl = false;
    }

    public void DeactivateCamera()
    {
        // Disable this camera
        isBeingControlled = false;
        currentCam.enabled = false;
        // Restore original rotation
        transform.localRotation = currentRotation;

        // Activate Player Control and Camera
        // Lerp/Animation to get to the player
        playerTransform.GetComponent<PlayerController>().enableControl = true;
    }
}
