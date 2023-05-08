using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float maxSpeed;
    public float accelerationSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool isReadyToJump;
    private float gravityWeight;
    public float gravityMax;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("MovementStates")]
    public bool isWallRunning;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool isGrounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        isReadyToJump = true;
    }

    private void Update()
    {
        // ground check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        //increases max speed and decreases it
        /*if (Input.GetKey(KeyCode.W))
        {
            moveSpeed += accelerationSpeed * Time.deltaTime;
        }
        else if (moveSpeed > 5)
        {
            moveSpeed -= accelerationSpeed * 4 * Time.deltaTime;
        }
        if (moveSpeed < 5) { moveSpeed = 5; }
        if (moveSpeed > maxSpeed) { moveSpeed = maxSpeed; }*/
        MyInput();
        PlayerSpeedUP();
        

        // handle drag
        if (isGrounded)
        {
            SpeedControl();
            rb.drag = groundDrag;
            gravityWeight = 1;

        }
        else //if(!isGrounded && !wallRunning) 
        {
            rb.drag = 0;
            rb.AddForce(-transform.up * gravityWeight, ForceMode.Force); //pulls character down faster
            gravityWeight++;
            if (gravityWeight>gravityMax) gravityWeight= gravityMax; //this is a medium pull cap,
        }
        /*else if (!isGrounded && wallRunning) 
        {

        }*/
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }


    private void MyInput()
    {
        if (isGrounded)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");
        }

        // when to jump
        if (Input.GetKey(jumpKey) && isReadyToJump && isGrounded)
        {
            isReadyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }


    //looks at rigidbody maginitude and increases current running force or decreases it
    private void PlayerSpeedUP() 
    {
        if(rb.velocity.magnitude>=moveSpeed) moveSpeed += accelerationSpeed*Time.deltaTime;
        else moveSpeed-=accelerationSpeed*8* Time.deltaTime;
        if(moveSpeed>maxSpeed) moveSpeed = maxSpeed;
        if(moveSpeed<5) moveSpeed = 5;

    }
    private void MovePlayer()
    {
        // calculate movement direction

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            //rb.velocity=Vector3.ClampMagnitude(rb.velocity, moveSpeed);
        }

        // in air
        /*else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }*/

    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
   

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        isReadyToJump = true;
    }
}

/*
 * using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementGravityTest : MonoBehaviour
{
    public Rigidbody rb;
    private Transform wall;

    public float moveSpeed;
    public float gravity = 12f;
    public float gravityRotationSpeed = 20f;
    public float detectRadius = 10f;
    public LayerMask WhatIsWall;
    public Transform[] GroundChecks;

    private Vector3 floorAngle;

    private float horizontalInput;
    private float verticalInput;

    void Start()
    {
        rb= GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        MyInput();
        floorAngle=FloorAngleCheck();
        RotateSelf();
        
    }

    private void FixedUpdate()
    {
        wallDetect();
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        Vector3 moveDirection = (transform.forward * verticalInput + transform.right * horizontalInput).normalized;
        rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Force);
        RotateSelf();
        rb.AddForce(wall.position - transform.position, ForceMode.Force);
        Debug.DrawRay(transform.position, wall.position-transform.position,Color.green,5f);


    }

    private void RotateSelf()
    {
        Vector3 LerpDir = Vector3.Lerp(transform.up, floorAngle, Time.deltaTime * gravity);
        transform.rotation = Quaternion.FromToRotation(transform.up, LerpDir) * transform.rotation;
    }

    private void wallDetect()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectRadius, WhatIsWall);
        Debug.Log(hitColliders[0]);
        if (hitColliders.Length > 0)
        {
            wall = hitColliders[0].transform;
        }
    }

    Vector3 FloorAngleCheck()
    {
        RaycastHit HitFront;
        RaycastHit HitCentre;
        RaycastHit HitBack;

        Physics.Raycast(GroundChecks[0].position, -GroundChecks[0].transform.up, out HitFront, 10f, WhatIsWall);
        Physics.Raycast(GroundChecks[1].position, -GroundChecks[1].transform.up, out HitCentre, 10f, WhatIsWall);
        Physics.Raycast(GroundChecks[2].position, -GroundChecks[2].transform.up, out HitBack, 10f, WhatIsWall);

        Vector3 HitDir = transform.up;

        if (HitFront.transform != null)
        {
            HitDir += HitFront.normal;
        }
        if (HitCentre.transform != null)
        {
            HitDir += HitCentre.normal;
        }
        if (HitBack.transform != null)
        {
            HitDir += HitBack.normal;
        }

        Debug.DrawLine(transform.position, transform.position + (HitDir.normalized * 5f), Color.red);

        return HitDir.normalized;
    }
}
*/