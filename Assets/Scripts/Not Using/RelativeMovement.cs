using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class RelativeMovement : MonoBehaviour
{
    [SerializeField] Transform target;
    public float rotSpeed = 15.0f;
    public float moveSpeed = 6.0f;
    public float jumpSpeed = 15.0f;
    public float gravity = -9.8f;
    public float terminalVelocity = -10.0f;
    public float minFall = -1.5f;
    public float gravFactor = 5.0f;
    public float pushForce = 3.0f;

    private ControllerColliderHit contact;
    private float vertSpeed;
    private CharacterController charController;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        vertSpeed = minFall;
        charController= GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = Vector3.zero;

        float horInput = Input.GetAxis("Horizontal");
        float verInput = Input.GetAxis("Vertical");
        if (horInput !=0 || verInput !=0) 
        {
            Vector3 right = target.right;
            Vector3 forward = Vector3.Cross(right, Vector3.up);
            movement =(right*horInput)+(forward*verInput);
            movement *= moveSpeed;
            movement = Vector3.ClampMagnitude(movement, moveSpeed);
            Quaternion direction = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Lerp(transform.rotation, direction, rotSpeed * Time.deltaTime);
        }

        bool hitGround = false;
        RaycastHit hit;
        if (vertSpeed<0 && Physics.Raycast(transform.position,Vector3.down,out hit))
        {
            float check = (charController.height + charController.radius) / 1.9f;
            hitGround = hit.distance <= 0;
        }

        animator.SetFloat("Speed", movement.sqrMagnitude);

        if(hitGround)
        {
            if(Input.GetButtonDown("Jump"))
            {
                vertSpeed = jumpSpeed;
            }
            else
            {
                vertSpeed = minFall;
                animator.SetBool("jump", false);
            }

        }
        else
        {
            vertSpeed += gravity * gravFactor * Time.deltaTime;
            if(vertSpeed<terminalVelocity)
            {
                vertSpeed = terminalVelocity;
            }
            if(contact!=null)
            {
                animator.SetBool("Jump", true);
            }
        }
        movement.y = vertSpeed;
        movement *= Time.deltaTime;
        charController.Move(movement);

    }
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        contact=hit;
        Rigidbody body = hit.collider.attachedRigidbody;
        if(body!=null && !body.isKinematic) 
        {
            body.velocity = hit.moveDirection * pushForce;
        }
    }
}
