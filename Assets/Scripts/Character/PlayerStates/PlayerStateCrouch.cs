using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateCrouch : PlayerState {

    private bool isUnderCeiling = false;
    private const float JUMP_SPEED = 17f;

    public override void EnterState(PlayerStateManager player, PlayerState prevState) {
        //Debug.Log("Entering Crouch state");
        horizontalSpeed = player.playerRigidbody2D.velocity.x;

        player.playerBoxCollider2D.enabled = false;
        UpdateAnimatorState(player, "IsCrouching");
    }

    public override void ResetState(PlayerStateManager player) {
        horizontalSpeed = 0f;
        verticalSpeed = 0f;
        isUnderCeiling = false;
        player.playerBoxCollider2D.enabled = true;
    }

    public override void UpdateState(PlayerStateManager player) {

        // Check for falling: if player is not on ground, and falling velocity < threshold(debounce)
        if (!GroundCheck(player) && player.playerRigidbody2D.velocity.y < fallSpeedThreshold) {
            player.SwitchState(player.fallState);
        }

        isUnderCeiling = CheckCeiling(player);
        // Check for crouch input and set state
        if (!Input.GetButton("Crouch") && !isUnderCeiling) {
            // If player is not holding Crouch key, attempt to uncrouch
            if (Input.GetAxisRaw("Horizontal") != 0f) {
                // don't uncrouch if there is something above the player
                player.SwitchState(player.runState);
            } else {
                player.SwitchState(player.idleState);
            }
        } else if (Input.GetButton("Jump")) {
            //player.SwitchState(player.jumpState);

            // crouch jump
            RaycastHit2D hit = GroundCheck(player);
            if (hit) {
                verticalSpeed = JUMP_SPEED * player.playerRigidbody2D.gravityScale;
            } else {
                verticalSpeed = 0f;
            }
        }

        horizontalSpeed = Input.GetAxisRaw("Horizontal") * player.runSpeed;
    }

    public override void OnCollisionEnter(PlayerStateManager player, Collision collision) {

    }

    private bool CheckCeiling(PlayerStateManager player) {
        // Check if player is in contact with a ceiling
        Vector2 colliderPos = player.playerBoxCollider2D.transform.position;
        colliderPos += player.playerBoxCollider2D.offset;
        RaycastHit2D hit = Physics2D.BoxCast(colliderPos, player.playerBoxCollider2D.size, 0f, Vector2.up, 0.01f, player.whatIsGround);
        
        // Update state if player is under ceiling
        if (hit) {
            return true;
        } else {
            return false;
        }
    }
}
