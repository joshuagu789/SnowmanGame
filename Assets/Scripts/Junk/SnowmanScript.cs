using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowmanScript : MonoBehaviour
{

    public Rigidbody rbody;
    public int speed = 10;

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        rbody.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        //WASD movement
        if (Input.GetKey(KeyCode.W))
            rbody.AddForce(0, 0, speed);
        if (Input.GetKey(KeyCode.A))
            rbody.AddForce(-speed, 0, 0);
        if (Input.GetKey(KeyCode.S))
            rbody.AddForce(0, 0, -speed);
        if (Input.GetKey(KeyCode.D))
            rbody.AddForce(speed, 0, 0);
    }
}
