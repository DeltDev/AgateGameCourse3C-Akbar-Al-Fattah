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
   private void Start(){
    input.OnMoveInput += Move;
    input.OnSprintInput +=Sprint;
   }

   private void OnDestroy(){
    input.OnMoveInput -= Move;
    input.OnSprintInput -=Sprint;
   }
   private void Awake(){
    speed = walkSpeed;
    rb3d = GetComponent<Rigidbody>();
   }
}
