using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateIdle : PlayerState {

    public override void EnterState(PlayerStateManager player) {
        Debug.Log("Entering Idle state");

        horizontalSpeed_target = 0f;
        verticalSpeed_target = 0f;

        player.animator.SetBool("IsIdling", true);
    }

    public override void UpdateState(PlayerStateManager player) {


        //if (!Physics2D.OverlapCircle(player.groundCheck.position, player.GROUNDED_RADIUS, player.whatIsGround)) {
        //    // If no colliders exist => player is in the air
        //    player.SwitchState(player.fallState);
        //}
    }

    public override void OnCollisionEnter(PlayerStateManager player, Collision collision) {

    }


}
