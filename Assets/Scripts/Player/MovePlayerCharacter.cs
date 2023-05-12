using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerCharacter : MonoBehaviour
{
    public Transform[] GroundChecks;
    public Transform player;
    public Rigidbody rb;
    public Transform orientation;
    public LayerMask WhatIsWall;
    public LayerMask WhatIsGround;

    public float minSpeed;
    public float moveSpeed;
    public float maxSpeed;
    public float accelerationSpeed;
    public float gravity = 12f;
    public float maxGravity = 12f;
    public float gravityRotationSpeed = 20f;
    public float detectRadius = 2;
    public float playerHeight;

    private float horizontalInput;
    private float verticalInput;

    private Transform wall;
    private Vector3 floorAngle;

    public enum PlayerState
    {
        Grounded,
        OnWall,
        InAir,
    }
    public PlayerState state;

    private bool isOnGround;
    private bool isOnWall;

    // Start is called before the first frame update
    void Start()
    {
        rb= GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        MyInput();
        floorAngle = FloorAngleCheck();
        //RotateSelf();
        PlayerSpeedUP();
        SpeedControl();

    }

    private void FixedUpdate()
    {
        //looks straight down to see if ground is down there
        isOnGround = Physics.Raycast(transform.position, -orientation.up, playerHeight * .5f + .3f, WhatIsGround);
        isPlayerOnWall();
        // radius detect for whatiswall       
        MovePlayer();
        RotateSelf();
        rb.AddForce(-orientation.up * gravity*5f, ForceMode.Force);

        /*if (Physics.Raycast(orientation.position, -orientation.up, playerHeight * .5f + .3f, WhatIsGround))
        {
            state= PlayerState.Grounded;
            wallDetect(WhatIsGround);
        }
        if (Physics.Raycast(transform.position, transform.forward, 2f, WhatIsWall))
        {
            state = PlayerState.OnWall;
            wallDetect(WhatIsWall);
            rb.AddForce(wall.position - transform.position, ForceMode.Force);
        }
        MovePlayer();
        rb.AddForce(-orientation.up * gravity, ForceMode.Force);*/
    }

    void OnDrawGizmosSelected()
    {
        Vector3 inFrontOfPlayer = new Vector3(player.position.x, player.position.y, player.position.z+1.1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(inFrontOfPlayer, 1.6f);
    }
    private void isPlayerOnWall()
    {
        //WORKING ON THIS/////////////// casting a tetection sphere in front of player also OnDrawGizmosSelected
        Vector3 inFrontOfPlayer = new Vector3(player.position.x, player.position.y, player.position.z + 1.1f);

        Collider[] wallWalking = Physics.OverlapSphere(inFrontOfPlayer, 2, WhatIsWall);
        //if wall detected, make it true
        if (wallWalking.Length > 0)
        {
            isOnWall = true;
        }
        else isOnWall = false;

        if (isOnGround && !isOnWall)
        {
            state = PlayerState.Grounded;
            wallDetect(WhatIsGround);
            //Debug.Log("grounded");

        }
        else if (isOnWall)
        {
            //var vel=rb.velocity; //
            //float rbSpeed = vel.magnitude;//
            //float newMoveSpeed = moveSpeed * moveSpeed;
            //Debug.Log("rb speed " + rbSpeed);
            //float holdToWall = (moveSpeed / maxSpeed) * (newMoveSpeed * 10); //holds player to wall the faster they move
            //Debug.Log("hold to wall " + newMoveSpeed);
            state = PlayerState.OnWall;
            wallDetect(WhatIsWall);
            //Vector3 pullForce= new Vector3(wall.position.x-transform.position.x, rb.transform.position.y, wall.position.z - transform.position.z);
            rb.AddForce((wall.position - transform.position) * moveSpeed, ForceMode.Force); //pulls toward wall object
            Debug.Log("wall");
        }
        else
        {
            state = PlayerState.InAir;
            Debug.Log("air");
        }
    }

    private void MyInput()
    {
        if (state== PlayerState.Grounded || state == PlayerState.OnWall)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");
        }
        /*/ when to jump
        if (Input.GetKey(jumpKey) && isReadyToJump && isGrounded)
        {
            isReadyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }*/
    }

    private void MovePlayer()
    {
        if (state == PlayerState.Grounded)
        {
            Vector3 moveDirection = (orientation.forward * verticalInput + orientation.right * horizontalInput).normalized;
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            //rb.velocity= moveDirection.normalized*moveSpeed*3f;
        }
        else 
        {
            Vector3 moveDirection = (transform.forward * verticalInput + transform.right * horizontalInput).normalized;
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            //rb.velocity = moveDirection.normalized * moveSpeed * 3f;

        }
        //RotateSelf();
        //rb.AddForce(wall.position - transform.position, ForceMode.Force);
        //Debug.DrawRay(transform.position, wall.position - transform.position, Color.green, 5f);
    }

    private void PlayerSpeedUP()
    {
        //if rb is moving at moveSpeed, bump move speed up by acceleration speed. else bump down move speed.  Don't let moveSpeed exceed maxSpeed.  
        if (rb.velocity.magnitude >= moveSpeed) moveSpeed += accelerationSpeed * Time.deltaTime;
        else moveSpeed -= accelerationSpeed * 8 * Time.deltaTime;
        if (moveSpeed > maxSpeed) moveSpeed = maxSpeed;
        if (moveSpeed < 5) moveSpeed = minSpeed;

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

    private void RotateSelf()
    {
        Vector3 LerpDir = Vector3.Lerp(transform.up, floorAngle, Time.deltaTime * gravity);
        transform.rotation = Quaternion.FromToRotation(transform.up, LerpDir) * transform.rotation;
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

        Debug.DrawLine(transform.position, transform.position + (HitDir.normalized * 5f), Color.red,5f);


        return HitDir.normalized;
    }

    private void wallDetect(LayerMask beneathFeet)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, playerHeight * .5f + .3f, beneathFeet);
        //Debug.Log(hitColliders[0]);
        if (hitColliders.Length > 0)
        {
            wall = hitColliders[0].transform;
        }
        /*int items = 0;
        foreach(Collider hit in hitColliders) 
        {
            Debug.Log(items+" "+hit);
            items++;
        }
        Debug.Log(hitColliders.Length);*/
    }


}

