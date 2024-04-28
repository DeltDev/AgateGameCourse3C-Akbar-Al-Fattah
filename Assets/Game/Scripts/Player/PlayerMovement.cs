using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    [SerializeField] private float RotationSmoothTime = 0.1f;
    [SerializeField] private float RotationSmoothVelocity;
   [SerializeField] private float walkSpeed;
   [SerializeField] private InputManager input;
    private Rigidbody rb3d;
   private void Move(Vector2 axisDirection){
    if(axisDirection.magnitude >=0.1f){
        float rotAngle = Mathf.Atan2(axisDirection.x,axisDirection.y) * Mathf.Rad2Deg;
        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotAngle, ref RotationSmoothVelocity, RotationSmoothTime);
        transform.rotation = Quaternion.Euler(0f,smoothAngle,0f);
        Vector3 movementDirection = Quaternion.Euler(0f,rotAngle,0f) * Vector3.forward;
        rb3d.AddForce(movementDirection * Time.deltaTime * walkSpeed);
    }
   }

   private void Start(){
    input.OnMoveInput += Move;
   }

   private void OnDestroy(){
    input.OnMoveInput -= Move;
   }
   private void Awake(){
    rb3d = GetComponent<Rigidbody>();
   }
}
