using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    public CameraControl cameraControl;

    #region Movement characteristic values to specify in the editor.
    [SerializeField] [Range(0f, 20f)] private float maxWalkSpeed = 8f;
    [SerializeField] private AnimationCurve runSpeedCurve;
    [SerializeField] [Range(0f, 100f)] private float gravity = 30f;
    [SerializeField] [Range(0f, 10f)] private float jumpHeight = 2.0f;
    [SerializeField] [Tooltip("Extra time to become grounded before jumping.")]
    private float jumpPreTimeout = 0.1f;
    [SerializeField] [Tooltip("Extra time to jump after starting to fall.")]
    private float jumpPostTimeout = 0.3f;
    [SerializeField] private float jumpCooldown = 0.5f;
    [SerializeField] private float jumpWindupTime = 0.1f;
    [SerializeField] [Tooltip("Look-ahead time to start landing animation.")]
    private float landingTime = 0.2f;
    [SerializeField] [Tooltip("Start sliding when the slope exceeds this (degrees).")]
    private float slideSlopeLimit = 60f;
    [SerializeField] [Tooltip("Prevent jumping when slope exceeds this (degrees).")]
    private float jumpSlopeLimit = 75f;
    [SerializeField] private float slideSpeed = 5f;
    [SerializeField] [Tooltip("Time required to start being classified as falling.")]
    private float fallingTime = 0.2f;
    public float mass = 1f;
    #endregion Movement characteristic values to specify in the editor.

    #region Smoothing controls.
    [SerializeField] private float groundSmoothDampTime = 0.1f;
    [SerializeField] private float airSmoothDampTime = 0.7f;
    private Vector3 velocityDamp = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    #endregion Smoothing controls.

    #region State (read-only visible to other scripts).
    public bool moving { get; private set; }
    public bool running { get; private set; }
    public bool inJumpWindup { get; private set; }
    public bool jumping { get; private set; }
    public bool bouncing { get; private set; }
    public bool sliding { get; private set; }
    public bool grounded { get { return (collisionFlags & CollisionFlags.CollidedBelow) != 0; } }
    public bool falling { get; private set; }
    public float controlSpeed { get; private set; }
    public float speed { get { return (velocity + Vector3.up * verticalSpeed).magnitude; } }
    public bool controllable { get; private set; }
    #endregion State (read-only visible to other scripts).

    #region Collision, jumping, sliding, bouncing, and input.
    private CharacterController controller;
    private CollisionFlags collisionFlags;
    private RaycastHit hit;
    private float verticalSpeed;
    private const float walkRunCutoff = 0.3f;
    private float lastRunInputStartTime;
    private float lastJumpInputTime;
    private float lastJumpTime;
    private float lastGroundedTime;
    private float lastBouncedTime;
    private const float minimumBounceTime = 0.3f;
    private const float bounceCooldown = 0.5f;
    private QuadraticCurve bounceTrajectory;
    private GameObject bounceTrajectoryParent;
    private GameObject bounceTrajectoryObject;
    private const float slopeRayDistance = 0.1f;
    private Vector3 contactPoint;
    private float slopeAngle;
    private float jumpVerticalSpeed { get { return Mathf.Sqrt(2 * jumpHeight * gravity); } }
    private float leftX;
    private float leftY;
    #endregion Collision, jumping, sliding, bouncing, and input.

    #region Animation
    private Animator animator;
    private int speedHash;
    private int strafeHash;
    private int jumpingHash;
    private int landingHash;
    #endregion Animation

    #region Setting backups.
    private float originalGravity;
    private float originalJumpHeight;
    private float originalGroundSmoothDampTime;
    private float originalAirSmoothDampTime;
    private float originalMass;
    #endregion Setting backups.

    private void Awake() {
        if (!cameraControl) { Debug.LogWarning("No camera control set on CharacterMovement!"); }

        controller = gameObject.GetComponent<CharacterController>();
        verticalSpeed = 0f;
        lastRunInputStartTime = -1000f;
        lastJumpInputTime = -1000f;
        lastJumpTime = -1000f;
        lastGroundedTime = -1000f;
        slopeAngle = 0f;
        leftX = 0f;
        leftY = 0f;

        // Fetch animator properties.
        animator = gameObject.GetComponentInChildren<Animator>();
        if (!animator) {
            Debug.LogWarning("No animator found on " + transform.GetPath());
        }
        speedHash = Animator.StringToHash("Speed");
        strafeHash = Animator.StringToHash("Strafe");
        jumpingHash = Animator.StringToHash("Jumping");
        landingHash = Animator.StringToHash("Landing");

        bounceTrajectoryParent = new GameObject("Bounce Trajectory Parent");

        originalGravity = gravity;
        originalJumpHeight = jumpHeight;
        originalGroundSmoothDampTime = groundSmoothDampTime;
        originalAirSmoothDampTime = airSmoothDampTime;
        originalMass = mass;

        ResetState();
    }

    private void Start() {

    }

    public void ResetState() {
        moving = false;
        running = false;
        inJumpWindup = false;
        jumping = false;
        bouncing = false;
        bounceTrajectory = null;
        sliding = false;
        falling = false;
        controlSpeed = 0f;
        controllable = true;

        ResetAirSmoothDampTime();
        ResetGroundSmoothDampTime();
        ResetGravity();
        ResetJumpHeight();
        ResetMass();
    }

    private void OnControllerColliderHit(ControllerColliderHit controllerHit) {
        contactPoint = controllerHit.point;
    }

    private void Update() {
        if (grounded) {
            lastGroundedTime = Time.time;

            ApplySliding();
        }

        if (bouncing) {
            ApplyBouncing();
        }

        // Get input values from controller/keyboard.
        leftX = controllable ? Input.GetAxis("Horizontal") : 0;
        leftY = controllable ? Input.GetAxis("Vertical") : 0;
        float strafeInput = controllable ? Input.GetAxis("Strafe") : 0;

        // Translate controls stick coordinates into world/cam/character space.
        float charAngle, inputSpeed;
        StickToWorldspace(transform, cameraControl.cameraTransform,
                          out inputSpeed, out charAngle, false);
        controlSpeed = inputSpeed;

        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) >= 0.01 ||
            Mathf.Abs(Input.GetAxisRaw("Vertical")) >= 0.01 ||
            Mathf.Abs(Input.GetAxisRaw("Strafe")) >= 0.01) {

            if (controllable && !running && controlSpeed < walkRunCutoff) {
                lastRunInputStartTime = Time.time;
            }
            running = controllable && controlSpeed > walkRunCutoff;
            moving = controllable;

            SendMessage("InputReceived", SendMessageOptions.DontRequireReceiver);
        } else {
            moving = false;
            running = false;
            SendMessage("NoMovementInput", SendMessageOptions.DontRequireReceiver);
        }

        // Don't rotate within a dead zone.
        if (controlSpeed > 0.05f) { transform.Rotate(new Vector3(0, charAngle, 0)); }

        float walkSpeed = maxWalkSpeed * runSpeedCurve.Evaluate(Time.time - lastRunInputStartTime);
        Vector3 motion = (transform.forward * controlSpeed + transform.right * strafeInput) * walkSpeed;
        motion = Vector3.ClampMagnitude(motion, maxWalkSpeed);

        ApplyGravity();
        ApplyJump();

        if (grounded && !jumping) {
            velocity = Vector3.SmoothDamp(velocity, motion, ref velocityDamp, groundSmoothDampTime);
        } else {
            velocity = Vector3.SmoothDamp(velocity, motion, ref velocityDamp, airSmoothDampTime);
        }

        if (!(bouncing && bounceTrajectory)) {
            collisionFlags = controller.Move((velocity + Vector3.up * verticalSpeed) * Time.deltaTime);
        }

        // Update animations.
        if (animator) {
            animator.SetFloat(speedHash, Mathf.Abs(controlSpeed));
            animator.SetFloat(strafeHash, strafeInput);
            animator.SetBool(jumpingHash, jumping);
        }
    }

    private void ApplyBouncing() {
        if (grounded && Time.time - lastBouncedTime > minimumBounceTime) {
            ResetAirSmoothDampTime();
            ResetGroundSmoothDampTime();
            bouncing = false;
            jumping = false;
            bounceTrajectory = null;
            Land();
        } else if (bounceTrajectory) {
            Vector3 target = bounceTrajectory.GetPoint(Time.time - lastBouncedTime);
            Vector3 difference = target - transform.position;
            Vector3 movement = difference * 0.5f;
            collisionFlags = controller.Move(movement);

            verticalSpeed = movement.y;
        }
    }

    private void ApplySliding() {
        sliding = false;

        slopeAngle = 0.0f;
        // First simply check under the character for a slope.
        if (Physics.Raycast(transform.position, Vector3.down, out hit, slopeRayDistance)) {
            slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
        } else {
            // Also check from a contactPoint (for particularly steep slopes).
            Physics.Raycast(contactPoint, Vector3.down, out hit);
            slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
        }

        if (slopeAngle > slideSlopeLimit) {
            sliding = true;

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

    private void ApplyGravity() {
        if (grounded) {
            if (falling) {
                Land();
            }
            jumping = false;
            animator.SetBool(landingHash, false);
        } else {
            if (Time.time - lastGroundedTime > fallingTime && verticalSpeed < 0.0f) {
                falling = true;
            }

            if (bouncing && bounceTrajectory) {
                // Gravity is already accounted for.
            } else {
                verticalSpeed -= gravity * Time.deltaTime;
            }

            if (jumping && !bouncing || Time.time - lastBouncedTime < minimumBounceTime) {
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

    private void Land() {
        if (bouncing && Time.time - lastBouncedTime < minimumBounceTime) { return; }

        float landingVerticalSpeed = verticalSpeed;
        verticalSpeed = 0.0f;
        SendMessage("Grounded", landingVerticalSpeed, SendMessageOptions.DontRequireReceiver);
        falling = false;
    }

    private void ApplyJump() {
        // Do not allow jumping while not controllable.
        if (!controllable) { return; }

        if (Input.GetButtonDown("Jump")) { TriggerJump(); }

        // Do not allow jumping while sliding.
        if (sliding && slopeAngle > jumpSlopeLimit) { return; }

        // PreTimeout lets you trigger a jump slightly before landing.
        if (Time.time < lastJumpInputTime + jumpPreTimeout &&
            Time.time > lastJumpTime + jumpCooldown) {
            // PostTimeout lets you trigger a jump slightly after starting to fall.
            if (grounded || (Time.time < lastGroundedTime + jumpPostTimeout)) {
                lastJumpTime = Time.time;
                inJumpWindup = true;

                SendMessage("JumpStarted", SendMessageOptions.DontRequireReceiver);
            }
        }

        if (inJumpWindup && Time.time - lastJumpTime > jumpWindupTime) {
            inJumpWindup = false;
            jumping = true;
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
        Bounce(bounceSpeed, Vector3.up);
    }

    public void Bounce(float bounceSpeed, Vector3 bounceDirection, float newSmoothDampTimes=0f) {
        if (Time.time - lastBouncedTime < bounceCooldown) { return; }
        lastBouncedTime = Time.time;
        Land();

        SendMessage("BounceTriggered", SendMessageOptions.DontRequireReceiver);
        Vector3 bounceVelocity = bounceSpeed * bounceDirection;
        verticalSpeed = Vector3.Dot(bounceVelocity, Vector3.up);
        SetVelocity(new Vector3(Vector3.Dot(bounceVelocity, Vector3.right),
                                0,
                                Vector3.Dot(bounceVelocity, Vector3.forward)));
        jumping = true;
        bouncing = true;
        lastJumpTime = Time.time;
        if (newSmoothDampTimes > 0) {
            SetAirSmoothDampTime(newSmoothDampTimes);
            SetGroundSmoothDampTime(newSmoothDampTimes);
        }
    }

    public void Bounce(QuadraticCurve trajectory) {
        if (Time.time - lastBouncedTime < bounceCooldown) { return; }
        lastBouncedTime = Time.time;
        Land();

        if (bounceTrajectoryObject) { Destroy(bounceTrajectoryObject); }
        bounceTrajectoryObject = new GameObject("Bounce Trajectory");
        bounceTrajectoryObject.transform.parent = bounceTrajectoryParent.transform;
        QuadraticCurve trajectoryCopy = bounceTrajectoryObject.AddComponent<QuadraticCurve>();
        trajectoryCopy.SetAsCopy(trajectory);

        bounceTrajectory = trajectoryCopy;

        SendMessage("BounceTriggered", SendMessageOptions.DontRequireReceiver);
        jumping = true;
        bouncing = true;
        lastJumpTime = Time.time;
    }

    // Velocity controls.
    public void AddVelocity(Vector3 extraVelocity) { velocity += extraVelocity; }
    public void SetVelocity(Vector3 newVelocity) { velocity = newVelocity; }
    public Vector3 GetVelocity() { return velocity; }
    public void Stop() {
        velocity = Vector3.zero;
        verticalSpeed = 0;
    }

    #region Set and reset properties.
    public void SetJumpHeight(float height) { jumpHeight = height; }
    public void ResetJumpHeight() { jumpHeight = originalJumpHeight; }
    public void SetGravity(float newGravity) { gravity = newGravity; }
    public void ResetGravity() { gravity = originalGravity; }
    public void SetGroundSmoothDampTime(float newGroundSmoothDampTime) { groundSmoothDampTime = newGroundSmoothDampTime; }
    public void ResetGroundSmoothDampTime() { groundSmoothDampTime = originalGroundSmoothDampTime; }
    public void SetAirSmoothDampTime(float newAirSmoothDampTime) { airSmoothDampTime = newAirSmoothDampTime; }
    public void ResetAirSmoothDampTime() { airSmoothDampTime = originalAirSmoothDampTime; }
    public void SetMass(float newMass) { mass = newMass; }
    public void ResetMass() { mass = originalMass; }
    public void SetControllable(bool isControllable) { controllable = isControllable; }
    #endregion Set and reset properties.

    private void StickToWorldspace(Transform root, Transform camera,
                                   out float speedOut, out float angleOut, bool isPivoting) {
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
        angleOut = isPivoting ? 0f: angleRootToMove;
    }
}
