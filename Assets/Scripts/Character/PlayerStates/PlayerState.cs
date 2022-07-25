using UnityEngine;

public abstract class PlayerState {

    public float horizontalSpeed_target { get; set; }
    public float verticalSpeed_target { get; set; }

    public abstract void EnterState(PlayerStateManager player);
    public abstract void UpdateState(PlayerStateManager player);
    public abstract void OnCollisionEnter(PlayerStateManager player, Collision collision);
}
