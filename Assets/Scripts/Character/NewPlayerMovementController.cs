using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Controller that acts like a state machine for player animations and movement.
 *    Class that manages player movement and animations:
 *    Idle, Run, Jump, Fall, Crouch
 */
public class NewPlayerMovementController : MonoBehaviour {
    
    // References to update in Inspector
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform ceilingCheck;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private PhysicsMaterial2D materialFrictionless;
    [SerializeField] private PhysicsMaterial2D materialHighFriction;
    [SerializeField] private PhysicsMaterial2D materialSlopeFriction;

    // References to validate
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D playerRigidbody2D;
    [SerializeField] private BoxCollider2D playerBoxCollider2D;
    [SerializeField] private CircleCollider2D playerCircleCollider2D;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;

    // References with defaults
    [SerializeField] private float runSpeed = 60f;
    [SerializeField] private float jumpSpeed = 17f;

    // Private values with defaults
    private float horizontalSpeed = 0f;
    private float horizontalSpeed_target = 0f;
    private float verticalSpeed = 0f;
    private float verticalSpeed_target = 0f;

    private float slopeAngle = 0f;
    private Vector2 slopeVector;

    // Private stateful bools
    private bool isJumping = false;     // Player press Space to send sprite into air
    private bool isFalling = false;     // Player is in air without pressing Space
    private bool isCrouching = false;   // Player press ctrl to make sprite crouch
    private bool wasCrouching = false;  // Store for previous Crouch state
    private bool isUnderCeiling = false;// Store for whether there is hitbox above player
    private bool isGrounded = false;    // Player is on designated 'ground'
    private bool wasGrounded = false;   // Store for previous Grounded state
    private bool isFacingRight = true;  // Player is facing right - left if false
    private bool isOnSlope = false;
    private bool isUpSlope = false;
    private bool isDownSlope = false;

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
        if (playerSpriteRenderer == null) {
            TryGetComponent<SpriteRenderer>(out playerSpriteRenderer);
        }
    }

    /** Method that runs whenever possible(CPU/Unity limit) to update player states - is* bools
     *  I think this method runs *more often* than FixedUpdate, so use as continuous update for state
     */
    void Update() {

        // Check if player is in contact with a ceiling
        isUnderCeiling = Physics2D.OverlapCircle(ceilingCheck.position, CEILING_RADIUS, whatIsGround);

        // Check if player is grounded or not
        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // MAKE SURE PLAYER LAYER IS NOT INCLUDED IN whatIsGround!
        wasGrounded = isGrounded;
        if (Physics2D.OverlapCircle(groundCheck.position, GROUNDED_RADIUS, whatIsGround)) {
            isGrounded = true;
            if (!wasGrounded) {
                // if !wasGrounded in previous check => player was in air => player just landed
                isJumping = false;
                isFalling = false;
                OnLanding();
            }
        } else {
            isGrounded = false;
            isFalling = true;
            // If no colliders exist => player is in the air
            OnFalling();
        }

        // Set horizontalSpeed_target for smoothdamp
        horizontalSpeed_target = Input.GetAxisRaw("Horizontal") * runSpeed;

        // Check for jump input and set verticalSpeed_target
        if (Input.GetButtonDown("Jump") && isGrounded && !isUnderCeiling) {
            // When player presses Jump key, is on ground, and not under a ceiling,
            // Set jumping, update verticalSpeed_target
            isJumping = true;
            verticalSpeed_target = jumpSpeed * playerRigidbody2D.gravityScale;
        }

        // Check for crouch input and set state
        if (Input.GetButton("Crouch") && !isJumping) {
            // When player presses Crouch key, and is not jumping, crouch
            isCrouching = true;
        } else if (!Input.GetButton("Crouch") && wasCrouching) {
            // If player is not holding Crouch key, and was crouching before, attempt to uncrouch
            if (isUnderCeiling) { // don't uncrouch if there is something above the player
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
        animator.SetFloat("Horizontal_Speed", Mathf.Abs(playerRigidbody2D.velocity.x));
        animator.SetFloat("Vertical_Speed", (Mathf.Abs(playerRigidbody2D.velocity.y) > 0.001 ? playerRigidbody2D.velocity.y : 0f));

        IsJumping();
        IsCrouching();
        Move();
    }
    
    // Player Movement methods
    private void Move() {
        //Debug.Log(playerRigidbody2D.velocity.x);

        // Calculate new horizontal move speed and implement
        float moveSpeedX = horizontalSpeed_target * Time.fixedDeltaTime * 10f;
        //moveSpeedX += moveSpeedX * (isOnSlope ? -slopeNormalPerp.x : 0f);
        moveSpeedX *= (isCrouching ? CROUCH_RUN_MULTIPLIER : 1f);
        moveSpeedX *= (!isGrounded ? AIR_SPEED_MULTIPLIER : 1f);
        playerRigidbody2D.velocity = new Vector2(moveSpeedX, playerRigidbody2D.velocity.y);
        //MoveHorizontal(moveSpeedX); // doesn't work with current iteration of slopes


        // Calculate new vertical move speed and implement
        float moveSpeedY = 1f;
        // If player is on ground, check for slope and/or jumping
        if (isGrounded) {
            playerCircleCollider2D.sharedMaterial = materialHighFriction;
            // Check for slope
            SlopeCheck();
            if (isJumping) {
                moveSpeedY = verticalSpeed_target * Time.fixedDeltaTime * 10f;
                moveSpeedY *= (isCrouching ? CROUCH_JUMP_MULTIPLIER : 1f);
                playerRigidbody2D.velocity = new Vector2(playerRigidbody2D.velocity.x, moveSpeedY);
            } else if (isOnSlope) {
                if (isUpSlope && Mathf.Abs(horizontalSpeed_target) > 0f) {
                    playerCircleCollider2D.sharedMaterial = materialSlopeFriction;
                } else if (isDownSlope && Mathf.Abs(horizontalSpeed_target) > 0f) {
                    playerCircleCollider2D.sharedMaterial = materialHighFriction;
                }
            }
        } 

        // Flip player sprite if moving in opposite direction of facing
        if ((horizontalSpeed_target > 0 && !isFacingRight) || (horizontalSpeed_target < 0 && isFacingRight)) {
            Flip();
        }
    }

    private void SlopeCheck() {
        // Some maths
        Vector2 colliderPos = playerCircleCollider2D.transform.position;    // center of the CircleCollider2D(center of player)
        colliderPos += playerCircleCollider2D.offset;                       // add offset to find 'actual' center of CircleCollider2D
        // CircleCast around CircleCollider to check for whatIsGround colliders
        RaycastHit2D hit = Physics2D.CircleCast(colliderPos, playerCircleCollider2D.radius + 0.01f, Vector2.down, 0.01f, whatIsGround);

        if (hit) {
            slopeVector = Vector2.Perpendicular(hit.normal).normalized * (isFacingRight ? -1f : 1f); // Angle of slope(*facing) 
            slopeAngle = Vector2.SignedAngle(Vector2.up, slopeVector); // Angle between slope and Up
            Debug.Log(slopeAngle + ": " + isUpSlope + ": " + isDownSlope);

            // Check slopeAngle sign and magnitude
            if (Mathf.Abs(slopeAngle) < 90f && slopeAngle < 0f) {
                // Is on Up slope, facing right
                isOnSlope = true;
                isUpSlope = true;
                isDownSlope = false;
            } else if (Mathf.Abs(slopeAngle) < 90f && slopeAngle > 0f) {
                // Is on Up slope, facing left
                isOnSlope = true;
                isUpSlope = true;
                isDownSlope = false;
            } else if (Mathf.Abs(slopeAngle) > 90f) {
                // Is on down slope
                isOnSlope = true;
                isUpSlope = false;
                isDownSlope = true;
            } else if (Mathf.Abs(slopeAngle) == 90f) {
                // Not on slope
                isOnSlope = false;
                isUpSlope = false;
                isDownSlope = false;
            }

            Debug.DrawRay(hit.point, hit.normal, Color.green);
            Debug.DrawRay(hit.point, slopeVector, Color.red); // angle of slope(tan of circlecast)
            Debug.DrawRay(hit.point, new Vector2(slopeVector.x, 0f), Color.blue); // X of angle of slope
            Debug.DrawRay(hit.point, new Vector2(0f, slopeVector.y), Color.cyan); // Y of angle of slope
        }
    }

    private void MoveHorizontal(float move) {
        // Calculate new target velocity and use SmoothDamp to accelerate to it
        Vector2 targetVelocity = new Vector2(move, playerRigidbody2D.velocity.y);
        playerRigidbody2D.velocity = Vector2.SmoothDamp(playerRigidbody2D.velocity, targetVelocity, ref ZERO_VELOCITY, MOVEMENT_SMOOTHING_FACTOR);
    }

    private void MoveVertical(float move) {
        // Set new velocity and let gravity pull down
        playerRigidbody2D.velocity = new Vector2(playerRigidbody2D.velocity.x, move);
    }
   
    private void Flip() {
        // Switch the way the player is labelled as facing.
        isFacingRight = !isFacingRight;
        playerSpriteRenderer.flipX = !isFacingRight;
    }

    // Animator Update methods
    private void OnFalling() {
        playerCircleCollider2D.sharedMaterial = materialFrictionless;
        animator.SetBool("IsFalling", isFalling);
    }

    private void OnLanding() {
        playerCircleCollider2D.sharedMaterial = materialHighFriction;
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsFalling", isFalling);
    }

    private void IsJumping() {
        if (isJumping) {
            playerCircleCollider2D.sharedMaterial = materialFrictionless;
        }
        animator.SetBool("IsJumping", isJumping);
    }

    private void IsCrouching() {
        if (playerBoxCollider2D != null) {
            playerBoxCollider2D.enabled = !this.isCrouching;
        }
        animator.SetBool("IsCrouching", this.isCrouching);
    }
}
