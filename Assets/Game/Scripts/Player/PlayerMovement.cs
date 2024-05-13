using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    [SerializeField] private float RotationSmoothTime = 0.1f;
    [SerializeField] private float RotationSmoothVelocity;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float walkSprintTransition;
    [SerializeField] private InputManager input;
    private Rigidbody rb3d;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform GroundDetector;
    [SerializeField] private float detectorRadius;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector3 upperStepOffset;
    [SerializeField] private float stepCheckerDist;
    [SerializeField] private float stepForce;
    private bool isGrounded;
    private PlayerStance playerStance;
    [SerializeField] private Transform climbDetector;
    [SerializeField] private float climbCheckDistance;
    [SerializeField] private LayerMask climbableLayer;
    [SerializeField] private Vector3 climbOffset;
    [SerializeField] private float climbSpeed;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private CameraManager cameraManager;
    private Vector3 movementDirection = Vector3.zero;
   private void Move(Vector2 axisDirection){
    

    bool isPlayerStanding = playerStance == PlayerStance.Stand;
    bool isPlayerClimbing = playerStance == PlayerStance.Climb;
    if(isPlayerStanding){
        switch(cameraManager.CameraState){
            case CameraState.ThirdPerson:        
                if(axisDirection.magnitude >=0.1f){
                    float rotAngle = Mathf.Atan2(axisDirection.x,axisDirection.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
                    float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotAngle, ref RotationSmoothVelocity, RotationSmoothTime);
                    transform.rotation = Quaternion.Euler(0f,smoothAngle,0f);
                    movementDirection = Quaternion.Euler(0f,rotAngle,0f) * Vector3.forward;
                    rb3d.AddForce(speed * Time.deltaTime * movementDirection);
                }
                break;
            case CameraState.FirstPerson:
                transform.rotation = Quaternion.Euler(0f,cameraTransform.eulerAngles.y,0f);
                Vector3 verticalDir = axisDirection.y*transform.forward;
                Vector3 horizontalDir = axisDirection.x*transform.right;
                movementDirection = verticalDir + horizontalDir;
                rb3d.AddForce(speed * Time.deltaTime * movementDirection);
                break;
            default:
                break;
        }

    } else if(isPlayerClimbing){
        Vector3 horizontal = axisDirection.x * transform.right;
        Vector3 vertical = axisDirection.y * transform.up;

        movementDirection = horizontal + vertical;
        rb3d.AddForce(climbSpeed * Time.deltaTime * movementDirection);
    }
    
   }
    private void Sprint(bool isSprint){
        if(isSprint){
            if(speed < sprintSpeed){
                speed += walkSprintTransition * Time.deltaTime;
            }
        } else {
            if(speed > walkSpeed){
                speed -= walkSprintTransition * Time.deltaTime;
            }
        }
    }

    private void Jump(){
        if(isGrounded){
            Vector3 jumpDirection = Vector3.up;
            rb3d.AddForce(jumpForce * Time.deltaTime * jumpDirection);
        }
        
    }
    private void Climb(){
        bool FrontOfClimbingWall = Physics.Raycast(climbDetector.position,transform.forward,out RaycastHit hit, climbCheckDistance, climbableLayer);
        bool notClimbing = playerStance != PlayerStance.Climb;

        if(FrontOfClimbingWall && notClimbing && isGrounded){
            Vector3 offset = (transform.forward * climbOffset.z) + (Vector3.up * climbOffset.y);
            transform.position = hit.point - offset;
            playerStance = PlayerStance.Climb;
            rb3d.useGravity = false;
            cameraManager.SetFPSClampedCamera(true,transform.rotation.eulerAngles);
            cameraManager.SetTPSFOV(70);
        }
    }
    
    private void CancelClimb(){
        if(playerStance == PlayerStance.Climb){
            playerStance = PlayerStance.Stand;
            rb3d.useGravity = true;
            transform.position -=transform.forward *1f;
            cameraManager.SetFPSClampedCamera(false,transform.rotation.eulerAngles);
            cameraManager.SetTPSFOV(40);
        }
    }    
    private void CheckIsGrounded(){
        isGrounded = Physics.CheckSphere(GroundDetector.position, detectorRadius, groundLayer);
    }
   private void Start(){
    input.OnMoveInput += Move;
    input.OnSprintInput +=Sprint;
    input.OnJumpInput += Jump;
    input.OnClimbInput += Climb;
    input.OnCancelClimb += CancelClimb;
   }

   private void OnDestroy(){
    input.OnMoveInput -= Move;
    input.OnSprintInput -=Sprint;
    input.OnJumpInput -= Jump;
    input.OnClimbInput -= Climb;
    input.OnCancelClimb -= CancelClimb;
   }
   private void Awake(){
    speed = walkSpeed;
    rb3d = GetComponent<Rigidbody>();
    playerStance = PlayerStance.Stand;
    HideAndLockCursor();
   }

   private void Update(){
    CheckIsGrounded();
    CheckStep();
   }

   private void CheckStep(){
    bool isHitLowerStep = Physics.Raycast(GroundDetector.position,transform.forward,stepCheckerDist);
    bool isHitUpperStep = Physics.Raycast(GroundDetector.position + upperStepOffset,transform.forward,stepCheckerDist);

    if(isHitLowerStep && !isHitUpperStep){
        rb3d.AddForce(0,stepForce,0);
    }
   }
   private void HideAndLockCursor(){
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
   }
}
