using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float speed = 8;
    public float rotateSpdY = 5;
    public float jumpSpeed = 16;
    public float gravity = -20;
    public float slideSpd = 4;
    public float maxSlope = 45;
    //public float slideConstraint = 0.7f;
    //public float slideFriction = 0.3f;

    CharacterController playerController;
    Animator animator;

    float moveX;
    float moveY;
    float moveZ;
    Vector3 move;
    Vector3 prevPos;
    Vector3 velocity;

    float currSlideSpd;
    float rotateY;
    bool isGrounded;
    bool isSliding;
    float slope;

    RaycastHit rayHit;
    ControllerColliderHit controlHit;

    public enum State
    { Idle, Run, Jump, Fall, Land }

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        moveX = moveY = moveZ = 0;
        move = Vector3.zero;
        prevPos = transform.position;
        velocity = Vector3.zero;

        rotateY = transform.eulerAngles.y;
        isGrounded = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (StageControl.state != StageControl.GameState.Play) return;

        prevPos = transform.position;
        Move();
        Rotate();
        velocity = transform.position - prevPos;
    }

    void Move()
    {
        //Debug.Log("isSliding: " + isSliding);
        //Debug.Log("isGrounded: " + isGrounded);

        moveX = Input.GetAxisRaw("Horizontal");
        moveZ = Input.GetAxisRaw("Vertical");
        move = Vector3.Normalize(moveX * transform.right + moveZ * transform.forward) * speed;
        SetSliding();
        SetRunning(move);

        Vector3 downDir;
        if (isGrounded && slope > 0)
        {
            downDir = Vector3.Normalize(Vector3.Cross(controlHit.normal, Vector3.Cross(controlHit.normal, Vector3.up)));
            if (move.magnitude > 0 && Vector3.Angle(move, controlHit.normal) < 90) //walk down slope
            {
                //problem
                move = Vector3.Cross(controlHit.normal, move);
                move = Vector3.Normalize(Vector3.Cross(move, controlHit.normal)) * speed;
                //Debug.DrawRay(transform.position, move, Color.cyan, 1, false);
            }
        }
        else
            downDir = -Vector3.up;
        //Debug.DrawRay(transform.position, downDir, Color.red, 1, false);

        if (isSliding)
        {
            //Debug.Log(Vector3.Angle(downDir, move));
            move += downDir * currSlideSpd * Time.deltaTime;
        }
        else
        {
            if (isGrounded)
            {
                moveY = 0;
                if (Input.GetKeyDown("space")) Jump();
            }
        }

        moveY += gravity * Time.deltaTime;
        move.y = moveY;

        //Debug.DrawRay(transform.position, move, Color.blue, 1, false);
        playerController.Move(move * Time.deltaTime);

        //Debug.DrawRay(transform.position, -0.2f * Vector3.up, Color.blue, 1, false);

        if (isGrounded && Physics.Raycast(transform.position, -Vector3.up, out rayHit, 0.5f)
            && GameObject.ReferenceEquals(rayHit.transform.gameObject, controlHit.gameObject)) //problem
            playerController.Move(-Vector3.up * rayHit.distance);

    }

    void Jump()
    {
        moveY = jumpSpeed;
        isGrounded = false;
        SetAnimator(State.Jump);
    }

    void Rotate()
    {
        rotateY += Input.GetAxis("Mouse X") * rotateSpdY;
        transform.eulerAngles = new Vector3(0, rotateY, 0);
    }

    void SetSliding()
    {
        if (isGrounded) {

            slope = Vector3.Angle(Vector3.up, controlHit.normal);

            if (slope > maxSlope) {
                isSliding = true;
                currSlideSpd += slope;
                return;
            }
        }

        isSliding = false;
        currSlideSpd = 0;
    }

    void SetRunning(Vector3 move)
    {
        if (move.magnitude > 0)
            SetAnimator(State.Run, true);
        else SetAnimator(State.Run, false);
    }

    public void SetIsGrounded(bool isGrounded)
    {
        this.isGrounded = isGrounded;
        if (isGrounded) {
            SetAnimator(State.Land);
            SetAnimator(State.Fall, false);
            //not walking
        }
        else
            SetAnimator(State.Fall, true);
    }

    void SetAnimator(State state, bool value = false)
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Run:
                animator.SetBool("IsRunning", value);
                break;
            case State.Jump:
                animator.SetTrigger("IsJumping");
                break;
            case State.Fall:
                animator.SetBool("IsFalling", value);
                break;
            case State.Land:
                animator.SetTrigger("IsGrounded");
                break;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) return;
        SetIsGrounded(false);//isGrounded = false;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        controlHit = hit;
        if (Mathf.Abs(hit.point.y - transform.position.y) < 0.3f)
            SetIsGrounded(true);//isGrounded = true;

        if (hit.collider.CompareTag("Stroke") && IsMoving())
            hit.gameObject.GetComponent<StrokeParticle>().CollidePlayer(GetMoveMagnitude());
    }

    float GetMoveMagnitude()
    { return Mathf.Abs(velocity.magnitude); }

    bool IsMoving()
    { return Mathf.Abs(velocity.magnitude) > 0.01 || !isGrounded; }
}
