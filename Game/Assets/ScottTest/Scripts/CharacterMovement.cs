using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour
{
    [Range(0.0f, 20.0f)] public float walkSpeed = 5.0f;
    [Range(0.0f, 540.0f)] public float degRotPerSec = 200.0f;
    [Range(0.0f, 100.0f)] public float gravity = 30.0f;
    [Range(0.0f, 10.0f)] public float jumpHeight = 2.0f; // Speed is calculated to reach this height.
    [Range(0.0f, 0.5f)] public float jumpTimeout = 0.1f; // Extra time to become grounded before jumping.

    private CharacterController controller;
    private CollisionFlags collisionFlags;
    private float verticalSpeed;
    private float lastJumpTime;

    private bool grounded {
        get { return (collisionFlags & CollisionFlags.CollidedBelow) != 0; }
    }
    private float jumpVerticalSpeed {
        get { return Mathf.Sqrt(2 * jumpHeight * gravity); }
    }

    void Awake() {
        controller = gameObject.GetComponent<CharacterController>();
        verticalSpeed = 0.0f;
        lastJumpTime = -1000.0f;
    }

    void Start() {

    }

    void Update() {
        // Rotate with horizontal input.
        float inputRotation = Input.GetAxis("Horizontal") * degRotPerSec * Time.deltaTime;
        Vector3 rotationEulerAngles = new Vector3(0.0f, inputRotation, 0.0f);
        transform.Rotate(rotationEulerAngles);

        // Move forward with vertical input.
        float inputForward = Input.GetAxis("Vertical") * walkSpeed;

        ApplyGravity();
        ApplyJump();

        Vector3 motion = transform.forward * inputForward + Vector3.up * verticalSpeed;
        motion *= Time.deltaTime;
        collisionFlags = controller.Move(motion);
    }

    void ApplyGravity() {
        if (grounded) {
            verticalSpeed = 0.0f;
        } else {
            verticalSpeed -= gravity * Time.deltaTime;
        }
    }

    void ApplyJump() {
        if (Input.GetButtonDown("Jump")) {
            lastJumpTime = Time.time;
        }
        // Timeout lets you trigger a jump slightly before landing.
        if (grounded && Time.time < lastJumpTime + jumpTimeout) {
            verticalSpeed = jumpVerticalSpeed;
        }
    }
}
