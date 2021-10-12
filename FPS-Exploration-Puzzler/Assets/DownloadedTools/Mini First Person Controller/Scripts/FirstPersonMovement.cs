using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    public float speed = 5;

    [Header("Running")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;
    public KeyCode runningKey = KeyCode.LeftShift;

    public Vector3 gravity;
    Vector3 RaycastOrigin => transform.position + Vector3.up * 0.001f;
    public Vector3 nearestGround;
    RaycastHit RaycastHitPoint;

    public float adjust; 
    public float angleOfGround;
    Rigidbody rigidbody;
    
    [SerializeField]
    GroundCheck groundCheck;

    [SerializeField]
    Jump jump;

    /// <summary> Functions to override movement speed. Will use the last added override. </summary>
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();



    void Awake()
    {
        // Get the rigidbody on this.
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Update IsRunning from input.
        IsRunning = canRun && Input.GetKey(runningKey);


        var ray = new Ray(RaycastOrigin, Vector3.down);
        Physics.Raycast(ray, out RaycastHitPoint);
        nearestGround = RaycastHitPoint.point;
        angleOfGround = Vector3.Angle(Vector3.down, RaycastHitPoint.normal); 
        

        // Get targetMovingSpeed.
        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        // Get targetVelocity from input.
        Vector2 targetVelocity =new Vector2( Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);

        
        // Apply movement.
        //Movement needs to be different if grounded vs if in air. Y-position needs to be on the ground.
        if (groundCheck.isGrounded && angleOfGround > 130){
            rigidbody.velocity = transform.rotation * new Vector3(targetVelocity.x, 0, targetVelocity.y);
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
            rigidbody.AddForce(-RaycastHitPoint.normal * gravity.magnitude);
            rigidbody.position = new Vector3 (rigidbody.position.x, nearestGround.y + adjust, rigidbody.position.z);

        }
        else{
            rigidbody.velocity = transform.rotation * new Vector3(targetVelocity.x, rigidbody.velocity.y , targetVelocity.y);
            rigidbody.AddForce(gravity);
        }
    }
}