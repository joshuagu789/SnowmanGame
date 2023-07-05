using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{
    public CinemachineVirtualCamera firstPersonCam;
    public Transform targetLock;

    public float range = 100f;
    bool isLockedOn = false;
    int numLocks = 0;

    Transform target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckLockState();
    }

    void CheckLockState()
    {
        if (Input.GetButtonDown("Fire2") && firstPersonCam.enabled)
        {
            // Locking on target if it's in range
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // If the ray has hit an enemy within range
            if (Physics.Raycast(ray, out hit, range) && hit.collider.gameObject.CompareTag("Enemy"))
            {
                if (isLockedOn)
                {
                    Destroy(target.gameObject);
                }
                target = Instantiate(targetLock, hit.collider.transform.position, hit.collider.transform.localRotation, hit.collider.transform);
                target.localScale = hit.collider.transform.localScale;
                isLockedOn = true;
            }

        }
    }
}
