using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]

public class CharacterMovement : MonoBehaviour
{
    public CameraControl cameraControl;

    [SerializeField] [Range(0.0f, 20.0f)] private float walkSpeed = 5.0f;
    [SerializeField] private float directionSpeed = 1.5f; // Affects turn rate.
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
    public float mass = 1.0f;

    // Smoothing.
    [SerializeField] private float groundSmoothDampTime = 0.05f;
    [SerializeField] private float airSmoothDampTime = 0.7f;
    private Vector3 velocityDamp = Vector3.zero;
    private Vector3 velocity = Vector3.zero;

    // State (read-only visible to other scripts).
    private bool m_moving;
    private bool m_inJumpWindup;
    private bool m_jumping;
    private bool m_sliding;
    private bool m_falling;
    private float m_controlSpeed;
    private bool m_controllable;
    public bool moving { get { return m_moving; } }
    public bool inJumpWindup { get { return m_inJumpWindup; } }
    public bool jumping { get { return m_jumping; } }
    public bool sliding { get { return m_sliding; } }
    public bool grounded { get { return (collisionFlags & CollisionFlags.CollidedBelow) != 0; } }
    public bool falling { get { return m_falling; } }
    public float controlSpeed { get { return m_controlSpeed; } }
    public float speed { get { return (velocity + Vector3.up * verticalSpeed).magnitude; } }
    public bool controllable { get { return m_controllable; } }

    // Collision, jumping, sliding.
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

    // Input.
    private float leftX = 0f;
    private float leftY = 0f;
    private float direction = 0f;
    private float charAngle = 0f;

    // Animation.
    private Animator animator;
    private int speedHash;
    private int strafeHash;
    private int jumpingHash;
    private int landingHash;

    // Setting backups.
    private float originalGravity;
    private float originalJumpHeight;
    private float originalAirSmoothDampTime;
    private float originalMass;

    private float jumpVerticalSpeed {
        get { return Mathf.Sqrt(2 * jumpHeight * gravity); }
    }

    void Awake() {
        if (!cameraControl) {
            Debug.LogWarning("No camera control set on CharacterMovement!");
        }

        ResetState();

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
        originalAirSmoothDampTime = airSmoothDampTime;
        originalMass = mass;
    }

    void Start() {

    }

    void ResetState() {
        m_moving = false;
        m_inJumpWindup = false;
        m_jumping = false;
        m_sliding = false;
        m_falling = false;
        m_controlSpeed = 0.0f;
        m_controllable = true;
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        contactPoint = hit.point;
    }

