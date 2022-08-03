using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateIdle : PlayerState {

    public override void EnterState(PlayerStateManager player, PlayerState prevState) {
        // Upon entering Idle - velocity is 0
        horizontalSpeed = 0f;
        verticalSpeed = 0f;

        UpdateAnimatorState(player, "IsIdling");
    }
    public override void ResetState(PlayerStateManager player) {
        horizontalSpeed = 0f;
        verticalSpeed = 0f;
    }

    public override void UpdateState(PlayerStateManager player) {

        // Check for falling: if player is not on ground, and falling velocity < threshold(debounce)
        if (!GroundCheck(player) && player.playerRigidbody2D.velocity.y < FALL_SPEED_THRESHOLD) {
            player.SwitchState(player.fallState);
        }

        // Check for user inputs - priority: crouch > jump > run
        // When user inputs commands, switch to different state and let new state handle
        if (Input.GetButton("Crouch")) {
            player.SwitchState(player.crouchState);
        } else if (Input.GetButton("Jump")) {
            player.SwitchState(player.jumpState);
        } else if (Input.GetAxisRaw("Horizontal") != 0) {
            player.SwitchState(player.runState);
        }
    }

    public override void OnCollisionEnter(PlayerStateManager player, Collision collision) {

    }
}
