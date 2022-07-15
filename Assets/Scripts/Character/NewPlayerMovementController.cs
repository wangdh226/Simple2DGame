using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerMovementController : MonoBehaviour
{
    // References to update in Inspector
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform ceilingCheck;
    [SerializeField] private LayerMask whatIsGround;

    // References to validate
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D playerRigidbody2D;
    [SerializeField] private BoxCollider2D playerBoxCollider2D;
    [SerializeField] private CircleCollider2D playerCircleCollider2D;

    // References with defaults
    [SerializeField] private float runSpeed = 60f;
    [SerializeField] private float jumpSpeed = 20f;
    [SerializeField] private float walkDebounce = 1.1f;

    // Private values with defaults
    private float horizontalSpeed = 0f;
    private float horizontalSpeed_target = 0f;
    private float verticalSpeed = 0f;
    private float verticalSpeed_target = 0f;
    

    // Private stateful bools
    private bool isJumping = false;     // Player press Space to send sprite into air
    private bool isFalling = false;     // Player is in air without pressing Space
    private bool isCrouching = false;   // Player press ctrl to make sprite crouch
    private bool isGrounded = false;    // Player is on designated 'ground'
    private bool isFacingRight = true;  // Player is facing right - left if false

    // Constants
    private const float GROUNDED_RADIUS = .2f;
    private Vector2 ZERO_VELOCITY = Vector2.zero;
    private const float CROUCH_SPEED_MULTIPLIER = 0.33f;
    private const float AIR_SPEED_MULTIPLIER = 0.8f;
    private const float MOVEMENT_SMOOTHING_FACTOR = .05f;

    private void OnValidate() {
        if (animator == null) {
            TryGetComponent<Animator>(out animator);
        }
        if (playerRigidbody2D == null) {
            TryGetComponent<Rigidbody2D>(out playerRigidbody2D);
        }
        if (playerBoxCollider2D == null) {
            TryGetComponent<BoxCollider2D>(out playerBoxCollider2D);
        }
        if (playerCircleCollider2D == null) {
            TryGetComponent<CircleCollider2D>(out playerCircleCollider2D);
        }
    }

    void Update() {

        // Constantly update the velocities and send to Animator
        horizontalSpeed = playerRigidbody2D.velocity.x;
        verticalSpeed = playerRigidbody2D.velocity.y;
        animator.SetFloat("Horizontal_Speed", Mathf.Abs(horizontalSpeed));
        animator.SetFloat("Vertical_Speed", Mathf.Abs(verticalSpeed) > walkDebounce ? verticalSpeed : 0f);

        // Set horizontalSpeed_target for smoothdamp
        horizontalSpeed_target = Input.GetAxisRaw("Horizontal") * runSpeed;

        if (Input.GetButtonDown("Jump") && isGrounded) {
            isJumping = true;
            animator.SetBool("IsJumping", true);
            verticalSpeed_target = jumpSpeed * playerRigidbody2D.gravityScale;
            //MoveVertical(verticalSpeed_target * Time.fixedDeltaTime * (isCrouching ? CROUCH_SPEED_MULTIPLIER : 1));
        } else if (Input.GetButtonUp("Jump")) {
            verticalSpeed_target = playerRigidbody2D.gravityScale * -10f;
        }
        if (Input.GetButtonDown("Crouch")) {
            isCrouching = true;
            animator.SetBool("IsCrouching", true);
        } else if (Input.GetButtonUp("Crouch")) {
            isCrouching = false;
            animator.SetBool("IsCrouching", false);
        }
    }

    void FixedUpdate() {

        bool wasGrounded = isGrounded;
        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // MAKE SURE PLAYER LAYER IS NOT CHECKED!
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, GROUNDED_RADIUS, whatIsGround);
        if(colliders.Length > 0) {
            // If colliders exist(non player colliders)
            foreach (Collider2D collider in colliders) {
                if (collider.gameObject != gameObject) {
                    // if found collider is not player collider, means contact with 'ground'
                    // should never be player, unless WhatIsGround is not set properly
                    isGrounded = true;
                    if (!wasGrounded) {
                        // if !wasGrounded in previous check => player was in air => player just landed
                        OnLanding();
                        IsFalling(false);
                    }
                }
            }
        } else {
            // If no colliders exist => player is in the air
            IsFalling(true);
            isGrounded = false;
        }



        float moveSpeedX = horizontalSpeed_target * Time.fixedDeltaTime;
        moveSpeedX *= (isCrouching ? CROUCH_SPEED_MULTIPLIER : 1);
        moveSpeedX *= (!isGrounded ? AIR_SPEED_MULTIPLIER : 1);
        MoveHorizontal(moveSpeedX);

        float moveSpeedY = verticalSpeed_target * Time.fixedDeltaTime;
        moveSpeedY *= (isCrouching ? CROUCH_SPEED_MULTIPLIER : 1);
        if (isJumping && isGrounded) {
            Debug.Log("Jumping");
            MoveVertical(moveSpeedY);
        }
    }

    private void MoveHorizontal(float move) {
        Vector2 targetVelocity = new Vector2(move * 10f, playerRigidbody2D.velocity.y);
        playerRigidbody2D.velocity = Vector2.SmoothDamp(playerRigidbody2D.velocity, targetVelocity, ref ZERO_VELOCITY, MOVEMENT_SMOOTHING_FACTOR);

        // Flip player sprite if moving in opposite direction of facing
        if ((move > 0 && !isFacingRight) || (move < 0 && isFacingRight)) {
            Flip();
        }
    }

    private void MoveVertical(float move) {
        playerRigidbody2D.velocity = new Vector2(playerRigidbody2D.velocity.x, move * 10f);
    }

    
    // Utility/Helper methods
    public void IsFalling(bool isFalling) {
        Debug.Log("Invoke isFalling");
        animator.SetBool("IsFalling", isFalling);
        this.isFalling = isFalling;
    }

    public void OnLanding() {
        Debug.Log("Invoke OnLanding");
        animator.SetBool("IsJumping", false);
        isJumping = false;
    }

    public void IsCrouching(bool isCrouching) {
        Debug.Log("Invoke IsCrouching");
        this.isCrouching = isCrouching;
        animator.SetBool("IsCrouching", isCrouching);
    }


    private void Flip() {
        // Switch the way the player is labelled as facing.
        isFacingRight = !isFacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
