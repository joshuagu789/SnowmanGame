/*
 * Script for the player and their allies to tell their location and direction
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Compass : MonoBehaviour
{
    [SerializeField]
    private Transform playerCamera;
    [SerializeField]
    private TextMeshProUGUI degreesBox;    

    // Update is called once per frame
    void Update()
    {
        degreesBox.SetText("" + (int) playerCamera.eulerAngles.y);
    }

    // Returns the degrees at which the player has to face on the compass to face the Transform parameter
    public float GetDirection(Transform target, Transform origin)
    {
        var targetPos = new Vector3(target.position.x, 0f, target.position.z);
        var originPos = new Vector3(origin.position.x, 0f, origin.position.z);

        var fromVector = Vector3.forward;
        var toVector = (targetPos - originPos).normalized;

        var angleFromNorth = Vector3.Angle(fromVector, toVector);   // Vector3.Angle can only return 0-180 degrees but compass should be 0-360

        if (target.position.x - origin.position.x < 0)  // Setting angle to 180-360 if target is to the left of origin
            angleFromNorth = 360f - angleFromNorth;

        return angleFromNorth;
    }
}
