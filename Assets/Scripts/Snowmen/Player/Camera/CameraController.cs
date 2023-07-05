using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera firstPersonCam;
    public CinemachineFreeLook thirdPersonCam;

    // Start is called before the first frame update
    void Start()
    {
        // Starting off in third person view
        firstPersonCam.enabled = false;
        thirdPersonCam.enabled = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            firstPersonCam.enabled = !firstPersonCam.enabled;
            thirdPersonCam.enabled = !thirdPersonCam.enabled;

            // To center cursor in middle of screen if first person view is enabled
            if (firstPersonCam.enabled)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
        }
    }
}
