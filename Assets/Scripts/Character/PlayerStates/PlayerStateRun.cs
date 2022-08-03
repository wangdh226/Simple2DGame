using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateRun : PlayerState {

    public override void EnterState(PlayerStateManager player, PlayerState prevState) {
        // Upon entering state, maintain previous horizontal velocity
        horizontalSpeed = player.playerRigidbody2D.velocity.x;

        UpdateAnimatorState(player, "IsRunning");
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

        // Check for user inputs - priority: crouch > jump > idle
        if (Input.GetButtonDown("Crouch")) {
            player.SwitchState(player.crouchState);
        } else if (Input.GetButtonDown("Jump")) {
            player.SwitchState(player.jumpState);
        } else if (Input.GetAxisRaw("Horizontal") == 0) {
            player.SwitchState(player.idleState);
        }

        horizontalSpeed = Input.GetAxisRaw("Horizontal") * player.runSpeed;
    }

    public override void OnCollisionEnter(PlayerStateManager player, Collision collision) {

    }
}
