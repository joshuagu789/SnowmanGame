using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{
    public CinemachineVirtualCamera firstPersonCam;
    public float range = 100f;
    public Transform targetLock;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2") && firstPersonCam.enabled)
        {
            // Locking on target if it's in range
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // If the has hit an enemy
            if (Physics.Raycast(ray, out hit, range) && hit.collider.gameObject.CompareTag("Enemy"))
            {
                Debug.Log(hit.collider.gameObject);
                var target = Instantiate(targetLock, hit.collider.transform.position, hit.collider.transform.localRotation, hit.collider.transform);
            }

        }
    }
}
