using UnityEngine;

public abstract class PlayerState {

    public float getHorizontalSpeed { get { return horizontalSpeed; } }
    private protected float horizontalSpeed;

    public float getVerticalSpeed { get { return verticalSpeed; } }
    private protected float verticalSpeed;

    public abstract void EnterState(PlayerStateManager player);
    public abstract void UpdateState(PlayerStateManager player);
    public abstract void OnCollisionEnter(PlayerStateManager player, Collision collision);

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


}
