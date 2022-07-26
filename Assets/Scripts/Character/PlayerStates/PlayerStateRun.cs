using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateRun : PlayerState {

    public override void EnterState(PlayerStateManager player) {
        Debug.Log("Entering Run state");

        UpdateAnimatorState(player, "IsRunning");
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


        

        Vector2 colliderPos = player.playerCircleCollider2D.transform.position;    // center of the CircleCollider2D(center of player)
        colliderPos += player.playerCircleCollider2D.offset;                       // add offset to find 'actual' center of CircleCollider2D
        // CircleCast around CircleCollider to check for whatIsGround colliders
        RaycastHit2D hit = Physics2D.CircleCast(colliderPos, player.playerCircleCollider2D.radius + 0.01f, Vector2.down, 0.01f, player.whatIsGround);
        // If no hit, then not on ground -> falling
        if (!hit) {
            player.SwitchState(player.fallState);
        }



        horizontalSpeed = Input.GetAxisRaw("Horizontal") * player.runSpeed;
    }

    public override void OnCollisionEnter(PlayerStateManager player, Collision collision) {

    }

    //private void Flip() {
    //    // Switch the way the player is labelled as facing.
    //    isFacingRight = !isFacingRight;
    //    playerSpriteRenderer.flipX = !isFacingRight;
    //}





}
