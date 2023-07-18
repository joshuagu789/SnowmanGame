using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

/* 
 * First script created for this game
 */

public class ThirdPersonMovement : MonoBehaviour
{

    public CinemachineVirtualCamera firstPersonCam;
    public CharacterController controller;
    public Transform cam;
    public Animator animator;
    public Player player;

    // For ApplyGravity()
    public float gravity = -9.81f;
    [SerializeField] private float gravityMultiplier = 1.0f;
    private float velocity;

    // For ApplyMovement()
    private Vector3 moveDirection;
    private Vector3 verticalMovement;

    public float turnSmoothTime = 0.5f;
    float turnSmoothVelocity;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        //animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        ApplyMovement();
        ApplyGravity();
    }

    void ApplyMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // If object is moving
        if (direction.magnitude >= 0.1f && player.speed > 0)
        {
            // Angle object is facing                                                   taking into account camera's angle (so that player moves based on WASD AND camera angle)
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

            // Makes the object rotate smoother
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            // Rotating object (is disabled if first person view is enabled since 1st person has its own rotations)
            if (!firstPersonCam.enabled)
            {
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }

            // Moving object
            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * player.speed * Time.deltaTime);

            animator.SetBool("isMoving", true);
        } else
            animator.SetBool("isMoving", false);
    }

    void ApplyGravity()
    {
        // If object is standing over something solid 
        if (controller.isGrounded)
        {
            velocity = -1.0f;
        }
        else
        {
            // Making object accelerate when falling
            velocity += gravity * gravityMultiplier * Time.deltaTime;
        }

        verticalMovement.y = velocity;
        controller.Move(verticalMovement * Time.deltaTime);
    }

}
