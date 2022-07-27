using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour {

    /** Player property fields =====================================================
    */

    // References to update in Inspector
    [SerializeField] public LayerMask whatIsGround;
    [SerializeField] private PhysicsMaterial2D materialFrictionless;
    [SerializeField] private PhysicsMaterial2D materialHighFriction;
    [SerializeField] private PhysicsMaterial2D materialSlopeFriction;

    // References to validate
    [SerializeField] public Animator animator;
    [SerializeField] public Rigidbody2D playerRigidbody2D;
    [SerializeField] public BoxCollider2D playerBoxCollider2D;
    [SerializeField] public CircleCollider2D playerCircleCollider2D;
    [SerializeField] public SpriteRenderer playerSpriteRenderer;

    // References with defaults
    [SerializeField] public float runSpeed = 60f;
    [SerializeField] private float jumpSpeed = 17f;

    // Private values with defaults
    //private float slopeAngle = 0f;
    //private Vector2 slopeVector;
    private bool isFacingRight = false;


    // Constants
    private const float CROUCH_RUN_MULTIPLIER = 0.5f;
    private const float CROUCH_JUMP_MULTIPLIER = 0.8f;
    private const float AIR_SPEED_MULTIPLIER = 0.8f;
    //private const float MOVEMENT_SMOOTHING_FACTOR = .05f;
    //private Vector2 ZERO_VELOCITY = Vector2.zero;



    /** State Machine fields and methods ===============================================
    */
    PlayerState currentState;
    public PlayerStateIdle idleState = new PlayerStateIdle();
    public PlayerStateRun runState = new PlayerStateRun();
    public PlayerStateJump jumpState = new PlayerStateJump();
    public PlayerStateFall fallState = new PlayerStateFall();
    public PlayerStateCrouch crouchState = new PlayerStateCrouch();

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

    void Start() {
        currentState = idleState;
        currentState.EnterState(this, idleState);

        isFacingRight = !playerSpriteRenderer.flipX;
    }

    void Update() {
        currentState.UpdateState(this);

        animator.SetFloat("Horizontal_Speed", Mathf.Abs(playerRigidbody2D.velocity.x));
        animator.SetFloat("Vertical_Speed", (Mathf.Abs(playerRigidbody2D.velocity.y) > 0.001 ? playerRigidbody2D.velocity.y : 0f));
    }

    private void FixedUpdate() {
        Move(currentState.getHorizontalSpeed, currentState.getVerticalSpeed);

        // Check if player sprite needs to flip
        if ((currentState.getHorizontalSpeed > 0 && !isFacingRight) || (currentState.getHorizontalSpeed < 0 && isFacingRight)) {
            playerSpriteRenderer.flipX = !playerSpriteRenderer.flipX;
            isFacingRight = !playerSpriteRenderer.flipX;
        }
    }

    public void SwitchState(PlayerState newState) {
        PlayerState prevState = currentState;
        currentState.ResetState(this);
        currentState = newState;
        currentState.EnterState(this, prevState);
    }

    void OnCollisionEnter(Collision collision) {
        currentState.OnCollisionEnter(this, collision);
    }

    public void Move(float horizontalSpeed, float verticalSpeed) {
        float moveSpeedX = horizontalSpeed * Time.fixedDeltaTime * 10f;
        moveSpeedX *= (currentState == crouchState ? CROUCH_RUN_MULTIPLIER : 1f);
        moveSpeedX *= ((currentState == jumpState || currentState == fallState) ? AIR_SPEED_MULTIPLIER : 1f);

        float moveSpeedY = verticalSpeed * Time.fixedDeltaTime * 10f;
        moveSpeedX *= (currentState == crouchState ? CROUCH_JUMP_MULTIPLIER : 1f);

        playerRigidbody2D.velocity = new Vector2(moveSpeedX, (moveSpeedY == 0f ? playerRigidbody2D.velocity.y : moveSpeedY));
    }
}
