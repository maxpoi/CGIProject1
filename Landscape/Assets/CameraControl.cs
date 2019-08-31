using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float sensitivity;
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    Vector3 camAngle;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //get camaeral view angle
        camAngle = transform.forward;

        //basic camera control according to the camera view angle, wasd as forward, left backward and right. Q as upward and E as downward
        if (Input.GetKey(KeyCode.D))
        {
            this.transform.localPosition += new Vector3(0.1f, 0.0f, 0.0f);
        }
        if (Input.GetKey(KeyCode.A))
        {
            camAngle.z = 0.0f;
            this.transform.localPosition -= new Vector3(0.1f, 0.0f, 0.0f);
        }
        if (Input.GetKey(KeyCode.S))
        {
            this.transform.localPosition -= camAngle / 10;
        }
        if (Input.GetKey(KeyCode.W))
        {
            this.transform.localPosition += camAngle / 10 ;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            this.transform.localPosition += new Vector3(0.0f, 0.1f, 0.0f);
        }
        if (Input.GetKey(KeyCode.E))
        {
            this.transform.localPosition -= new Vector3(0.0f, 0.1f, 0.0f);
        }

        //move camera angles with mouse input
        yaw += sensitivity * Input.GetAxis("Mouse X");
        pitch -= sensitivity * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
}
