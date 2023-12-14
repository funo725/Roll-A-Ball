using System;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5; // The force added to the ball to move it.
    //[SerializeField]
    private bool useTorque = true; // Whether or not to use torque to move the ball.
    //[SerializeField]
    private float maxAngularVelocity = 25; // The maximum velocity the ball can rotate at.
    //[SerializeField]
    private float jumpPower = 2; // The force added to the ball when it jumps.

    private Vector2 movementVector; // a container for the input value vector from Input System
    private Vector3 move; // the world-relative desired move direction, calculated from the camForward and user input.
    private Transform cam; // A reference to the main camera in the scenes transform
    private Vector3 camForward; // The current forward direction of the camera
    private bool jump; // whether the jump button is currently pressed

    private const float k_GroundRayLength = 1f; // The length of the ray to check if the ball is grounded.
    private Rigidbody rb;

    private void Awake()
    {
        // get the transform of the main camera
        if (Camera.main != null)
        {
            cam = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning(
                "Warning: no main camera found. Ball needs a Camera tagged \"MainCamera\", for camera-relative controls.");
            // we use world-relative controls in this case, which may not be what the user wants, but hey, we warned them!
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Set the maximum angular velocity.
        GetComponent<Rigidbody>().maxAngularVelocity = maxAngularVelocity;
    }

    private void FixedUpdate()
    {
        // Call the Move function of the ball controller
        Move(move);
        Jump(jump);
        jump = false;
    }

    private void OnMove(InputValue movementValue)
    {
        movementVector = movementValue.Get<Vector2>();
        
        //// Get the axis and jump input.
        //float h = CrossPlatformInputManager.GetAxis("Horizontal");
        //float v = CrossPlatformInputManager.GetAxis("Vertical");
        //jump = CrossPlatformInputManager.GetButton("Jump");

        // calculate move direction
        if (cam != null)
        {
            // calculate camera relative direction to move:
            //camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
            camForward = Vector3.Scale(cam.forward, Vector3.right + Vector3.forward).normalized;
            move = (movementVector.y * camForward + movementVector.x * cam.right).normalized;
        }
        else
        {
            // we use world-relative directions in the case of no main camera
            move = (movementVector.y * Vector3.forward + movementVector.x * Vector3.right).normalized;
        }
    }

    public void Move(Vector3 moveDirection)
    {
        // If using torque to rotate the ball...
        if (useTorque)
        {
            // ... add torque around the axis defined by the move direction.
            rb.AddTorque(new Vector3(moveDirection.z, 0, -moveDirection.x) * speed);
        }
        else
        {
            // Otherwise add force in the move direction.
            rb.AddForce(moveDirection * speed);
        }
    }

    public void Jump(bool jump)
    {
        // If on the ground and jump is pressed...
        if (Physics.Raycast(transform.position, -Vector3.up, k_GroundRayLength) && jump)
        {
            // ... add force in upwards.
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }
}
