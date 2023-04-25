using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Rigidbody rb;
    public LayerMask whatIsWall;

    [Header("Climbing")]
    public float climbSpeed;
    public float maxClimbTime;
    private float climbTimer;
    public PlayerMovement pm;
    private bool isClimbing;

    [Header("Detection")]
    public float detectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool isWallInFront;

    private void Update()
    {
        WallCheck();
        StateMachine();
        if(isClimbing) ClimbMovement();
    }

    private void StateMachine()
    {
        if(isWallInFront && Input.GetKey(KeyCode.W) /*&& wallLookAngle<maxWallLookAngle*/)
        {
            if (!isClimbing) StartClimbing();
        }
        else
        {
            if(isClimbing) StopClimbing();
        }
    }

    private void WallCheck()
    {
        isWallInFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, whatIsWall);
        //wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal); //wall look angle prevents a shallow wall climb
        if (pm.isGrounded) climbTimer = maxClimbTime;

    }

    private void StartClimbing()
    {
        isClimbing= true;
    }
    private void ClimbMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x,climbSpeed,rb.velocity.z);
        //rb.AddForce() might make is smoother

    }
    private void StopClimbing()
    {
        isClimbing= false;
    }

}
