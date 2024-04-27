using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SlothMovement : MonoBehaviour
{
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.E;

    [Header("References")]
    public Transform orientation;
    public Transform player; 
    public Transform playerObj;
    public Rigidbody rb;

    [Header("Movement")]
    public float moveSpeed;
    public float dashSpeed, dashSpeedChangeFactor;
    public float groundDrag;
    private float horizontalInput, verticalInput;
    Vector3 moveDirection;
    public bool isMoving, isDashing;

    [Header("Jump")]
    public float jumpForce;
    public float jumpCooldown, airMultiplier;
    public bool readyToJump, isJumping, isGrounded;

    [Header("Dash")]
    public float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private bool keepMomentum;

    [Header("Attack")]
    public bool readyToAttack;
    public bool isAttacking;
    public float attackCooldown;

    // ready for adding animation
    [Header("Hook")]
    public bool readyToHook;
    public bool isHooking;
    public float hookCooldown;

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

        else if (isDashing)
            rb.drag = 0;

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

        if (isDashing)
        {
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
        }

        moveSpeed = desiredMoveSpeed;

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

        // needs new animation, and probably resetting up
        // hook
        if (Input.GetMouseButton(1) && readyToHook)
        {
            readyToHook = false;
            animator.SetBool("isThrowing", true);

            // resets hook
            Invoke(nameof(ResetHook), hookCooldown);
            isHooking = false;
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
            desiredMoveSpeed = 10f;

        else
            desiredMoveSpeed = 7f;

        bool desriedMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if(isDashing)
            keepMomentum = true;

        if (desriedMoveSpeedHasChanged)
        {
            if(keepMomentum)
            {
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }

            else
                moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // jumps
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;

        animator.SetBool("isJumping", false);
        isJumping = false;
    }

    private void ResetAttack()
    {
        readyToAttack = true;
        animator.SetBool("isAttacking", false);
        isAttacking = false;
    }
    
    private void ResetHook()
    {
        readyToHook = true;
        animator.SetBool("isThrowing", false);
        isHooking = false;
    }

    public void AnimationHandler()
    {
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

    private float speedChangeFactor;
    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp moveSpeed to desired value 
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostFactor = speedChangeFactor;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1;
        keepMomentum = false;
    }
}
