using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Update is called once per frame
    [HideInInspector] public Action<Vector2> OnMoveInput;
    [HideInInspector] public Action<bool> OnSprintInput;
    [HideInInspector] public Action OnJumpInput;
    [HideInInspector] public Action OnClimbInput;
    [HideInInspector] public Action OnCancelClimb;
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
        Vector2 InputAxis = new Vector2(horizontalAxis,verticalAxis);
        if(OnMoveInput != null){
            OnMoveInput(InputAxis);
        }
    }

    private void CheckSprintInput(){
        bool isHoldingSprintInput = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if(isHoldingSprintInput){
            if(OnSprintInput != null){
                OnSprintInput(true);
            }
        } else {
            if(OnSprintInput != null){
                OnSprintInput(false);
            }
        }
    }

    private void CheckJumpInput(){
        bool isJumpingInput = Input.GetKey(KeyCode.Space);

        if(isJumpingInput){
            if(OnJumpInput != null){
                OnJumpInput();
            }
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
            if(OnClimbInput != null){
                OnClimbInput();
            }
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
            if(OnCancelClimb != null){
                OnCancelClimb();
            }
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
