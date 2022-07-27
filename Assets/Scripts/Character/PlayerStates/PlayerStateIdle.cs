using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateIdle : PlayerState {

    public override void EnterState(PlayerStateManager player, PlayerState prevState) {
        Debug.Log("Entering Idle state");

        horizontalSpeed = 0f;
        verticalSpeed = 0f;

        UpdateAnimatorState(player, "IsIdling");
    }
    public override void ResetState() {
        horizontalSpeed = 0f;
        verticalSpeed = 0f;
    }

    public override void UpdateState(PlayerStateManager player) {
        // Check for user inputs - priority: crouch > jump > run
        if (Input.GetButtonDown("Crouch")) {
            player.SwitchState(player.crouchState);
        } else if (Input.GetButtonDown("Jump")) {
            player.SwitchState(player.jumpState);
        } else if (Input.GetAxisRaw("Horizontal") != 0) {
            player.SwitchState(player.runState);
        }

        // Check for falling: if player is not on ground, and falling velocity < threshold(debounce)
        if (!GroundCheck(player) && player.playerRigidbody2D.velocity.y < fallSpeedThreshold) {
            player.SwitchState(player.fallState);
        }
    }

    public override void OnCollisionEnter(PlayerStateManager player, Collision collision) {

    }


}
