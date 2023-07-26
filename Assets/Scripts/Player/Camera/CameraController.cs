using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine; // Needed for Cinemachine cameras

/*
 * Script for controlling which camera is currently active
 *  - will later build on this by adding cutscenes & menu screens
 */

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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            firstPersonCam.enabled = !firstPersonCam.enabled;
            thirdPersonCam.enabled = !thirdPersonCam.enabled;
        }
    }
}
