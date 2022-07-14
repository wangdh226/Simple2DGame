using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerMovementController : MonoBehaviour
{
    [SerializeField] public CharacterController2D controller;
    [SerializeField] public Animator animator;
    [SerializeField] private float runSpeed = 40f;
    [SerializeField] private float jumpSpeed = 40f;
    [SerializeField] private Rigidbody2D playerRigidbody2D;

    [SerializeField] private float walkDebounce;

    private float horizontalSpeed = 0f;
    private float horizontalSpeed_target = 0f;
    private float verticalSpeed = 0f;
    private float verticalSpeed_target = 0f;
    private float crouchSpeed = 0.33f;

    private Vector2 m_Velocity = Vector2.zero;
    private float m_MovementSmoothing = .05f;

    private bool jumping = false;
    private bool crouching = false;
    private bool grounded = false;

    private bool m_FacingRight = true;

    private void OnValidate() {
        if (controller == null) {
            TryGetComponent(out controller);
        }
        if (animator == null) {
            TryGetComponent(out animator);
        }
        if (playerRigidbody2D == null) {
            TryGetComponent(out playerRigidbody2D);
        }
    }

    void Update() {

        // Constantly update the velocities;
        horizontalSpeed = playerRigidbody2D.velocity.x;
        verticalSpeed = playerRigidbody2D.velocity.y;
        animator.SetFloat("Horizontal_Speed", Mathf.Abs(horizontalSpeed));
        animator.SetFloat("Vertical_Speed", Mathf.Abs(verticalSpeed) > walkDebounce ? verticalSpeed : 0f);
        //if(Mathf.Abs(verticalSpeed) > walkDebounce && !jumping) {
        //    Debug.Log(verticalSpeed + " > " + walkDebounce);
        //}

        horizontalSpeed_target = Input.GetAxisRaw("Horizontal") * runSpeed;

        //if (Input.GetButtonDown("Jump") && !jumping) {
        //    jumping = true;
        //    animator.SetBool("IsJumping", true);
        //    verticalSpeed_target = jumpSpeed;
        //} else if (jumping) {
        //    verticalSpeed_target = 0f;
        //}

        if (Input.GetButtonDown("Crouch")) {
            crouching = true;
        } else if (Input.GetButtonUp("Crouch")) {
            crouching = false;
        }
    }

    void FixedUpdate() {
        float moveSpeedX = horizontalSpeed_target * Time.fixedDeltaTime * (crouching ? crouchSpeed : 1);
        //float moveSpeedY = verticalSpeed_target * Time.fixedDeltaTime * (crouching ? crouchSpeed : 1);
        //Debug.Log("1:" + moveSpeedX);
        MoveHorizontal(moveSpeedX);
        //MoveVertical(moveSpeedY);
    }

    private void MoveHorizontal(float move) {
        Vector2 targetVelocity = new Vector2(move * 10f, playerRigidbody2D.velocity.y);
        //Debug.Log("2:" + targetVelocity);
        playerRigidbody2D.velocity = Vector2.SmoothDamp(playerRigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        //Debug.Log("3:" + playerRigidbody2D.velocity);

        // Flip player sprite if moving in opposite direction of facing
        if ((move > 0 && !m_FacingRight) || (move < 0 && m_FacingRight)) {
            Flip();
        }
    }

    private void MoveVertical(float move) {
        Vector2 targetVelocity = new Vector2(playerRigidbody2D.velocity.x, move * 10f);
        playerRigidbody2D.velocity = Vector2.SmoothDamp(playerRigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
    }



    // Utility/Helper methods
    public void OnLanding() {
        animator.SetBool("IsJumping", false);
        jumping = false;
    }

    public void OnCrouching(bool isCrouching) {
        crouching = isCrouching;
        animator.SetBool("IsCrouching", isCrouching);
    }

    private void Flip() {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }


}
