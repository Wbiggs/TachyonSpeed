using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask whatIsWall;
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
    private Transform Orientation;
    private PlayerMovement pm;
    private Rigidbody rb;

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
        /*if (pm.wallrunning)
        {
            WallRunningMovement();
        }*/
    }

    private void CheckForWall()
    {
        frontWall = Physics.Raycast(transform.position, Orientation.forward, out frontWallhit, wallCheckDistance, whatIsWall);
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
        //pm.wallrunning = true;
    }
    private void WallRunningMovement()
    {
        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        Vector3 wallNormal = frontWallhit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);
    }

    private void StopWallRun()
    {
        //pm.wallrunning = false;
    }

}
