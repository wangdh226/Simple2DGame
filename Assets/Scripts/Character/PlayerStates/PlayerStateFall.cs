using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateFall : PlayerState {

    public override void EnterState(PlayerStateManager player) {
        Debug.Log("Entering Fall state");

        UpdateAnimatorState(player, "IsFalling");
    }

    public override void UpdateState(PlayerStateManager player) {
        Vector2 colliderPos = player.playerCircleCollider2D.transform.position;    // center of the CircleCollider2D(center of player)
        colliderPos += player.playerCircleCollider2D.offset;                       // add offset to find 'actual' center of CircleCollider2D
        // CircleCast around CircleCollider to check for whatIsGround colliders
        RaycastHit2D hit = Physics2D.CircleCast(colliderPos, player.playerCircleCollider2D.radius + 0.01f, Vector2.down, 0.01f, player.whatIsGround);

        if (hit) {
            player.SwitchState(player.idleState);
        }
    }

    public override void OnCollisionEnter(PlayerStateManager player, Collision collision) {

    }
}
