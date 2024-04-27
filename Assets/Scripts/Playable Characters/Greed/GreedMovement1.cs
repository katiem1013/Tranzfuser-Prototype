using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreedMovement1 : MonoBehaviour
{
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dodgeKey = KeyCode.E;

    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;

    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    private float horizontalInput, verticalInput;
    Vector3 moveDirection;
    public bool isMoving;

    [Header("Jump")]
    public float jumpForce;
    public float jumpCooldown, airMultiplier;
    public bool readyToJump, isJumping, isGrounded;

    [Header("Attack")]
    public bool readyToAttack;
    public bool isAttacking;
    public float attackCooldown;

    [Header("Dodge")]
    public bool readyToDodge;
    public bool isDodging;
    public float dodgeCooldown, dodgeSpeed;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Animations")]
    public Animator animator;
    Vector2 speedPercent;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        MovementInputs();
        SpeedControl();
        AnimationHandler();

        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        // handle drag
        if (grounded)
        {
            rb.drag = groundDrag;

            animator.SetBool("isGrounded", true);
            isGrounded = true;


            animator.SetBool("isFalling", false);
        }

        else
        {
            rb.drag = 0;

            animator.SetBool("isGrounded", false);
            isGrounded = false;

            if ((isJumping && rb.velocity.y < 0) || rb.velocity.y < -2)
            {
                animator.SetBool("isFalling", true);
            }

        }

    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovementInputs()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            animator.SetBool("isJumping", true);
            isJumping = true;

            Jump();
            // stops constant jumping
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // attack
        if (Input.GetMouseButtonDown(0) && readyToAttack && grounded)
        {
            print("Attack");
            readyToAttack = false;
            animator.SetBool("isAttacking", true);
            isAttacking = true;

            // resets attack
            Invoke(nameof(ResetAttack), attackCooldown);
        }

        // dodge
        if (Input.GetKeyDown(dodgeKey))
        {
            readyToDodge = false;
            StartCoroutine(Dodge());
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground 
        if (grounded && !isAttacking)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded && !isAttacking)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            isMoving = true;

        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
            isMoving = false;

    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }

        if (Input.GetKey(KeyCode.LeftShift))
            moveSpeed = 10f;

        else
            moveSpeed = 7f;
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // jumps
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    IEnumerator Dodge()
    {   
        isDodging = true;
        float timer = 0;
        animator.SetBool("isDodging", true);
        
        while (timer < dodgeCooldown)
        {
            timer += Time.deltaTime;

            Vector3 dir = (playerObj.forward * dodgeSpeed) + (Vector3.up * rb.velocity.y);
            rb.velocity += dir * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        readyToDodge = true;
        animator.SetBool("isDodging", false);
        isDodging = false;
    }

    private void ResetJump()
    {
        readyToJump = true;

        animator.SetBool("isJumping", false);
        isJumping = false;
    }

    private void ResetAttack()
    {
        // stops spam attacks and handles the animation
        readyToAttack = true;
        animator.SetBool("isAttacking", false);
        isAttacking = false;
    }

    public void AnimationHandler()
    {
        // sets the animations 
        if (isMoving && moveSpeed == 7)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isRunning", false);
        }

        if (isMoving && moveSpeed > 7)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", true);
        }

        if (!isMoving)
        {
            animator.SetBool("isWalking", false);
        }

    }
}
