using UnityEngine;

public abstract class PlayerState {

    // Constants
    private protected const float OBSTACLE_CHECK_CAST_DISTANCE = 0.1f;
    private protected const float FALL_SPEED_THRESHOLD = -0.1f;
    private protected const float JUMP_SPEED = 17f;
    // PlayerState fields    
    private protected PlayerState prevState;

    // Speed state properties/fields
    public float getHorizontalSpeed { get { return horizontalSpeed; } }
    private protected float horizontalSpeed;
    public float getVerticalSpeed { get { return verticalSpeed; } }
    private protected float verticalSpeed;

    // Methods to implement in children
    public abstract void EnterState(PlayerStateManager player, PlayerState prevState);
    public abstract void ResetState(PlayerStateManager player);
    public abstract void UpdateState(PlayerStateManager player);
    public abstract void OnCollisionEnter(PlayerStateManager player, Collision collision);

    // Methods to be used by children
    public void UpdateAnimatorState(PlayerStateManager player, string newState) {
        // Disable all animator bools
        player.animator.SetBool("IsIdling", false);
        player.animator.SetBool("IsRunning", false);
        player.animator.SetBool("IsCrouching", false);
        player.animator.SetBool("IsJumping", false);
        player.animator.SetBool("IsFalling", false);

        // Enable the newState
        player.animator.SetBool(newState, true);
    }

    private protected RaycastHit2D GroundCheck(PlayerStateManager player) {
        Vector2 colliderPos = player.playerCircleCollider2D.transform.position;    // center of the CircleCollider2D(center of player)
        colliderPos += player.playerCircleCollider2D.offset;                       // add offset to find 'actual' center of CircleCollider2D
        float colliderRadius = player.playerCircleCollider2D.radius;
        
        // CircleCast below CircleCollider to check for whatIsGround colliders
        RaycastHit2D hit = Physics2D.CircleCast(colliderPos, colliderRadius, Vector2.down, OBSTACLE_CHECK_CAST_DISTANCE, player.whatIsGround);
        return hit;
    }
}
