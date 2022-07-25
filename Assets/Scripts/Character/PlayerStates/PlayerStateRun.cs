using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateRun : PlayerState {

    public override void EnterState(PlayerStateManager player) {
        Debug.Log("Entering Run state");

        player.animator.SetBool("IsRunning", true);
    }

    public override void UpdateState(PlayerStateManager player) {
        horizontalSpeed_target = Input.GetAxisRaw("Horizontal") * runSpeed;



        wasGrounded = isGrounded;
        if (Physics2D.OverlapCircle(groundCheck.position, GROUNDED_RADIUS, whatIsGround)) {
            isGrounded = true;
            if (!wasGrounded) {
                // if !wasGrounded in previous check => player was in air => player just landed
                isJumping = false;
                isFalling = false;
                OnLanding();
            }
        } else {
            isGrounded = false;
            isFalling = true;
            // If no colliders exist => player is in the air
            OnFalling();
        }
    }

    public override void OnCollisionEnter(PlayerStateManager player, Collision collision) {

    }

    private void Flip() {
        // Switch the way the player is labelled as facing.
        isFacingRight = !isFacingRight;
        playerSpriteRenderer.flipX = !isFacingRight;
    }





}
