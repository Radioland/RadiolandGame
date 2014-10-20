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
    [SerializeField] private float jumpCooldown = 0.25f;

    // State (visible to other scripts).
    [SerializeField] private bool m_moving;
    public bool moving { get { return m_moving; } }
    [SerializeField] private bool m_jumping;
    public bool jumping { get { return m_jumping; } }

    private CharacterController controller;
    private CollisionFlags collisionFlags;
    private float verticalSpeed;
    private float lastJumpInputTime;
    private float lastJumpTime;
    private float lastGroundedTime;

    // Animation.
    private Animator animator;
    private int speedHash;
    private int strafeHash;

    // Setting backups.
    private float originalGravity;
    private float originalJumpHeight;

	//Moving platform
	private bool isOnMovingPlatform = false;
	GameObject platform;
	RaycastHit hit;

    private bool grounded {
        get { return (collisionFlags & CollisionFlags.CollidedBelow) != 0; }
    }
    private float jumpVerticalSpeed {
        get { return Mathf.Sqrt(2 * jumpHeight * gravity); }
    }

    void Awake() {
        if (!cameraControl) {
            Debug.LogWarning("No camera control set on CharacterMovement!");
        }

        // Setup initial state.
        m_moving = false;
        m_jumping = false;

        controller = gameObject.GetComponent<CharacterController>();
        verticalSpeed = 0.0f;
        lastJumpInputTime = -1000.0f;
        lastGroundedTime = -1000.0f;

        // Fetch animator properties.
        animator = gameObject.GetComponentInChildren<Animator>();
        if (!animator) {
            Debug.LogWarning("No animator found on " + transform.GetPath());
        }
        speedHash = Animator.StringToHash("Speed");
        strafeHash = Animator.StringToHash("Strafe");

        originalGravity = gravity;
        originalJumpHeight = jumpHeight;

		platform = GameObject.Find ("Platform");
    }

    void Start() {
		
    }

    void Update() {
		isOnMovingPlatform = false;
		if (Physics.Raycast (transform.position, Vector3.down, out hit,0.1f)) {
			if (hit.transform.tag == "moving") {
				isOnMovingPlatform = true;
				platform = hit.transform.gameObject;
			}
			else {
				isOnMovingPlatform = false;
			}
		}

		if (isOnMovingPlatform) {
			transform.position += new Vector3(platform.GetComponent<PlatformMoving>().xVelocity * Time.deltaTime,platform.GetComponent<PlatformMoving>().yVelocity * Time.deltaTime,platform.GetComponent<PlatformMoving>().zVelocity * Time.deltaTime);
		}

        if (grounded) { lastGroundedTime = Time.time; }

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
                float inputRotation = Input.GetAxis("Horizontal") *
                                      degRotPerSec * Time.deltaTime;
                Vector3 rotationEulerAngles = new Vector3(0.0f, inputRotation, 0.0f);
                transform.Rotate(rotationEulerAngles);
            } else {
                // Rotate to match the camera.
                float cameraRotation = cameraControl.cameraTransform.eulerAngles.y;
                Vector3 newEulerAngles = transform.eulerAngles;
                newEulerAngles.y = cameraRotation;
                transform.eulerAngles = newEulerAngles;
            }
        }
        m_moving = inputReceived;

        // Grab movement input values and update animations.
        Vector3 inputVector = transform.forward * Input.GetAxis("Vertical") +
                              transform.right * Input.GetAxis("Strafe");
        if (animator) {
            animator.SetFloat(speedHash, Mathf.Abs(Input.GetAxis("Vertical")));
            animator.SetFloat(strafeHash, Input.GetAxis("Strafe"));
        }

        ApplyGravity();
        ApplyJump();

        Vector3 motion = inputVector * walkSpeed + Vector3.up * verticalSpeed;
        motion *= Time.deltaTime;
        collisionFlags = controller.Move(motion);
    }

    void ApplyGravity() {
        if (grounded) {
            verticalSpeed = 0.0f;
            m_jumping = false;
        } else {
            verticalSpeed -= gravity * Time.deltaTime;
        }
    }

    void ApplyJump() {
        if (Input.GetButtonDown("Jump")) {
            lastJumpInputTime = Time.time;
        }

        // PreTimeout lets you trigger a jump slightly before landing.
        if (Time.time < lastJumpInputTime + jumpPreTimeout &&
            Time.time > lastJumpTime + jumpCooldown) {
            // PostTimeout lets you trigger a jump slightly after starting to fall.
            if (grounded || (Time.time < lastGroundedTime + jumpPostTimeout)) {
                verticalSpeed = jumpVerticalSpeed;
                lastJumpTime = Time.time;
                m_jumping = true;
            }
        }
    }

    public void SetJumpHeight(float height) { jumpHeight = height; }
    public void ResetJumpHeight() { jumpHeight = originalJumpHeight; }
    public void SetGravity(float newGravity) { gravity = newGravity; }
    public void ResetGravity() { gravity = originalGravity; }
}
