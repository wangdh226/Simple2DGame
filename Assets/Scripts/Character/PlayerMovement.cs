using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public CharacterController2D controller;
    public Animator animator;

    public float runSpeed = 40f;

    float horizontalSpeed = 0f;
    bool jump = false;
    bool crouch = false;
    float verticalSpeed = 0f;

    // Update is called once per frame
    void Update() {
        horizontalSpeed = Input.GetAxisRaw("Horizontal") * runSpeed;
        animator.SetFloat("Horizontal_Speed", Mathf.Abs(horizontalSpeed));


        if (Input.GetButtonDown("Jump") || Input.GetButton("Jump")) {
            jump = true;
            animator.SetBool("IsJumping", true);
        } 
        //else if (Input.GetButtonUp("Jump")) {
        //    jump = false;
        //}

        if (Input.GetButtonDown("Crouch")) {
            crouch = true;
        } else if (Input.GetButtonUp("Crouch")) {
            crouch = false;
        }
    }

    public void OnLanding() {
        animator.SetBool("IsJumping", false);
        jump = false;
    }

    public void OnCrouching(bool isCrouching) {
        crouch = isCrouching;
        animator.SetBool("IsCrouching", isCrouching);
    }


    void FixedUpdate() {
        controller.Move(horizontalSpeed * Time.fixedDeltaTime, crouch, jump);
        
    }


}
