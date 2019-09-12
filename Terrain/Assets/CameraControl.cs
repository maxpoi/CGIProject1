using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float sensitivity = 1;
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
        //get camaeral view angle and the normal vector of it
        camAngle = transform.forward;
        Vector3 normal = Vector3.Cross(camAngle, new Vector3(0.0f, 0.1f, 0.0f));

        //basic camera control according to the camera view angle, wasd as forward, left backward and right. Q as upward and E as downward
        if (Input.GetKey(KeyCode.D))
        {
            this.transform.localPosition -= normal /10 ;
        }
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.localPosition += normal /10 ;
        }
        if (Input.GetKey(KeyCode.S))
        {
            this.transform.localPosition -= camAngle / 100;
        }
        if (Input.GetKey(KeyCode.W))
        {
            this.transform.localPosition += camAngle / 100 ;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            this.transform.localPosition += new Vector3(0.0f, 0.01f, 0.0f);
        }
        if (Input.GetKey(KeyCode.E))
        {
            this.transform.localPosition -= new Vector3(0.0f, 0.01f, 0.0f);
        }

        //move camera angles with mouse input
        yaw += sensitivity * Input.GetAxis("Mouse X");
        pitch -= sensitivity * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);


        Vector3 position = transform.position;
        Vector3 newPosition = new Vector3(0.0f, 0.0f, 0.0f);

        // camera collision detection
        RaycastHit hit;
        if (Physics.Linecast(position + Vector3.up, position, out hit))
        {
            string name = hit.collider.gameObject.tag;
            if (name != "Main Camera")
            {
                //if hits not camera, calculate the distance
                float currentDistance = Vector3.Distance(hit.point, position);
                //prevent camera go through terrain
                if (currentDistance < 1)
                {
                    newPosition = position;
                    newPosition = hit.point;
                    transform.position = newPosition;
                }
            }
        }

        //prevent camera go through scence
        if(position.x > 10)
        {
            newPosition = position;
            newPosition.x = 10;
            transform.position = newPosition;
        }
        if (position.x < -10)
        {
            newPosition = position;
            newPosition.x = -10;
            transform.position = newPosition;
        }
        if (position.y > 10)
        {
            newPosition = position;
            newPosition.y = 10;
            transform.position = newPosition;
        }
        if (position.y < -10)
        {
            newPosition = position;
            newPosition.y = -10;
            transform.position = newPosition;
        }
        if (position.z > 10)
        {
            newPosition = position;
            newPosition.z = 10;
            transform.position = newPosition;
        }
        if (position.z < -10)
        {
            newPosition = position;
            newPosition.z = -10;
            transform.position = newPosition;
        }


    }
}
