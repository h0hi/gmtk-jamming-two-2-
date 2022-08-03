using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterControl : MonoBehaviour, IEncounterEventListener
{
    private Rigidbody rb;
    private Collider coll;
    
    [Header("Movement")]
    [SerializeField] private float topSpeed;
    [SerializeField] private float accelerationTime;
    [SerializeField] private float decelerationTime;
    [SerializeField] private float topAngularSpeed;
    
    [Header("Jump")]
    [SerializeField] private float jumpAscendTime;
    [SerializeField] private float jumpFallTime;
    [SerializeField] private float jumpHeight;

    private float jumpTimer;
    private bool grounded = true;

    // driven values
    [HideInInspector] public bool intentionToJump;
    [HideInInspector] public Vector2 moveDirection;
    [HideInInspector] public float lookRadians;

    // driven methods
    public void ResetVerticalVelocity() {
        var velocity = rb.velocity;
        velocity.y = 0;
        rb.velocity = velocity;
    }

    private void Start() {
        rb = GetComponent<Rigidbody>();
        coll = GetComponentInChildren<Collider>(); 
        rb.maxAngularVelocity = 0;
    }

    private void Update() {
        var currentDegrees = Mathf.Atan2(transform.forward.x, transform.forward.z) * Mathf.Rad2Deg;
        var degrees = Mathf.MoveTowardsAngle(currentDegrees, lookRadians * Mathf.Rad2Deg, topAngularSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(0, degrees, 0);
    }

    private void FixedUpdate() {
        HorizontalMove();
        HandleJump();
    }

    private void HorizontalMove() {
        if (topSpeed == 0) return;
        var input = moveDirection * topSpeed;
        var relativeVelocity = rb.velocity;
        var deltavTarget = new Vector2(input.x - relativeVelocity.x, input.y - relativeVelocity.z);
        
        var deltavCap = topSpeed * Time.fixedDeltaTime / accelerationTime;
        var deltavCapDeceleration = topSpeed * Time.fixedDeltaTime / decelerationTime;
        var deltavRb = new Vector3(
            Mathf.MoveTowards(0, deltavTarget.x, input.x == 0 ? deltavCapDeceleration : deltavCap),
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

    private bool CheckGrounded() {
        const float window = 0.05f;
        return Physics.Raycast(new Ray(new Vector3(coll.bounds.center.x, coll.bounds.min.y + window * 0.5f, coll.bounds.center.z), Vector3.down), window, 1);
    }

    public void OnEncounterEvent(EncounterEventType eventType)
    {
        switch (eventType) {
            case EncounterEventType.Load:
                enabled = false;
                break;
            case EncounterEventType.Begin:
                enabled = true;
                break;
        }
    }

    public float GetJumpDuration() => jumpAscendTime + jumpFallTime;
}
