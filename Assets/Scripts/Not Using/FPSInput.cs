using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[AddComponentMenu("Control Script/FPS Input")]
public class FPSInput : MonoBehaviour
{
    // Start is called before the first frame update

    public float speed { get; set; }
    public float maxSpeed { get; set; }
    private float acceleration { get; set; }



    private CharacterController _charController;
    private float gravity = -9.8f;
    Camera cam;

    void Start()
    {
        speed= 5;
        maxSpeed= 100;
        acceleration = 20;

        _charController= GetComponent<CharacterController>();
        cam = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            speed = speed + acceleration*Time.deltaTime;
        }
        else if (speed > 5)
        {
            speed = speed - acceleration*4*Time.deltaTime;
        }
        if (speed < 5) { speed = 5; }
        if (speed > maxSpeed) { speed = maxSpeed; }

        float deltaX = Input.GetAxis("Horizontal") * speed;
        float deltaZ = Input.GetAxis("Vertical") * speed;
        
        Vector3 movement = new Vector3(deltaX, 0, deltaZ);
        //movement = Vector3.ClampMagnitude(movement, speed);
        movement.y = gravity;
        movement *= Time.deltaTime;
        movement=transform.TransformDirection(movement);
        _charController.Move(movement);
    }
}
