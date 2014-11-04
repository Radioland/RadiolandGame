using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]

public class CharacterMovement : MonoBehaviour
{
    public CameraControl cameraControl;

    [SerializeField] [Range(0.0f, 20.0f)] private float walkSpeed = 5.0f;
    [SerializeField] [Range(0.0f, 540.0f)] private float degRotPerSec = 200.0f;
    [SerializeField] [Range(0.0f, 100.0f)] private float gravity = 30.0f;
    // Speed is calculated to reach this height.
    [SerializeField] [Range(0.0f, 10.0f)] private float jumpHeight = 2.0f;
    [SerializeField] [Tooltip("Extra time to become grounded before jumping.")]
    private float jumpPreTimeout = 0.1f;
    [SerializeField] [Tooltip("Extra time to jump after starting to fall.")]
    private float jumpPostTimeout = 0.2f;
    [SerializeField] private float jumpCooldown = 0.35f;
    [SerializeField] private float jumpWindupTime = 0.1f;
    [SerializeField] private float landingTime = 0.2f;
    [SerializeField] [Tooltip("Start sliding when the slope exceeds this (degrees).")]
    private float slideSlopeLimit = 60.0f;
    [SerializeField] [Tooltip("Prevent jumping when slope exceeds this (degrees).")]
    private float jumpSlopeLimit = 75.0f;
    [SerializeField] private float slideSpeed = 5.0f;
    [SerializeField] [Tooltip("Time required to start being classified as falling.")]
    private float fallingTime = 0.2f;

    // State (read-only visible to other scripts).
    private bool m_controllable;
    public bool controllable { get { return m_controllable; } }
    private bool m_moving;
    public bool moving { get { return m_moving; } }
    private bool m_inJumpWindup;
    public bool inJumpWindup { get { return m_inJumpWindup; } }
    private bool m_jumping;
    public bool jumping { get { return m_jumping; } }
    private bool m_sliding;
    public bool sliding { get { return m_sliding; } }
    public bool grounded {
        get { return (collisionFlags & CollisionFlags.CollidedBelow) != 0; }
    }
    private bool m_falling;
    public bool falling { get { return m_falling; } }

    private CharacterController controller;
    private CollisionFlags collisionFlags;
    private RaycastHit hit;
    private float verticalSpeed;
    private float lastJumpInputTime;
    private float lastJumpTime;
    private float lastGroundedTime;
    private float rayDistance;
    private Vector3 contactPoint;
    private float slopeAngle;

    // Animation.
    private Animator animator;
    private int speedHash;
    private int strafeHash;
    private int jumpingHash;
    private int landingHash;

    // Setting backups.
    private float originalGravity;
    private float originalJumpHeight;

    private float jumpVerticalSpeed {
        get { return Mathf.Sqrt(2 * jumpHeight * gravity); }
    }

    void Awake() {
        if (!cameraControl) {
            Debug.LogWarning("No camera control set on CharacterMovement!");
        }

        // Setup initial state.
        m_controllable = true;
        m_moving = false;
        m_inJumpWindup = false;
        m_jumping = false;
        m_sliding = false;

        controller = gameObject.GetComponent<CharacterController>();
        verticalSpeed = 0.0f;
        lastJumpInputTime = -1000.0f;
        lastJumpTime = -1000.0f;
        lastGroundedTime = -1000.0f;
        rayDistance = 0.1f;
        slopeAngle = 0.0f;

        // Fetch animator properties.
        animator = gameObject.GetComponentInChildren<Animator>();
        if (!animator) {
            Debug.LogWarning("No animator found on " + transform.GetPath());
        }
        speedHash = Animator.StringToHash("Speed");
        strafeHash = Animator.StringToHash("Strafe");
        jumpingHash = Animator.StringToHash("Jumping");
        landingHash = Animator.StringToHash("Landing");

        originalGravity = gravity;
        originalJumpHeight = jumpHeight;
    }

    void Start() {

    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        contactPoint = hit.point;
    }

