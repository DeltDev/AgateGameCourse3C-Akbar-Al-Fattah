using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        CheckMovementInput();
        CheckCancelInput();
        CheckChangePOVInput();
        CheckClimbInput();
        CheckCrouchInput();
        CheckGlideInput();
        CheckJumpInput();
        CheckMainMenuInput();
        CheckPunchInput();
        CheckSprintInput();
    }

    private void CheckMovementInput(){
        float verticalAxis = Input.GetAxis("Vertical");
        float horizontalAxis = Input.GetAxis("Horizontal");
        Debug.Log("Vertical Axis: " + verticalAxis);
        Debug.Log("horizontal Axis: " + horizontalAxis);
    }

    private void CheckSprintInput(){
        bool isHoldingSprintInput = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if(isHoldingSprintInput){
            Debug.Log("Is Sprinting");
        } else {
            Debug.Log("Not Sprinting");
        }
    }

    private void CheckJumpInput(){
        bool isJumpingInput = Input.GetKey(KeyCode.Space);

        if(isJumpingInput){
            Debug.Log("Jumping");
        } else {
            Debug.Log("Not Jumping");
        }
    }

    private void CheckCrouchInput(){
        bool isCrouchingInput = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        if(isCrouchingInput){
            Debug.Log("Crouching");
        } else {
            Debug.Log("Not Crouching");
        }
    }

    private void CheckChangePOVInput(){
        bool isPOVChangeInput = Input.GetKey(KeyCode.Q);

        if(isPOVChangeInput){
            Debug.Log("POVChange");
        } else {
            Debug.Log("Not POVChange");
        }
    }

    private void CheckClimbInput(){
        bool isClimbInput = Input.GetKey(KeyCode.E);

        if(isClimbInput){
            Debug.Log("Climb");
        } else {
            Debug.Log("Not Climb");
        }
    }

    private void CheckGlideInput(){
        bool isGlideInput = Input.GetKey(KeyCode.G);

        if(isGlideInput){
            Debug.Log("Glide");
        } else {
            Debug.Log("Not Glide");
        }
    }

    private void CheckCancelInput(){
        bool isCancelInput = Input.GetKey(KeyCode.C);

        if(isCancelInput){
            Debug.Log("Cancel");
        } else {
            Debug.Log("Not Cancel");
        }
    }

    private void CheckPunchInput(){
        bool isPunchInput = Input.GetKey(KeyCode.Mouse0);

        if(isPunchInput){
            Debug.Log("Punch");
        } else {
            Debug.Log("Not Punch");
        }
    }

    private void CheckMainMenuInput(){
        bool isMainMenuInput = Input.GetKey(KeyCode.Escape);

        if(isMainMenuInput){
            Debug.Log("MainMenu");
        } else {
            Debug.Log("Not MainMenu");
        }
    }
}
