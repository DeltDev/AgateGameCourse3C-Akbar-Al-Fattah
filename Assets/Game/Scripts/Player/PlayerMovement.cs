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
   private void Move(Vector2 axisDirection){
    if(axisDirection.magnitude >=0.1f){
        float rotAngle = Mathf.Atan2(axisDirection.x,axisDirection.y) * Mathf.Rad2Deg;
        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotAngle, ref RotationSmoothVelocity, RotationSmoothTime);
        transform.rotation = Quaternion.Euler(0f,smoothAngle,0f);
        Vector3 movementDirection = Quaternion.Euler(0f,rotAngle,0f) * Vector3.forward;
        rb3d.AddForce(movementDirection * Time.deltaTime * speed);
    }
   }
    private void Sprint(bool isSprint){
        if(isSprint){
            if(speed < sprintSpeed){
                speed = speed + walkSprintTransition * Time.deltaTime;
            }
        } else {
            if(speed > walkSpeed){
                speed = speed - walkSprintTransition * Time.deltaTime;
            }
        }
    }

    private void Jump(){
        if(isGrounded){
            Vector3 jumpDirection = Vector3.up;
            rb3d.AddForce(jumpDirection * jumpForce * Time.deltaTime);
        }
        
    }

    private void CheckIsGrounded(){
        isGrounded = Physics.CheckSphere(GroundDetector.position, detectorRadius, groundLayer);
    }
   private void Start(){
    input.OnMoveInput += Move;
    input.OnSprintInput +=Sprint;
    input.OnJumpInput += Jump;
   }

   private void OnDestroy(){
    input.OnMoveInput -= Move;
    input.OnSprintInput -=Sprint;
    input.OnJumpInput -= Jump;
   }
   private void Awake(){
    speed = walkSpeed;
    rb3d = GetComponent<Rigidbody>();
   }

   private void Update(){
    CheckIsGrounded();
    checkStep();
   }

   private void checkStep(){
    bool isHitLowerStep = Physics.Raycast(GroundDetector.position,transform.forward,stepCheckerDist);
    bool isHitUpperStep = Physics.Raycast(GroundDetector.position + upperStepOffset,transform.forward,stepCheckerDist);

    if(isHitLowerStep && !isHitUpperStep){
        rb3d.AddForce(0,stepForce,0);
    }
   }
}
