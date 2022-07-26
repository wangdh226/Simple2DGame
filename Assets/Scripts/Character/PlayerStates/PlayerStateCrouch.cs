using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateCrouch : PlayerState {

    private bool isUnderCeiling = false;

    public override void EnterState(PlayerStateManager player) {
        Debug.Log("Entering Crouch state");

        UpdateAnimatorState(player, "IsCrouching");
    }

    public override void UpdateState(PlayerStateManager player) {
        // Check if player is in contact with a ceiling
        Vector2 colliderPos = player.playerBoxCollider2D.transform.position;
        colliderPos += player.playerBoxCollider2D.offset;
        RaycastHit2D hit = Physics2D.BoxCast(colliderPos, player.playerBoxCollider2D.size, 0f, Vector2.up, 0.01f, player.whatIsGround);
        // Update state if player is under ceiling
        isUnderCeiling = hit.Equals(null);

        // Check for crouch input and set state
        if (!Input.GetButton("Crouch")) {
            // If player is not holding Crouch key, attempt to uncrouch
            if (!isUnderCeiling) { // don't uncrouch if there is something above the player
                player.SwitchState(player.idleState);
            }
        } else if (Input.GetButtonDown("Jump")) {
            //player.SwitchState(player.jumpState); // crouch jump?
        } else if (Input.GetAxisRaw("Horizontal") != 0) {
            horizontalSpeed = Input.GetAxisRaw("Horizontal") * player.runSpeed;
        }
    }

    public override void OnCollisionEnter(PlayerStateManager player, Collision collision) {

    }
}
