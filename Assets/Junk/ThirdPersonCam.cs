using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{

    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rbody;

    public float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        //Locks Cursor in Middle
        Cursor.lockState = CursorLockMode.Locked;
        //To Make Cursor Invisible:
        //Cursor.visible = false;

    }

    // Update is called once per frame
    void Update()
    {
        //TESTS: REPLACED orientation.forward with orientation.up?


        //Rotate orientation                                                                        swapped z and y here to test?
        Vector3 viewDirection = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDirection.normalized;

        //Rotate Player Object
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (inputDirection != Vector3.zero)
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDirection.normalized, Time.deltaTime * rotationSpeed);
    }
}
