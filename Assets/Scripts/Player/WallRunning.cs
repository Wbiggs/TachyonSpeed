using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask WhatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce;
    public float maxWallRunTime;
    private float wallRunTimer;

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    //private RaycastHit leftwallhit;
    //private RaycastHit rightwallhit;
    private RaycastHit frontWallhit;
    //private bool wallLeft;
    //private bool wallRight;
    private bool frontWall;

    [Header("References")]
    public Transform Orientation;
    public PlayerMovement pm;
    public Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if(pm.isWallRunning) WallRunningMovement();
        /*if (pm.wallrunning)
        {
            WallRunningMovement();
        }*/
    }

    private void CheckForWall()
    {
        frontWall = Physics.Raycast(transform.position, Orientation.forward, out frontWallhit, wallCheckDistance, WhatIsWall);
        //wallRight = Physics.Raycast(transform.position, Orientation.right, out rightwallhit, wallCheckDistance, whatIsWall);
        //wallLeft = Physics.Raycast(transform.position, -Orientation.right, out leftwallhit, wallCheckDistance, whatIsWall);

    }

    /*private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }*/

    private void StateMachine()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        if (frontWall)
        {
            //start wallrun
            StartWallRun();
        }
        else
        {
            StopWallRun();
        }
    }

    private void StartWallRun()
    {
        pm.isWallRunning = true;
    }
    private void WallRunningMovement()
    {
        if (verticalInput > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, pm.moveSpeed, rb.velocity.z);
            rb.useGravity = false;
        }
        /*rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        Vector3 wallNormal = frontWallhit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);*/
    }

    private void StopWallRun()
    {
        pm.isWallRunning = false;
        rb.useGravity=true;
    }

}
