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
    private Transform north;    // To find the location of the game object representing north (0 degrees)
    [SerializeField]
    private TextMeshProUGUI degreesBox;    

    // Update is called once per frame
    void Update()
    {
        north.position = Vector3.forward + playerCamera.position;
        degreesBox.SetText("" + (int) GetDirection(playerCamera));
    }

    // Returns the degrees at which the player has to face on the compass to face the Transform parameter
    public float GetDirection(Transform character)
    {
        var angleFromNorth = Vector3.Angle(north.forward, character.forward);   // Vector3.Angle can only return 0-180 degrees but compass should be 0-360

        // Comparing character's x position in relation to north's position to see if it's left or right of north (left means 180-360 degrees and right means 0-180 degrees) 
        Vector3 characterLocalPos = north.InverseTransformPoint(character.forward);    // Converting character's position to local in relation to north to better compare left or right

        if (characterLocalPos.x < 0f)
            angleFromNorth = 360f - angleFromNorth;

        return angleFromNorth;
    }
}
