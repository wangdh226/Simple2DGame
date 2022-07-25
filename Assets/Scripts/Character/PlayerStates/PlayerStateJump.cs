using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateJump : PlayerState {

    public override void EnterState(PlayerStateManager player) {
        Debug.Log("Entering Jump state");

        player.animator.SetBool("IsJumping", true);
    }

    public override void UpdateState(PlayerStateManager player) {
        if (player.playerRigidbody2D.velocity.y < 0.5f) {
            player.SwitchState(player.fallState);
        }
    }

    public override void OnCollisionEnter(PlayerStateManager player, Collision collision) {

    }
}
