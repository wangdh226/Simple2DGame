using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] public CharacterController2D controller;
    [SerializeField] public Animator animator;
    [SerializeField] private float runSpeed = 40f;
    [SerializeField] private Rigidbody2D playerRigidbody2D;

    [SerializeField] private float walkDebounce;

    private float horizontalSpeed = 0f;
    private float horizontalSpeed_target = 0f;
    private float verticalSpeed = 0f;
    private float verticalSpeed_target = 0f;
    private float crouchSpeed = 0.33f;

    private Vector3 m_Velocity = Vector3.zero;
    private float m_MovementSmoothing = .05f;

    private bool jumping = false;
    private bool crouching = false;
    private bool grounded = false;

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

    // Update is called once per frame
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


        if (Input.GetButtonDown("Jump") || Input.GetButton("Jump")) {
            jumping = true;
            animator.SetBool("IsJumping", true);
        } 

        if (Input.GetButtonDown("Crouch")) {
            crouching = true;
        } else if (Input.GetButtonUp("Crouch")) {
            crouching = false;
        }

        gameObject.GetComponent<Rigidbody2D>();


    }

    public void OnLanding() {
        animator.SetBool("IsJumping", false);
        jumping = false;
    }

    public void OnCrouching(bool isCrouching) {
        crouching = isCrouching;
        animator.SetBool("IsCrouching", isCrouching);
    }


    void FixedUpdate() {
        controller.Move(horizontalSpeed_target * Time.fixedDeltaTime, crouching, jumping);


        //float moveSpeedX = horizontalSpeed_target * Time.fixedDeltaTime * (crouching ? crouchSpeed : 1);
        //float moveSpeedY = verticalSpeed_target * Time.fixedDeltaTime * (crouching ? crouchSpeed : 1);
        //MoveHorizontal(moveSpeedX);
        //MoveVertical(moveSpeedY);
    }

    private void MoveHorizontal(float move) {
        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(move * 10f, playerRigidbody2D.velocity.y);
        // And then smoothing it out and applying it to the character
        playerRigidbody2D.velocity = Vector3.SmoothDamp(playerRigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
    }

    private void MoveVertical(float move) {
        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(playerRigidbody2D.velocity.x, move * 10f);
        // And then smoothing it out and applying it to the character
        playerRigidbody2D.velocity = Vector3.SmoothDamp(playerRigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
    }


}
