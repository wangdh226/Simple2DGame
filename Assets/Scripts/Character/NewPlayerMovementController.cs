using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerMovementController : MonoBehaviour {
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
    [SerializeField] private float jumpSpeed = 17f;
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
    private bool wasCrouching = false;  // Store for previous Crouch state
    private bool isUnderCeiling = false;// Store for whether there is hitbox above player
    private bool isGrounded = false;    // Player is on designated 'ground'
    private bool wasGrounded = false;   // Store for previous Grounded state
    private bool isFacingRight = true;  // Player is facing right - left if false

    // Constants
    private const float GROUNDED_RADIUS = 0.2f;
    private const float CEILING_RADIUS = 0.2f;
    private const float CROUCH_RUN_MULTIPLIER = 0.33f;
    private const float CROUCH_JUMP_MULTIPLIER = 0.8f;
    private const float AIR_SPEED_MULTIPLIER = 0.8f;
    private const float MOVEMENT_SMOOTHING_FACTOR = .05f;
    private Vector2 ZERO_VELOCITY = Vector2.zero;

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

    /** Method to update player states - is* bools
     *  I think this method runs *more often* than FixedUpdate, so use as continuous update for state
     */
    void Update() {

        // Check if player is in contact with a ceiling
        if (Physics2D.OverlapCircle(ceilingCheck.position, CEILING_RADIUS, whatIsGround)) {
            isUnderCeiling = true;
            Debug.Log("isUnderCeiling = " + isUnderCeiling);
        } else {
            isUnderCeiling = false;
            Debug.Log("isUnderCeiling = " + isUnderCeiling);
        }

        // Check if player is grounded or not
        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // MAKE SURE PLAYER LAYER IS NOT INCLUDED IN whatIsGround!
        wasGrounded = isGrounded;
        if (Physics2D.OverlapCircle(groundCheck.position, GROUNDED_RADIUS, whatIsGround)) {
            isGrounded = true;
            if (!wasGrounded) {
                // if !wasGrounded in previous check => player was in air => player just landed
                OnLanding();
            }
        } else {
            isGrounded = false;
            // If no colliders exist => player is in the air
            OnFalling();
        }

        // Set horizontalSpeed_target for smoothdamp
        horizontalSpeed_target = Input.GetAxisRaw("Horizontal") * runSpeed;

        // Check for jump input and set verticalSpeed_target
        if (Input.GetButtonDown("Jump") && isGrounded && !isUnderCeiling) {
            isJumping = true;
            verticalSpeed_target = jumpSpeed * playerRigidbody2D.gravityScale;
        }

        // Check for crouch input and set state
        if (Input.GetButton("Crouch") && !isJumping) {
            isCrouching = true;
        } else if (!Input.GetButton("Crouch") && wasCrouching) {
            if (isUnderCeiling) {
                isCrouching = true;
            } else {
                isCrouching = false;
            }
            
        }
        wasCrouching = isCrouching;
    }

    /** Method to update player every frame - moving/crouching/jumping etc
     *   Used like a frame-based discrete update based on continuous state changes from Update()
     */
    void FixedUpdate() {
        // Constantly update the velocities and send to Animator
        horizontalSpeed = playerRigidbody2D.velocity.x;
        verticalSpeed = playerRigidbody2D.velocity.y;
        animator.SetFloat("Horizontal_Speed", Mathf.Abs(horizontalSpeed));
        animator.SetFloat("Vertical_Speed", verticalSpeed);

        IsJumping(isJumping);
        IsCrouching(isCrouching);
        Move();
    }

    private void Move() {
        // Calculate new horizontal move speed and implement
        float moveSpeedX = horizontalSpeed_target * Time.fixedDeltaTime;
        moveSpeedX *= (isCrouching ? CROUCH_RUN_MULTIPLIER : 1f);
        moveSpeedX *= (!isGrounded ? AIR_SPEED_MULTIPLIER : 1f);
        MoveHorizontal(moveSpeedX);

        // Calculate new vertical move speed and implement
        //Debug.Log(isJumping + ": " + isGrounded);
        if (isJumping && isGrounded) {
            float moveSpeedY = verticalSpeed_target * Time.fixedDeltaTime;
            moveSpeedY *= (isCrouching ? CROUCH_JUMP_MULTIPLIER : 1f);
            MoveVertical(moveSpeedY);
        }

        // Flip player sprite if moving in opposite direction of facing
        if ((moveSpeedX > 0 && !isFacingRight) || (moveSpeedX < 0 && isFacingRight)) {
            Flip();
        }
    }

    private void MoveHorizontal(float move) {
        // Calculate new target velocity and use SmoothDamp to accelerate to it
        Vector2 targetVelocity = new Vector2(move * 10f, playerRigidbody2D.velocity.y);
        playerRigidbody2D.velocity = Vector2.SmoothDamp(playerRigidbody2D.velocity, targetVelocity, ref ZERO_VELOCITY, MOVEMENT_SMOOTHING_FACTOR);
    }

    private void MoveVertical(float move) {
        // Sett new velocity and let gravity pull down
        playerRigidbody2D.velocity = new Vector2(playerRigidbody2D.velocity.x, move * 10f);
    }


    // Utility/Helper methods
    private void OnFalling() {
        //Debug.Log("Invoke onFalling");
        isFalling = true;
        animator.SetBool("IsFalling", isFalling);
    }

    private void OnLanding() {
        //Debug.Log("Invoke OnLanding");
        isJumping = false;
        isFalling = false;
        isGrounded = true;
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsFalling", isFalling);
    }

    private void IsJumping(bool isJumping) {
        //Debug.Log("Invoke IsJumping");
        this.isJumping = isJumping;
        animator.SetBool("IsJumping", isJumping);
    }

    private void IsCrouching(bool isCrouching) {
        //Debug.Log("Invoke IsCrouching");
        this.isCrouching = isCrouching;

        if (playerBoxCollider2D != null) {
            playerBoxCollider2D.enabled = !this.isCrouching;
        }
        animator.SetBool("IsCrouching", this.isCrouching);
    }

    private void Flip() {
        // Switch the way the player is labelled as facing.
        isFacingRight = !isFacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }
}
