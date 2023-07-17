using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySearchLight : MonoBehaviour
{
    public Entity entity;
    float defaultX;
    float defaultY;
    float defaultZ;

    // Start is called before the first frame update
    void Start()
    {
        defaultX = transform.localEulerAngles.x;
        defaultY = transform.localEulerAngles.y;
        defaultZ = transform.localEulerAngles.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (entity.isLockedOn)
        {
            // Swivelling search light to face target
            var targetRotation = Quaternion.LookRotation(entity.target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2 * Time.deltaTime);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(defaultX, defaultY, defaultZ);
        }
    }
}
