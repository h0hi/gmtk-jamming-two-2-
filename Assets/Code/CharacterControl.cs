using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    private Rigidbody rb;
    private Collider coll;
    private Transform camTransform;

    [Header("Look")]
    [SerializeField] private float sensitivity;
    [SerializeField] private float bobFrequency;
    [SerializeField] private float bobAmplitude;
    
    [Header("Movement")]
    [SerializeField] private float topSpeed;
    [SerializeField] private float accelerationTime;
    [SerializeField] private float decelerationTime;
    
    [Header("Jump")]
    [SerializeField] private float jumpAscendTime;
    [SerializeField] private float jumpFallTime;
    [SerializeField] private float jumpFloatTime;
    [SerializeField] private float jumpHeight;

    private float rotationX;
    private float rotationY;
    private float jumpTimer;
    private bool intentionToJump;
    private Vector2 moveVector;
    private bool grounded = true;

    private void Start() {
        camTransform = GetComponentInChildren<Camera>().transform;
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 
    }

    private void Update() {
        Look();
        CheckResetJump();

        // jump buffer
        intentionToJump = Input.GetKey(KeyCode.Space);

        moveVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        var movingWithBob = (moveVector.x != 0 || moveVector.y != 0) && grounded;

        var cameraBob = movingWithBob ? Mathf.Sin(Time.time * bobFrequency) * bobAmplitude : 0;
        var camLocalTarget = Vector3.up * (1.5f + cameraBob) + camTransform.InverseTransformVector(rb.velocity) * 0.05f;
        var newPos = Vector3.MoveTowards(camTransform.localPosition, camLocalTarget, 2 * Time.deltaTime);
        camTransform.localPosition = newPos;
    }

    private void FixedUpdate() {
        HorizontalMove();
        HandleJump();
    }

    private void Look() {
        rotationX -= Input.GetAxisRaw("Mouse Y") * sensitivity * Time.deltaTime;
        rotationY += Input.GetAxisRaw("Mouse X") * sensitivity * Time.deltaTime;
        transform.localEulerAngles = Vector3.up * rotationY;
        camTransform.localEulerAngles = Vector3.right * rotationX;
    }

    private void HorizontalMove() {
        var input = moveVector.normalized * topSpeed;
        var relativeVelocity = transform.InverseTransformVector(rb.velocity);
        var deltavTarget = new Vector2(input.x - relativeVelocity.x, input.y - relativeVelocity.z);
        
        var deltavCap = topSpeed * Time.fixedDeltaTime / accelerationTime;
        var deltavCapDeceleration = topSpeed * Time.fixedDeltaTime / decelerationTime;
        var deltavRb = new Vector3(Mathf.MoveTowards(0, deltavTarget.x, input.x == 0 ? deltavCapDeceleration : deltavCap),
            0,
            Mathf.MoveTowards(0, deltavTarget.y, input.y == 0 ? deltavCapDeceleration : deltavCap));

        rb.AddRelativeForce(deltavRb, ForceMode.VelocityChange);
    }

    private void HandleJump() {

        grounded = CheckGrounded();

        if (grounded) {
            jumpTimer = 1;
        }

        if (intentionToJump && jumpTimer > 0) {
            var velocity = rb.velocity;
            velocity.y = 2 * jumpHeight / jumpAscendTime;
            jumpTimer = 0;
            rb.velocity = velocity;
        }

        if (!grounded) {
            var gravity = -2 * jumpHeight / (jumpAscendTime * jumpAscendTime);
            
            if (rb.velocity.y < 0) {
                if (intentionToJump) {
                    gravity = -2 * jumpHeight / (jumpFloatTime * jumpFloatTime);
                } else {
                    gravity = -2 * jumpHeight / (jumpFallTime * jumpFallTime);
                }
            }

            rb.AddForce(gravity * rb.mass * Vector3.up);
        }
    }

    private void CheckResetJump() {
        if (Input.GetKeyUp(KeyCode.Space)) {
            var velocity = rb.velocity;
            velocity.y = 0;
            rb.velocity = velocity;
        }
    }

    private bool CheckGrounded() {
        const float window = 0.05f;
        return Physics.Raycast(new Ray(new Vector3(coll.bounds.center.x, coll.bounds.min.y + window * 0.5f, coll.bounds.center.z), Vector3.down), window, 1);
    }
}
