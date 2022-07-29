using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateCrouch : PlayerState {

    private bool isUnderCeiling = false;
    //private bool isCrouchJumping = false;

    public override void EnterState(PlayerStateManager player, PlayerState prevState) {
        // Upon entering state, maintain previous horizontal velocity
        horizontalSpeed = player.playerRigidbody2D.velocity.x;

        player.playerBoxCollider2D.enabled = false;
        UpdateAnimatorState(player, "IsCrouching");
    }

    public override void ResetState(PlayerStateManager player) {
        horizontalSpeed = 0f;
        verticalSpeed = 0f;
        isUnderCeiling = false;
        //isCrouchJumping = false;
        player.playerBoxCollider2D.enabled = true;
    }

    public override void UpdateState(PlayerStateManager player) {

        // Check for falling: if player is not on ground, and falling velocity < threshold(debounce)
        //if (!GroundCheck(player) && player.playerRigidbody2D.velocity.y < FALL_SPEED_THRESHOLD && !isCrouchJumping) {
        //    player.SwitchState(player.fallState);
        //}

        isUnderCeiling = CheckCeiling(player);
        // Check for crouch input and set state
        if (!Input.GetButton("Crouch") && !isUnderCeiling) {
            // If player is not holding Crouch key AND nothing above the player, uncrouch
            if (Input.GetAxisRaw("Horizontal") != 0f) {
                // If player input attempting to move when uncrouching, go to runState
                player.SwitchState(player.runState);
            } else {
                // If player input not trying to move when uncrouching, go to idleState
                player.SwitchState(player.idleState);
            }
        }
        RaycastHit2D hit = GroundCheck(player);
        // Check for jump input and deal with crouch jump
        if (Input.GetButton("Jump")) {
            //isCrouchJumping = true;
            // crouch jump - while grounded, overwrite vSpeed, once off ground, let gravity
            verticalSpeed = (hit ? JUMP_SPEED * player.playerRigidbody2D.gravityScale : 0f);
        } else if (!Input.GetButton("Jump")) {
            // deal with case where player lets go of jump before 'leaving ground'
            verticalSpeed = 0f;
        }

        horizontalSpeed = Input.GetAxisRaw("Horizontal") * player.runSpeed;
    }

    public override void OnCollisionEnter(PlayerStateManager player, Collision collision) {

    }

    private bool CheckCeiling(PlayerStateManager player) {
        // Check if player is in contact with a ceiling
        Vector2 colliderPos = player.playerBoxCollider2D.transform.position;
        colliderPos += player.playerBoxCollider2D.offset;
        RaycastHit2D hit = Physics2D.BoxCast(colliderPos, player.playerBoxCollider2D.size, 0f, Vector2.up, OBSTACLE_CHECK_CAST_DISTANCE, player.whatIsGround);
        
        // Update state if player is under ceiling
        if (hit) {
            return true;
        } else {
            return false;
        }
    }
}
