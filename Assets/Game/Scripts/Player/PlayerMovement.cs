using System.Collections;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
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
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float glideSpeed;
    [SerializeField] private float airDrag;
    [SerializeField] private Vector3 glideRotationSpeed;
    [SerializeField] private float minGlideRotationX;
    [SerializeField] private float maxGlideRotationX;
    [SerializeField] private float resetComboInterval;
    [SerializeField] private Transform hitDetector;
    [SerializeField] private float hitDetectorRadius;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private PlayerAudioManager audioManager;
    private Coroutine resetCombo;
    private bool isPunching;
    private int combo = 0;
    private CapsuleCollider col;
    private Animator animator;
    private Vector3 movementDirection = Vector3.zero;
    private void Hit(){
        Collider[] hitObjects = Physics.OverlapSphere(hitDetector.position,hitDetectorRadius,hitLayer);
        for(int i = 0; i<hitObjects.Length; i++){
            if(hitObjects[i].gameObject != null){
                Destroy(hitObjects[i].gameObject);
            }
        }
    }
    private void ChangePerspective(){
        animator.SetTrigger("ChangePerspective");
    }
    private void Crouch(){
        if(playerStance == PlayerStance.Stand){
            col.height = 1.3f;
            col.center = Vector3.up *0.66f;
            playerStance = PlayerStance.Crouch;
            animator.SetBool("isCrouch",true);
            speed = crouchSpeed;
        } else if(playerStance == PlayerStance.Crouch){
            col.height = 1.8f;
            col.center = Vector3.up *0.9f;
            playerStance = PlayerStance.Stand;
            animator.SetBool("isCrouch",false);
            speed = walkSpeed;
        }
    }
   private void Move(Vector2 axisDirection){
    

    bool isPlayerStanding = playerStance == PlayerStance.Stand;
    bool isPlayerClimbing = playerStance == PlayerStance.Climb;
    bool isPlayerCrouching = playerStance == PlayerStance.Crouch;
    bool isPlayerGliding = playerStance == PlayerStance.Glide;
    if((isPlayerStanding || isPlayerCrouching) && !isPunching){
        Vector3 velocity = new Vector3(rb3d.velocity.x,0,rb3d.velocity.z);
        animator.SetFloat("Velocity",velocity.magnitude * axisDirection.magnitude);
        animator.SetFloat("VelocityX",velocity.magnitude * axisDirection.x);
        animator.SetFloat("VelocityZ",velocity.magnitude * axisDirection.y);
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
        Vector3 velocity = new Vector3(rb3d.velocity.x,rb3d.velocity.y,0);
        animator.SetFloat("ClimbVeloY",velocity.magnitude * axisDirection.y);
        animator.SetFloat("ClimbVeloX",velocity.magnitude * axisDirection.x);
    } else if(isPlayerGliding){
        Vector3 rotDeg = transform.rotation.eulerAngles;
        rotDeg.x += glideRotationSpeed.x * axisDirection.y*Time.deltaTime;
        rotDeg.x = Mathf.Clamp(rotDeg.x,minGlideRotationX,maxGlideRotationX);
        rotDeg.z += glideRotationSpeed.z * axisDirection.x * Time.deltaTime;
        rotDeg.y += glideRotationSpeed.y * axisDirection.x * Time.deltaTime;
        transform.rotation = Quaternion.Euler(rotDeg);
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
            animator.SetTrigger("Jump");
        }
        
    }
    private void Climb(){
        bool FrontOfClimbingWall = Physics.Raycast(climbDetector.position,transform.forward,out RaycastHit hit, climbCheckDistance, climbableLayer);
        bool notClimbing = playerStance != PlayerStance.Climb;

        if(FrontOfClimbingWall && notClimbing && isGrounded){
            col.center = Vector3.up * 1.3f;
            animator.SetBool("isClimb",true);
            Vector3 offset = (transform.forward * climbOffset.z) + (Vector3.up * climbOffset.y);
            transform.position = hit.point - offset;
            playerStance = PlayerStance.Climb;
            rb3d.useGravity = false;
            cameraManager.SetFPSClampedCamera(true,transform.rotation.eulerAngles);
            cameraManager.SetTPSFOV(80);
        }
    }
    
    private void CancelClimb(){
        if(playerStance == PlayerStance.Climb){
            col.center = Vector3.up * 0.9f;
            animator.SetBool("isClimb",false);
            playerStance = PlayerStance.Stand;
            rb3d.useGravity = true;
            transform.position -=transform.forward *1f;
            cameraManager.SetFPSClampedCamera(false,transform.rotation.eulerAngles);
            cameraManager.SetTPSFOV(60);
        }
    }   
    private void StartGlide(){
        if(playerStance != PlayerStance.Glide && !isGrounded){
            playerStance = PlayerStance.Glide;
            animator.SetBool("isGliding",true);
            cameraManager.SetFPSClampedCamera(true,transform.rotation.eulerAngles);
            audioManager.PlayGlideSFX();
        }
    } 
    private void CancelGlide(){
        if(playerStance == PlayerStance.Glide){
            playerStance = PlayerStance.Stand;
            animator.SetBool("isGliding",false);
            cameraManager.SetFPSClampedCamera(false,transform.rotation.eulerAngles);
            audioManager.StopGlideSFX();
        }
    }
    private void Glide(){
        if(playerStance == PlayerStance.Glide){
            Vector3 playerRot = transform.rotation.eulerAngles;
            float lift = playerRot.x;
            Vector3 upForce = transform.up * (lift+airDrag);
            Vector3 forwardForce = transform.forward * glideSpeed;
            Vector3 totalForce = upForce + forwardForce;
            rb3d.AddForce(totalForce * Time.deltaTime);
        }
    }
    private void CheckIsGrounded(){
        isGrounded = Physics.CheckSphere(GroundDetector.position, detectorRadius, groundLayer);
        animator.SetBool("isGrounded",isGrounded);
        if(isGrounded){
            CancelGlide();
        }
    }
   private void Punch(){
        if(!isPunching && playerStance == PlayerStance.Stand){
            isPunching = true;
            if(combo<3){
                combo++;
            } else {
                combo = 1;
            }
            animator.SetInteger("Combo",combo);
            animator.SetTrigger("Punch");
        }
   }
   private void EndPunch(){
    isPunching = false;
    if(resetCombo != null){
        StopCoroutine(resetCombo);
    }
    resetCombo = StartCoroutine(ResetCombo());
    
   }
   private IEnumerator ResetCombo(){
    yield return new WaitForSeconds(resetComboInterval);
    combo = 0;
   }
   private void Start(){
    input.OnMoveInput += Move;
    input.OnSprintInput +=Sprint;
    input.OnJumpInput += Jump;
    input.OnClimbInput += Climb;
    input.OnCancelClimb += CancelClimb;
    cameraManager.OnChangePerspective += ChangePerspective;
    input.OnCrouchInput += Crouch;
    input.OnGlideInput += StartGlide;
    input.OnCancelGlide +=CancelGlide;
    input.OnPunchInput += Punch;
   }

   private void OnDestroy(){
    input.OnMoveInput -= Move;
    input.OnSprintInput -=Sprint;
    input.OnJumpInput -= Jump;
    input.OnClimbInput -= Climb;
    input.OnCancelClimb -= CancelClimb;
    cameraManager.OnChangePerspective -= ChangePerspective;
    input.OnCrouchInput -= Crouch;
    input.OnGlideInput -= StartGlide;
    input.OnCancelGlide -=CancelGlide;
    input.OnPunchInput -= Punch;
   }
   private void Awake(){
    speed = walkSpeed;
    rb3d = GetComponent<Rigidbody>();
    playerStance = PlayerStance.Stand;
    HideAndLockCursor();
    animator = GetComponent<Animator>();
    col = GetComponent<CapsuleCollider>();
   }

   private void Update(){
    CheckIsGrounded();
    CheckStep();
    Glide();
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
