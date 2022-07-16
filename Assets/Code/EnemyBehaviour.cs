using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private Rigidbody rb;
    private Collider coll;
    private Transform camTransform;
    
    [Header("Movement")]
    [SerializeField] private float topSpeed;
    [SerializeField] private float accelerationTime;
    [SerializeField] private float decelerationTime;
    
    [Header("Jump")]
    [SerializeField] private float jumpAscendTime;
    [SerializeField] private float jumpFallTime;
    [SerializeField] private float jumpHeight;

    private float rotationX;
    private float rotationY;
    private float jumpTimer;
    private bool intentionToJump;
    private Vector2 moveVector;
    private bool grounded = true;

    private void Start() {
        camTransform = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>(); 
    }

    private void Update() {
        CheckResetJump();

        // jump buffer
        intentionToJump = false;

        var dPos = (transform.position - GameObject.FindWithTag("Player").transform.position);
        moveVector = new Vector2(dPos.x, dPos.z);
    }

    private void FixedUpdate() {
        HorizontalMove();
        HandleJump();
    }

    private void HorizontalMove() {
        var transformedInput = camTransform.TransformDirection(new Vector3(moveVector.normalized.x, 0, moveVector.normalized.y));
        var input = new Vector2(transformedInput.x, transformedInput.z) * topSpeed;
        var relativeVelocity = rb.velocity;
        var deltavTarget = new Vector2(input.x - relativeVelocity.x, input.y - relativeVelocity.z);
        
        var deltavCap = topSpeed * Time.fixedDeltaTime / accelerationTime;
        var deltavCapDeceleration = topSpeed * Time.fixedDeltaTime / decelerationTime;
        var deltavRb = new Vector3(Mathf.MoveTowards(0, deltavTarget.x, input.x == 0 ? deltavCapDeceleration : deltavCap),
            0,
            Mathf.MoveTowards(0, deltavTarget.y, input.y == 0 ? deltavCapDeceleration : deltavCap));

        rb.AddForce(deltavRb, ForceMode.VelocityChange);
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
                gravity = -2 * jumpHeight / (jumpFallTime * jumpFallTime);
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