/*using System.Collections;
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
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        MyInput();
        floorAngle = FloorAngleCheck();
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
        Debug.DrawRay(transform.position, wall.position - transform.position, Color.green, 5f);


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
    }*/
////////////////////////////////////////////////////
/*
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerCharacter : MonoBehaviour
{
    public Transform[] GroundChecks;
    public Rigidbody rb;
    public Transform orientation;
    public LayerMask WhatIsWall;
    public LayerMask WhatIsGround;

    public float moveSpeed;
    public float maxSpeed;
    public float accelerationSpeed;
    public float gravity = 12f;
    public float maxGravity = 12f;
    public float gravityRotationSpeed = 20f;
    public float detectRadius = 2;
    public float playerHeight;

    private float horizontalInput;
    private float verticalInput;

    private Transform wall;
    private Vector3 floorAngle;

    public enum PlayerState
    {
        Grounded,
        OnWall,
        InAir,
    }
    public PlayerState state;
    private bool isOnGround;
    private bool isOnWall;
    private bool isInAir;

    // Start is called before the first frame update
    void Start()
    {
        rb= GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        MyInput();
        floorAngle = FloorAngleCheck();
        //RotateSelf();
        PlayerSpeedUP();
        SpeedControl();

    }

    private void FixedUpdate()
    {
        //looks straight down to see if ground is down there
        isOnGround=Physics.Raycast(transform.position, -orientation.up, playerHeight * .5f + .3f, WhatIsGround);
        
        // radius detect for whatiswall
        Collider[] wallWalking = Physics.OverlapSphere(transform.position, 2, WhatIsWall); 
        //if wall detected, make it true
        if(wallWalking.Length> 0)
        {
            isOnWall=true;
        }
        else isOnWall=false;

        if (isOnGround && !isOnWall)
        {
            state= PlayerState.Grounded;
            wallDetect(WhatIsGround);
            Debug.Log("grounded");

        }
        else if (isOnWall)
        {
            state = PlayerState.OnWall;
            wallDetect(WhatIsWall);
            rb.AddForce(wall.position - transform.position*10f, ForceMode.Force);
            Debug.Log("wall");
        }
        else
        {
            state = PlayerState.InAir;
            Debug.Log("air");
        }
        MovePlayer();
        RotateSelf();
        rb.AddForce(-orientation.up * gravity, ForceMode.Force);
    }

    private void MyInput()
    {
        if (state== PlayerState.Grounded || state == PlayerState.OnWall)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");
        }

        
when to jump
        if (Input.GetKey(jumpKey) && isReadyToJump && isGrounded)
        {
    isReadyToJump = false;

    Jump();

    Invoke(nameof(ResetJump), jumpCooldown);
}

    }

    private void MovePlayer()
{
    if (state == PlayerState.Grounded)
    {
        Vector3 moveDirection = (orientation.forward * verticalInput + orientation.right * horizontalInput).normalized;
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }
    else if (state == PlayerState.OnWall)
    {
        Vector3 moveDirection = (transform.forward * verticalInput + transform.right * horizontalInput).normalized;
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    //rb.AddForce(wall.position - transform.position, ForceMode.Force);
    //Debug.DrawRay(transform.position, wall.position - transform.position, Color.green, 5f);
}

private void PlayerSpeedUP()
{
    //if rb is moving at moveSpeed, bump move speed up by acceleration speed. else bump down move speed.  Don't let moveSpeed exceed maxSpeed.  
    if (rb.velocity.magnitude >= moveSpeed) moveSpeed += accelerationSpeed * Time.deltaTime;
    else moveSpeed -= accelerationSpeed * 8 * Time.deltaTime;
    if (moveSpeed > maxSpeed) moveSpeed = maxSpeed;
    if (moveSpeed < 5) moveSpeed = 5;

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

private void RotateSelf()
{
    Vector3 LerpDir = Vector3.Lerp(transform.up, floorAngle, Time.deltaTime * gravity);
    transform.rotation = Quaternion.FromToRotation(transform.up, LerpDir) * transform.rotation;
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

    Debug.DrawLine(transform.position, transform.position + (HitDir.normalized * 5f), Color.red, 5f);


    return HitDir.normalized;
}

private void wallDetect(LayerMask beneathFeet)
{
    Collider[] hitColliders = Physics.OverlapSphere(transform.position, playerHeight * .5f + .3f, beneathFeet);
    //Debug.Log(hitColliders[0]);
    if (hitColliders.Length > 0)
    {
        wall = hitColliders[0].transform;
    }
    /*int items = 0;
    foreach(Collider hit in hitColliders) 
    {
        Debug.Log(items+" "+hit);
        items++;
    }
    Debug.Log(hitColliders.Length);
}
*/