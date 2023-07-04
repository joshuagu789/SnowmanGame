using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class FirstPersonView : MonoBehaviour
{
    public CinemachineVirtualCamera firstPersonCam;
    public Transform playerBody;
    public Transform cameraTarget;

    public float mouseSensitivity = 1000f;
    float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (firstPersonCam.enabled)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            // For moving camera vertically and preventing camera from over rotating
            xRotation = mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            // Setting boundaries of cameraTarget for how far it can go up and down
            if (xRotation >= 0f && cameraTarget.localPosition.y < 2.4f || xRotation <= 0f && cameraTarget.localPosition.y > 3.4f
                || cameraTarget.localPosition.y <= 3.4f && cameraTarget.localPosition.y >= 2.4f)
            {
                cameraTarget.Translate(0f, (xRotation * 2f * Time.deltaTime), 0f);
            }

            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}
