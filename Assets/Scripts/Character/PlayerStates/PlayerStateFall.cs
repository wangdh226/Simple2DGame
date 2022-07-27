using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateFall : PlayerState {

    public override void EnterState(PlayerStateManager player, PlayerState prevState) {
        //Debug.Log("Entering Fall state");
        horizontalSpeed = player.playerRigidbody2D.velocity.x;

        this.prevState = prevState;
        if (prevState != player.crouchState) {
            UpdateAnimatorState(player, "IsFalling");
        }
    }

    public override void ResetState(PlayerStateManager player) {
        horizontalSpeed = 0f;
        verticalSpeed = 0f;
    }

    public override void UpdateState(PlayerStateManager player) {

        // Change 'prevState' based on player input
        // Since the state change only happens when hitting the ground,
          // constantly update next state based on player input while falling
        if (Input.GetButton("Crouch")) {
            prevState = player.crouchState;
        } else if (Input.GetButton("Jump")) {
            prevState = player.jumpState;
        } else if (Input.GetAxisRaw("Horizontal") != 0) {
            prevState = player.runState;
        } else {
            // If player is not inputting at the moment of landing, return to idle
            prevState = player.idleState;
        }

        RaycastHit2D hit = GroundCheck(player);
        // Check what the state was before entering fall, and return to it
        if (hit) {
            if (prevState == player.crouchState) {
                player.SwitchState(player.crouchState);
            } else if (prevState == player.jumpState) {
                player.SwitchState(player.jumpState);
            } else if (prevState == player.runState) {
                player.SwitchState(player.runState);
            } else {
                player.SwitchState(player.idleState);
            }
        }

        horizontalSpeed = Input.GetAxisRaw("Horizontal") * player.runSpeed;
    }

    public override void OnCollisionEnter(PlayerStateManager player, Collision collision) {

    }
}