    void Update() {
        if (grounded) {
            lastGroundedTime = Time.time;

            ApplySliding();
        }

        // Check for movement input.
        bool inputReceived = false;
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) >= 0.01 ||
            Mathf.Abs(Input.GetAxisRaw("Vertical")) >= 0.01 ||
            Mathf.Abs(Input.GetAxisRaw("Strafe")) >= 0.01) {
            inputReceived = true;
        }

        // Rotate the player (only when input is received).
        if (inputReceived) {
            if (m_moving) {
                // Rotate with horizontal input.
                /*
                float inputRotation = Input.GetAxis("Horizontal") *
                                      degRotPerSec * Time.deltaTime;
                Vector3 rotationEulerAngles = new Vector3(0.0f, inputRotation, 0.0f);
                transform.Rotate(rotationEulerAngles);
                */
            } else {
                // Rotate to match the camera.
                /*
                float cameraRotation = cameraControl.cameraTransform.eulerAngles.y;
                Vector3 newEulerAngles = transform.eulerAngles;
                newEulerAngles.y = cameraRotation;
                transform.eulerAngles = newEulerAngles;
                */
            }
        }
        m_moving = inputReceived;

        // Grab movement input values.
        float verticalInput = 0.0f;
        float strafeInput = 0.0f;
        Vector3 inputVector = Vector3.zero;
        if (m_controllable) {
            verticalInput = Input.GetAxis("Vertical");
            //strafeInput = Input.GetAxis("Strafe");
            strafeInput = Input.GetAxis("Horizontal");
            inputVector = transform.forward * verticalInput + transform.right * strafeInput;
        }

        ApplyGravity();
        ApplyJump();

        Vector3 motion = inputVector * walkSpeed + Vector3.up * verticalSpeed;
        motion *= Time.deltaTime;
        collisionFlags = controller.Move(motion);

        // Update animations.
        if (animator) {
            animator.SetFloat(speedHash, Mathf.Abs(verticalInput));
            animator.SetFloat(strafeHash, strafeInput);
            animator.SetBool(jumpingHash, jumping);
        }
    }

    void ApplySliding() {
        m_sliding = false;

        slopeAngle = 0.0f;
        // First simply check under the character for a slope.
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance)) {
            slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
        } else {
            // Also check from a contactPoint (for particularly steep slopes).
            Physics.Raycast(contactPoint, Vector3.down, out hit);
            slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
        }

        if (slopeAngle > slideSlopeLimit) {
            m_sliding = true;

            // Compute moveDirection to point down the slope.
            Vector3 hitNormal = hit.normal;
            Vector3 moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
            Vector3.OrthoNormalize(ref hitNormal, ref moveDirection);
            collisionFlags = controller.Move(moveDirection * slideSpeed * Time.deltaTime);

            Debug.DrawRay(contactPoint, Vector3.down, Color.blue);
            Debug.DrawRay(hit.point, hit.normal, Color.red);
            Debug.DrawRay(hit.point, moveDirection, Color.magenta);
        }
    }

    void ApplyGravity() {
        if (grounded) {
            if (falling) {
                float landingVerticalSpeed = verticalSpeed;
                verticalSpeed = 0.0f;
                SendMessage("Grounded", landingVerticalSpeed);
                m_falling = false;
            }
            m_jumping = false;
            animator.SetBool(landingHash, false);
        } else {
            if (Time.time - lastGroundedTime > fallingTime && verticalSpeed < 0.0f) {
                m_falling = true;
            }

            verticalSpeed -= gravity * Time.deltaTime;

            if (m_jumping) {
                // Check if predicted to be landing within landingTime.
                // d = v * t + 1/2 a * t^2
                float distance = -(verticalSpeed * landingTime +
                                   0.5f * -gravity * landingTime * landingTime);
                if (distance > 0) {
                    Debug.DrawLine(transform.position, transform.position + Vector3.down * distance);

                    if (Physics.Raycast(transform.position, Vector3.down, out hit, distance)) {
                        animator.SetBool(landingHash, true);
                    }
                }
            }
        }
    }

    void ApplyJump() {
        if (Input.GetButtonDown("Jump")) {
            lastJumpInputTime = Time.time;
        }

        // Do not allow jumping while sliding.
        if (sliding && slopeAngle > jumpSlopeLimit) { return; }

        // PreTimeout lets you trigger a jump slightly before landing.
        if (m_controllable &&
            Time.time < lastJumpInputTime + jumpPreTimeout &&
            Time.time > lastJumpTime + jumpCooldown) {
            // PostTimeout lets you trigger a jump slightly after starting to fall.
            if (grounded || (Time.time < lastGroundedTime + jumpPostTimeout)) {
                lastJumpTime = Time.time;
                m_inJumpWindup = true;
                m_controllable = false;

                SendMessage("StartJump");
            }
        }

        if (m_inJumpWindup && Time.time - lastJumpTime > jumpWindupTime) {
            m_inJumpWindup = false;
            m_controllable = true;
            m_jumping = true;
            verticalSpeed = jumpVerticalSpeed;
        }
    }

    public void Bounce(float bounceSpeed) {
        verticalSpeed = bounceSpeed;
        m_jumping = true;
        lastJumpTime = Time.time;
    }

    public void SetJumpHeight(float height) { jumpHeight = height; }
    public void ResetJumpHeight() { jumpHeight = originalJumpHeight; }
    public void SetGravity(float newGravity) { gravity = newGravity; }
    public void ResetGravity() { gravity = originalGravity; }
}
