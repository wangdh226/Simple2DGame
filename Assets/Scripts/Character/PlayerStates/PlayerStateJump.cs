using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateJump : PlayerState {

    private bool hasLeftGround = false;

    public override void EnterState(PlayerStateManager player, PlayerState prevState) {
        // Upon entering state, maintain previous horizontal velocity
        horizontalSpeed = player.playerRigidbody2D.velocity.x;
        // Upon entering state, maintain previous vertical velocity and add jump velocity
        verticalSpeed = player.playerRigidbody2D.velocity.y + (JUMP_SPEED * player.playerRigidbody2D.gravityScale);

        hasLeftGround = false;
        this.prevState = prevState;
        UpdateAnimatorState(player, "IsJumping");
    }
    public override void ResetState(PlayerStateManager player) {
        horizontalSpeed = 0f;
        verticalSpeed = 0f;
    }

    public override void UpdateState(PlayerStateManager player) {

        // Check if we are on the ground every frame+
        RaycastHit2D hit = GroundCheck(player);

        // Check for falling: if player is not on ground, and falling velocity < threshold(debounce)
        if (hit && !hasLeftGround) {
            // On the ground AND has not left the ground(not yet 'actually jumped')
              // then keep the vertical speed to the jump speed since we haven't 'actually jumped'
            verticalSpeed = JUMP_SPEED * player.playerRigidbody2D.gravityScale;
        } else if (!hit && !hasLeftGround) {
            // Not on the ground AND has not yet left the ground
              // then set hasLeftGround to true and let gravity - ie now we are in the air and 'actually jumped'
            hasLeftGround = true;
            verticalSpeed = 0f;
        }

        // After we have left the ground, let gravity deal with falling
          // Switch states to handle situations - go to fallState, lands on smth before fallState
        if (hasLeftGround) {
            // If velocity becomes less than the fallSpeedThreshold(always negative) -> fallState
            if (player.playerRigidbody2D.velocity.y < FALL_SPEED_THRESHOLD) {
                player.SwitchState(player.fallState);
            }
            // If lands on 'ground' before fallState, check input for next state
            if (hit) {
                if (Input.GetButton("Crouch")) {
                    player.SwitchState(player.crouchState);
                } else if (Input.GetAxisRaw("Horizontal") != 0) {
                    player.SwitchState(player.runState);
                } else {
                    player.SwitchState(player.idleState);
                }
            }
        }

        horizontalSpeed = Input.GetAxisRaw("Horizontal") * player.runSpeed;
    }

    public override void OnCollisionEnter(PlayerStateManager player, Collision collision) {

    }
}
