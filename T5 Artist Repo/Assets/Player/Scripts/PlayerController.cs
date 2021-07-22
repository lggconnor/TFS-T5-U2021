using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public bool isDebug = false;

    // the Character Controller of the player
    public Rigidbody _rb;

    // the Animator for the player
    public PlayerAnimationManager animManage;

    // A get/set for an instance of the character controller script
    public static PlayerController instance { get; set; } = null;

    public HealthManager playerHealth;

    // a class containing all of the movement variables. Could be a struct, but structs don't support default values
    [System.Serializable]
    public class MovementVariables
    {
        public bool smoothMotion = false;
        public float playerSpeed = 5f;
        public float jumpStrength = 5f;
        [Range(-100, 0)]
        public float gravity = -9.8f;
        public float DashDistance = 5f;
        public float dashInitialSpeed = 7f;
        public float dashFinalSpeed = 4f;
        public Vector3 Drag;
    }

    // Serialize field makes the private variable _moveVars visible in the inspector. 
    // moveVars has a get and set so it is accessible and changeable in other scripts (without having to have an explicit reference to the script)
    [SerializeField] MovementVariables _moveVars;
    public MovementVariables moveVars
    {
        get { return _moveVars; }
        set { _moveVars = moveVars; }
    }
    // Idk why this is still here, This should be removed (We're not using SimpleMove)
    enum ControllerType { Move, SimpleMove };
    [SerializeField] ControllerType type;

    [Space(10)]

    // If this script controls the camera- disable if another script manipulates the camera
    public bool scriptControlsCamera = true;

    // the type of camera- changes take effect on play
    enum CameraType { Perspective, Orthographic }
    [SerializeField] CameraType camType;

    // Determines whether the camera moves parallel to the viewing plane, or if the camera moves parallel to the player's XZ plane
    enum CameraMoveType { ParallelToView, ParallelToPlayer }
    [SerializeField] CameraMoveType moveType;

    // Determines the Camera Angle to be used. (Select Override to use the cameraViewAngleOverrideValue as the camera angle)
    enum CameraView { Dimetric, Isometric, A45Degrees, A60Degrees, Override };

    [SerializeField, Tooltip("Dimetric and Isometric have a camera angle of 30 and 35.264 degrees respectively from the xz plane")] CameraView view;

    // [HideInInspector]

    // the camera's rotation when the Override enum is selected
    public Vector3 cameraViewAngleOverrideValue = new Vector3(70f, 45f, 0f);

    // the camera's distance from the player. Useless when the camera is orthographic.
    public float cameraDistanceFromPlayer = 10f;
    // the offset of the camera in worldspace, calculated based on cameraDistanceFromPlayer
    private Vector3 offset;

    // A bool determining if the camera is offset to the midpoint of the player and the mouse
    public bool cameraOffsetByMouse = true;
    // the maximum offset of the camera by the mouse
    public float maxCameraOffset = 1f;

    // Whether to use LERP to smooth the camera
    public bool cameraSmoothing = false;
    // the time it takes for the camera to move towards its new position (The lower the faster)
    public float smoothSpeed = 0.2f;

    // Should the character be controlled or not
    [HideInInspector] public bool enableControl;

    // private bools to control jump, dash, shoot, and others.
    private bool jump = false;
    private bool dash = false;
    private float dashCurrentDistance = 0f;
    private float dashCurrentSpeed;
    private float dashAcceleration;
    private float dashCurrentTime = 0f;

    private bool isGrounded = true;

    // private variables containing input
    private Vector2 mousePos = Vector2.zero;
    private Vector2 moveInput = Vector2.zero;
    private bool isSneaking = true;

    private Thrower thrower;
    private Inventory inventory;

    // the direction the player is moving, calculated and used in the script
    Vector3 moveDirection;

    // the direction the player is looking, calculated and used in the script
    Vector3 lookDirection;

    [Space(10)]

    // transform for the spawn location of projectiles. 
    // this is a wierd way to do this if the shoot location is static, because it requires a dependancy apart from the script.
    public Transform projectileSpawn;

    // instead we could use a Vector3 that can be changed in the inspector, as well as from other scripts (as well as events in the animator)
    [SerializeField] Vector3 _shootPosition;
    public Vector3 shootPosition
    {
        get { return _shootPosition; }
        set { _shootPosition = shootPosition; }
    }
    // public Rigidbody projectilePrefab;

    private Vector3 camVelocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        // gets the character controller component from the scripts' parent object
        _rb = GetComponent<Rigidbody>();

        playerHealth = GetComponent<HealthManager>();

        thrower = GetComponent<Thrower>();

        inventory = GetComponent<Inventory>();

        enableControl = true;

        // speed = moveVars.playerSpeed;
        // jumpSpeed = moveVars.jumpStrength;

        // initialize the moveDirection vector
        moveDirection = Vector3.zero;

        // sets the camera to orthographic or perspective depending on the enum value in the inspector
        if (camType == CameraType.Orthographic)
        {
            Camera.main.orthographic = true;
        }
        else
        {
            Camera.main.orthographic = false;
        }

        // sets the camera's angle based on the camera angle specified in the inspector
        switch (view)
        {
            case CameraView.Isometric:
                Camera.main.transform.eulerAngles = new Vector3(Mathf.Atan(1 / Mathf.Sqrt(2)) * Mathf.PI, 45f, 0);
                break;

            case CameraView.Dimetric:
                Camera.main.transform.eulerAngles = new Vector3(30f, 45f, 0);
                break;

            case CameraView.A45Degrees:
                Camera.main.transform.eulerAngles = new Vector3(45f, 45f, 0);
                break;

            case CameraView.A60Degrees:
                Camera.main.transform.eulerAngles = new Vector3(60f, 45f, 0);
                break;

            case CameraView.Override:
                Camera.main.transform.eulerAngles = cameraViewAngleOverrideValue;
                break;

            default:
                Camera.main.transform.eulerAngles = new Vector3(Mathf.Atan(1 / Mathf.Sqrt(2)) * Mathf.PI, 45, 0);
                break;
        }

    }
    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position + Vector3.up, Vector3.down, 1.2f);

        // Debug.Log(isGrounded);

        // Necessary for times when player doesn't need to be controlled
        if (!enableControl)
            return;

        // mouseVector is the vector from the mouseposition on the screen to the player position on the screen
        Vector2 mouseVector;
        // lookVector is whe the player's forward vector will be rotated towards
        Vector3 lookVector = Vector3.zero;

        // If the player's camera is orthographic, then the camera calculates the vector to look towards using screen coordinates
        if (Camera.main.orthographic)
        {
            // if there is a projectile spawn point, then calculate the rotation of the player based on the y position of the player,
            // with an offset of the y of the spawn point
            if (projectileSpawn)
            {
                mouseVector = (Vector3) mousePos - Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, projectileSpawn.position.y, transform.position.z));
            }
            // else calculate the rotation based on the character's y position
            else
            {
                mouseVector = (Vector3) mousePos - Camera.main.WorldToScreenPoint(transform.position);
            }
            // make a normalized vector (probably unnecessary) from the player's position to the mouse position
            lookVector = new Vector3(mouseVector.x, 0, (mouseVector.y / (Mathf.Cos(Camera.main.transform.eulerAngles.y)))).normalized;
        }

        // if the camera is perspective, calculate the mouse position by projecting a ray onto a XZ plane at a specified y
        else
        {
            if (projectileSpawn)
            {
                lookVector = GetWorldPositionOnPlane((Vector3)mousePos + new Vector3(0, 0, cameraDistanceFromPlayer), projectileSpawn.position.y) - new Vector3(transform.position.x, projectileSpawn.position.y, transform.position.z);
            }
            else
            {
                lookVector = GetWorldPositionOnPlane((Vector3)mousePos + new Vector3(0, 0, cameraDistanceFromPlayer), transform.position.y) - transform.position;
            }
        }

        // this vector3 calculates the vector which translates the mousevector to a vector for the player to face in the direction of in 3d space

        lookDirection = lookVector;

        /// this uses quaternions to rotate the player so their vector3.forwards is the same direction as the vector, as well as shifts vector to be positioned relative to the camera
        /// rather than worldspace
        if (Camera.main.orthographic)
            transform.rotation = Quaternion.LookRotation(lookVector) * Quaternion.FromToRotation(Vector3.right, Camera.main.transform.right);
        // The vector already has the correct orientation, as it was calculated using world positions rather than screen positions
        else
            transform.rotation = Quaternion.LookRotation(lookVector);//transform.LookAt(new Vector3(lookVector.x, transform.position.y, lookVector.z));

        switch (type)
        {
            case ControllerType.Move:
                // A new vector3 to store the current y movement, so that CharacterController.isGrounded behaves nicely (seriously comment out this line and try jumping)
                // Vector3 downVel = new Vector3(0, moveDirection.y, 0);

                // checks that the player is grounded and not dashing before allowing the player to move their character
                if (isGrounded && !dash)
                {
                    /// Player input is fetched and stored in this variable, and is rotated so forward is relative to the camera.
                    /// Also remember Quaternion-Vector multiplication is not communative.
                    /// In this case Vector = Quaternion * Vector is okay, but Vector = Vector * Quaternion is not.
                    if (moveVars.smoothMotion)
                    {
                        moveDirection = Quaternion.FromToRotation(Vector3.right, Camera.main.transform.right) * new Vector3(moveInput.x, 0, moveInput.y);
                    }
                    else
                    {
                        moveDirection = Quaternion.FromToRotation(Vector3.right, Camera.main.transform.right) * new Vector3(moveInput.x, 0, moveInput.y).normalized;
                    }
                   

                    // multiply moveDirection by speed
                    moveDirection *= moveVars.playerSpeed;

                }
                // Probably going to be replaced by a rootmotion animation, but in case it isn't this is here. 
                // Was too complicated to calculate the non-constant acceleration given an initial and final velocity, a time, and a distance.
                // this one calculates the 
                else if (dash && dashCurrentDistance <= moveVars.DashDistance && moveDirection != Vector3.zero)
                {
                    Dash();
                }
                else
                    dash = false;

                // Make sure the Blendtree vars are based on the characters forward vector
                Vector3 blendVars = Quaternion.FromToRotation(transform.right, Camera.main.transform.right) * new Vector3(moveInput.x, 0, moveInput.y);

                // set the floats for blend tree
                if (animManage)
                    animManage.SetBlend(blendVars.x, blendVars.z);
                // Debug.Log(cc.isGrounded);
                // moveDirection.x /= 1 + moveVars.Drag.x * Time.deltaTime;
                // moveDirection.y /= 1 + moveVars.Drag.y * Time.deltaTime;
                // moveDirection.z /= 1 + moveVars.Drag.z * Time.deltaTime;

                // apply Gravity
                // moveDirection.y += moveVars.gravity * Time.deltaTime;
                break;
        }

        // _rb.AddForce(moveDirection * Time.deltaTime, ForceMode.VelocityChange);


    }

    private void FixedUpdate()
    {
        // Necessary for times when player doesn't need to be controlled
        if (!enableControl)
            return;
        // the Jump is here to ensure that the jump fires (Move() is in fixedupdate as well, and Movedirection is reset every update)
        if (jump)
        {
            moveDirection.y = moveVars.jumpStrength;
            jump = false;
        }


        _rb.velocity = new Vector3(moveDirection.x, _rb.velocity.y, moveDirection.z);

        // _rb.AddForce(moveDirection * 2, ForceMode.Force);

        // Move() calls the charactercontroller Move() function.
         // * Time.fixedDeltaTime;

    }

    // If there is no camera smoothing, the camera position is updated in LateUpdate (called after update, and more often than FixedUpdate)
    void LateUpdate()
    {
        CameraMove();
    }

    // A function that moves the camera according to the parameters set in the inspector
    void CameraMove()
    {
        // first checks if the script controls the camera (bool and check can most likely be removed for optimization)
        if (scriptControlsCamera)
        {
            // checks to make sure the look direction vector calculated in update isn't a zero vector
            if (lookDirection != Vector3.zero)
            {
                // calculates the offset of the camera in worldspace based on the distance specified in the inspector
                offset = Quaternion.LookRotation(Camera.main.transform.forward) * new Vector3(0, 0, -cameraDistanceFromPlayer);

                // a vector3 to store the mouse position
                Vector3 mousePos;

                // checks if the camera is orthographic, and if so calculates the mouse position by getting the world position of the mouse
                // and subtracting the position of the camera
                if (Camera.main.orthographic)
                    mousePos = Camera.main.ScreenToWorldPoint((Vector3)this.mousePos) - Camera.main.transform.position;

                // if the movement type is parallel to view, the mouse position is calculated by getting the world position of 
                // the mouse position at the specified z (the camera's distance from the player), adding the offset, and subtracting the position of the camera
                else if (moveType == CameraMoveType.ParallelToView)
                    mousePos = Camera.main.ScreenToWorldPoint((Vector3)this.mousePos + new Vector3(0, 0, cameraDistanceFromPlayer)) + offset - Camera.main.transform.position;

                // in every other case (if the camera is perspective, and the move type is parallel to a XZ plane at the y of the player)
                // the mouse position is calculated by getting the mouses' worldPosition on the XZ plane at the player's y, adding the offset and subtracting the camera position
                else 
                {
                    mousePos = GetWorldPositionOnPlane((Vector3)this.mousePos + new Vector3(0, 0, cameraDistanceFromPlayer), transform.position.y) + offset - Camera.main.transform.position;
                }

                // here we add the player's position + the offset determined earlier to calculate the worldspace of the camera.
                Vector3 cameraPos = transform.position + offset;

                // if the camera is offset by the mouse, camera position has the offset (limited by a clamp) added to it

                /*TODO - ADD CHECK TO SEE IF BUTTON IS DOWN*/
                if (cameraOffsetByMouse)
                {
                    // Debug.Log(mousePos);
                    cameraPos += Vector3.ClampMagnitude(mousePos, maxCameraOffset);
                }

                // if camera smoothing is desired, the camera is smoothed with linear interpolation between the camera's position, and the value of cameraPos, 
                // over time specified by smoothSpeed.
                if (cameraSmoothing)
                    cameraPos = Vector3.SmoothDamp(Camera.main.transform.position, cameraPos, ref camVelocity, smoothSpeed * Time.deltaTime);

                // finally we set the camera's position to the calculated value, cameraPos.
                Camera.main.transform.position = cameraPos;

            }
        }
    }


    // GetWorldPositionOnPlane takes a screen position as a Vector3 of which to cast a ray from the camera through that point, 
    // and a value y at which to create a new XZ plane to get a point on 
    public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float y)
    {
        // a Ray is created (from the camera, through the point defined by 'screenPosition') using ScreenPointToRay
        // and stored in the Ray var 'ray'
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        // a new XZ plane at the specified 'y' is created and stored in the Plane var 'xz'
        Plane xz = new Plane(Vector3.up, new Vector3(0, y, 0));

        // a float variable named distance is declared
        float distance;

        // a raycast on the Plane 'xz' using the Ray 'ray' and outputting a distance from the rays source to the plane into distance
        xz.Raycast(ray, out distance);

        // returns a point at Distance 'distance' along the Ray 'ray'
        return ray.GetPoint(distance);
    }

    // returns a Vector3 at a point on the floor. Will need to be modified to use layermasks
    public Vector3 GetMousePositionOnFloor()
    {
        // 'mouseRay' is the Ray from the camera through a point on the screen a specified distance forward
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition + new Vector3(0, 0, cameraDistanceFromPlayer));
        
        // 'distance' is the distance along the ray mouseRay
        float distance = 0f;

        // 'newHit' is the hit information of a raycast
        RaycastHit newHit;

        // this is a raycast that is from the camera, in a direction of the ray 'mouseRay'.
        if (Physics.Raycast(Camera.main.transform.position, mouseRay.direction, out newHit, 50))
        {
            distance = newHit.distance;
        }

        // Debug.Log(distance);
        // Debug.Log(newMousePos);

        // returns the point at Distance 'distance' along the Ray 'mouseRay'
        return mouseRay.GetPoint(distance);
    }

    public void TakeDamage(float damage)
    {
        if (playerHealth)
        {
            if (playerHealth.Damage(damage))
            {
                Death();
            }
        }
    }

    void Death()
    {
        Debug.Log("Oof Death");
        Destroy(gameObject);
        SceneManager.LoadScene("GameOver");
    }

    // Input action functions

    public void OnMove(InputValue input)
    {
        Vector2 inputVector = input.Get<Vector2>();

        // if (isDebug) print(inputVector);
        
        // assign input vector to variable for access in update
        // TODO have player input states
        moveInput = inputVector;
    }

    public void OnDash()
    {
        // set Dash bool for access in update
        dash = true;
    }

    public void OnShoot(InputValue input)
    {
        /*if (isDebug)
            print("On Shoot Input : " + input.isPressed);*/
        // call shoot function from Thrower script
        
        thrower.isThrowing = input.isPressed;
       
            
    }



    public void OnWeaponScroll(InputValue input)
    {
        float scrollDir = input.Get<float>();

        // call weapon swap if float != 0

        if (scrollDir != 0)
        {
            // call weaponswap and pass in scroll dir
             if (scrollDir > 0) 
            {
                // call weapon swap up
               /* if (isDebug)
                    Debug.Log("Swap Up");*/

                inventory.WeaponScrollUp();
            }
            else
            {
                // call weapon swap down
                /*if (isDebug)
                    Debug.Log("Swap Down");*/

                inventory.WeaponScrollDown();
            }
            
        }
    }

    public void OnMousePos(InputValue input)
    {
        Vector2 mousePos = input.Get<Vector2>();

        // if (isDebug) print(mousePos);

        // assign mouse pos to variable for access in update
        this.mousePos = mousePos;
    }

    public void OnSneak(InputValue input)
    {
        isSneaking = input.Get<float>() == 0f ? false : true;
    }

    private void Dash()
    {
        float dashDuration = moveVars.DashDistance / moveVars.playerSpeed;
        // can probably be moved to start
        float distanceDifference = Mathf.Abs(moveVars.dashInitialSpeed - moveVars.dashFinalSpeed);
        Debug.Log(distanceDifference);
        float initialDistanceDifference = Mathf.Abs(moveVars.dashInitialSpeed - moveVars.playerSpeed);
        Debug.Log(initialDistanceDifference);
        float finalDistanceDifference = Mathf.Abs(moveVars.playerSpeed - moveVars.dashFinalSpeed);
        Debug.Log(finalDistanceDifference);
        float ratioOne = initialDistanceDifference / distanceDifference;
        float ratioTwo = finalDistanceDifference / distanceDifference;
        float dashAccelerationOne = (moveVars.playerSpeed - moveVars.dashInitialSpeed) / (dashDuration * ratioTwo);
        float dashAccelerationTwo = (moveVars.dashFinalSpeed - moveVars.playerSpeed) / (dashDuration * ratioOne);
        float dashCurrentSpeed = 0f;
        if (dashCurrentTime < (ratioTwo * dashDuration))
        {
            dashCurrentSpeed = moveVars.dashInitialSpeed + (dashAccelerationOne * dashCurrentTime);
        }
        else
        {
            dashCurrentSpeed = moveVars.dashInitialSpeed + (dashAccelerationOne * ratioTwo * dashDuration) + (dashAccelerationTwo * (dashCurrentTime - ratioTwo * dashDuration));
        }

        Debug.Log(ratioOne);
        Debug.Log(ratioTwo);
        Debug.Log(dashAccelerationOne);
        Debug.Log(dashAccelerationTwo);

        moveDirection = new Vector3(moveDirection.x, 0, moveDirection.z);


        // the instantaneous velocity of the dash at a certain distance
        Debug.Log(dashCurrentSpeed);
        dashCurrentDistance += (dashCurrentSpeed * Time.deltaTime);
        dashCurrentTime += Time.deltaTime;

        Debug.Log(dashCurrentDistance);

        moveDirection = moveDirection.normalized * dashCurrentSpeed;
        Debug.Log(moveDirection);

        if (dashCurrentDistance >= moveVars.DashDistance)
        {
            dashCurrentTime = 0f;
            dashCurrentDistance = 0f;
            dash = false;
        }
    }

    public void ToggleInput(bool isInput = true)
    {
        enableControl = isInput;
        scriptControlsCamera = isInput;
        Time.timeScale = isInput ? 1f : 0f;
    }


}