    void Update() {
        if (grounded) {
            lastGroundedTime = Time.time;

            ApplySliding();
        }

        // Get input values from controller/keyboard.
        leftX = controllable ? Input.GetAxis("Horizontal") : 0;
        leftY = controllable ? Input.GetAxis("Vertical") : 0;
        float strafeInput = controllable ? Input.GetAxis("Strafe") : 0;

        bool inputReceived = false;
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) >= 0.01 ||
            Mathf.Abs(Input.GetAxisRaw("Vertical")) >= 0.01 ||
            Mathf.Abs(Input.GetAxisRaw("Strafe")) >= 0.01) {
            inputReceived = true;
            SendMessage("InputReceived", SendMessageOptions.DontRequireReceiver);
        } else {
            SendMessage("NoMovementInput", SendMessageOptions.DontRequireReceiver);
        }
        if (inputReceived && controllable) {
            m_moving = true;
        } else {
            m_moving = false;
        }

        charAngle = 0f;
        direction = 0f;
        float inputSpeed = 0f;

        // Translate controls stick coordinates into world/cam/character space.
        StickToWorldspace(transform, cameraControl.cameraTransform, ref direction,
                          ref inputSpeed, ref charAngle, false);
        m_controlSpeed = inputSpeed;

        // Don't rotate within a dead zone.
        if (m_controlSpeed > 0.05f) {
            transform.Rotate(new Vector3(0, charAngle, 0));
        }

        Vector3 motion = (transform.forward * m_controlSpeed + transform.right * strafeInput) * walkSpeed;
        motion = Vector3.ClampMagnitude(motion, walkSpeed);

        ApplyGravity();
        ApplyJump();

        if (grounded && !jumping) {
            velocity = Vector3.SmoothDamp(velocity, motion, ref velocityDamp, groundSmoothDampTime);
        } else {
            velocity = Vector3.SmoothDamp(velocity, motion, ref velocityDamp, airSmoothDampTime);
        }

        collisionFlags = controller.Move((velocity + Vector3.up * verticalSpeed) * Time.deltaTime);

        // Update animations.
        if (animator) {
            animator.SetFloat(speedHash, Mathf.Abs(m_controlSpeed));
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
                SendMessage("Grounded", landingVerticalSpeed, SendMessageOptions.DontRequireReceiver);
                m_falling = false;
            }
            m_jumping = false;
            animator.SetBool(landingHash, false);
        } else {
            if (Time.time - lastGroundedTime > fallingTime && verticalSpeed < 0.0f) {
                m_falling = true;
            }

            verticalSpeed -= gravity * Time.deltaTime;

            if (jumping) {
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
            TriggerJump();
        }

        // Do not allow jumping while sliding.
        if (sliding && slopeAngle > jumpSlopeLimit) { return; }

        // Do not allow jumping while not controllable.
        if (!controllable) { return; }

        // PreTimeout lets you trigger a jump slightly before landing.
        if (Time.time < lastJumpInputTime + jumpPreTimeout &&
            Time.time > lastJumpTime + jumpCooldown) {
            // PostTimeout lets you trigger a jump slightly after starting to fall.
            if (grounded || (Time.time < lastGroundedTime + jumpPostTimeout)) {
                lastJumpTime = Time.time;
                m_inJumpWindup = true;

                SendMessage("JumpStarted", SendMessageOptions.DontRequireReceiver);
            }
        }

        if (inJumpWindup && Time.time - lastJumpTime > jumpWindupTime) {
            m_inJumpWindup = false;
            m_jumping = true;
            verticalSpeed = jumpVerticalSpeed;

            SendMessage("Jump", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void TriggerJump() {
        lastJumpInputTime = Time.time;
        SendMessage("JumpTriggered", SendMessageOptions.DontRequireReceiver);
        SendMessage("InputReceived", SendMessageOptions.DontRequireReceiver);
    }

    public void Bounce(float bounceSpeed) {
        SendMessage("BounceTriggered", SendMessageOptions.DontRequireReceiver);
        verticalSpeed = bounceSpeed;
        m_jumping = true;
        lastJumpTime = Time.time;
    }

    // Velocity controls.
    public void AddVelocity(Vector3 extraVelocity) { velocity += extraVelocity; }
    public void SetVelocity(Vector3 newVelocity) { velocity = newVelocity; }

    // Set and reset properties.
    public void SetJumpHeight(float height) { jumpHeight = height; }
    public void ResetJumpHeight() { jumpHeight = originalJumpHeight; }
    public void SetGravity(float newGravity) { gravity = newGravity; }
    public void ResetGravity() { gravity = originalGravity; }
    public void SetAirSmoothDampTime(float newAirSmoothDampTime) { airSmoothDampTime = newAirSmoothDampTime; }
    public void ResetAirSmoothDampTime() { airSmoothDampTime = originalAirSmoothDampTime; }
    public void SetMass(float newMass) { mass = newMass; }
    public void ResetMass() { mass = originalMass; }
    public void SetControllable(bool isControllable) { m_controllable = isControllable; }

    private void StickToWorldspace(Transform root, Transform camera,
                                  ref float directionOut, ref float speedOut,
                                  ref float angleOut, bool isPivoting) {
        Vector3 rootDirection = root.forward;

        Vector3 stickDirection = new Vector3(leftX, 0, leftY);

        speedOut = stickDirection.sqrMagnitude;

        // Get camera rotation.
        Vector3 CameraDirection = camera.forward;
        CameraDirection.y = 0.0f; // kill Y
        Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward,
                                                                Vector3.Normalize(CameraDirection));

        // Convert joystick input in Worldspace coordinates.
        Vector3 moveDirection = referentialShift * stickDirection;
        Vector3 axisSign = Vector3.Cross(moveDirection, rootDirection);

        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z),
                      moveDirection, Color.green);
        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z),
                      rootDirection, Color.magenta);
        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z),
                      stickDirection, Color.blue);
        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2.5f, root.position.z),
                      axisSign, Color.red);

        float angleRootToMove = Vector3.Angle(rootDirection, moveDirection) * (axisSign.y >= 0 ? -1f : 1f);
        if (!isPivoting) {
            angleOut = angleRootToMove;
        }
        angleRootToMove /= 180f;

        directionOut = angleRootToMove * directionSpeed;
    }

}
