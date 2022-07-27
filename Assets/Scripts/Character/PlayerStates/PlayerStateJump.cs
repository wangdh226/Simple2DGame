using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateJump : PlayerState {

    public override void EnterState(PlayerStateManager player, PlayerState prevState) {
        Debug.Log("Entering Jump state");
        horizontalSpeed = player.playerRigidbody2D.velocity.x;
        //verticalSpeed = player.playerRigidbody2D.velocity.y;
        UpdateAnimatorState(player, "IsJumping");
    }
    public override void ResetState() {
        horizontalSpeed = 0f;
        verticalSpeed = 0f;
    }

    public override void UpdateState(PlayerStateManager player) {
        // Check for falling: if player is not on ground, and falling velocity < threshold(debounce)
        if (!GroundCheck(player) && player.playerRigidbody2D.velocity.y < fallSpeedThreshold) {
            player.SwitchState(player.fallState);
        } else if (GroundCheck(player)) {
            if (Input.GetAxisRaw("Horizontal") != 0) {
                player.SwitchState(player.runState);
            } else {
                player.SwitchState(player.idleState);
            }
            
        }

        horizontalSpeed = Input.GetAxisRaw("Horizontal") * player.runSpeed;
        verticalSpeed = player.playerRigidbody2D.velocity.y;
    }

    public override void OnCollisionEnter(PlayerStateManager player, Collision collision) {

    }
}
