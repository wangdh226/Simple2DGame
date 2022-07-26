using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateIdle : PlayerState {

    public override void EnterState(PlayerStateManager player) {
        Debug.Log("Entering Idle state");

        horizontalSpeed = 0f;
        verticalSpeed = 0f;

        UpdateAnimatorState(player, "IsIdling");
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



        Vector2 colliderPos = player.playerCircleCollider2D.transform.position;    // center of the CircleCollider2D(center of player)
        colliderPos += player.playerCircleCollider2D.offset;                       // add offset to find 'actual' center of CircleCollider2D
        // CircleCast around CircleCollider to check for whatIsGround colliders
        RaycastHit2D hit = Physics2D.CircleCast(colliderPos, player.playerCircleCollider2D.radius + 0.01f, Vector2.down, 0.01f, player.whatIsGround);
        // If no hit, then not on ground -> falling
        if (!hit) {
            player.SwitchState(player.fallState);
        }


    }

    public override void OnCollisionEnter(PlayerStateManager player, Collision collision) {

    }


}
