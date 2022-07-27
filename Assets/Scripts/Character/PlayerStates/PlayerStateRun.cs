using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateRun : PlayerState {

    public override void EnterState(PlayerStateManager player, PlayerState prevState) {
        Debug.Log("Entering Run state");
        horizontalSpeed = player.playerRigidbody2D.velocity.x;
        //verticalSpeed = player.playerRigidbody2D.velocity.y;
        UpdateAnimatorState(player, "IsRunning");
    }
    public override void ResetState() {
        horizontalSpeed = 0f;
        verticalSpeed = 0f;
    }

    public override void UpdateState(PlayerStateManager player) {

        // Check for user inputs - priority: crouch > jump > idle
        if (Input.GetButtonDown("Crouch")) {
            player.SwitchState(player.crouchState);
        } else if (Input.GetButtonDown("Jump")) {
            player.SwitchState(player.jumpState);
        } else if (Input.GetAxisRaw("Horizontal") == 0) {
            player.SwitchState(player.idleState);
        }

        // Check for falling: if player is not on ground, and falling velocity < threshold(debounce)
        if (!GroundCheck(player) && player.playerRigidbody2D.velocity.y < fallSpeedThreshold) {
            player.SwitchState(player.fallState);
        }

        horizontalSpeed = Input.GetAxisRaw("Horizontal") * player.runSpeed;
        //bool isFacingRight = !player.playerSpriteRenderer.flipX;
        //if ((horizontalSpeed > 0 && !isFacingRight) || (horizontalSpeed < 0 && isFacingRight)) {
        //    player.playerSpriteRenderer.flipX = !player.playerSpriteRenderer.flipX;
        //}
    }

    public override void OnCollisionEnter(PlayerStateManager player, Collision collision) {

    }
}
