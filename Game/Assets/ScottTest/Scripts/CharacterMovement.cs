using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour
{
    [Range(0.0f, 20.0f)] public float walkSpeed = 5.0f;
    [Range(0.0f, 720.0f)] public float degRotPerSec = 60.0f;
    public float gravity = 10.0f;
    public float jumpHeight = 1.0f;

    private CharacterController controller;
    private CollisionFlags collisionFlags;
    private float verticalSpeed;

    private bool grounded {
        get { return (collisionFlags & CollisionFlags.CollidedBelow) != 0; }
    }
    private float jumpVerticalSpeed {
        get { return Mathf.Sqrt(2 * jumpHeight * gravity); }
    }

    void Awake() {
        controller = gameObject.GetComponent<CharacterController>();
        verticalSpeed = 0.0f;
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

        // Apply gravity.
        if (grounded) {
            verticalSpeed = 0.0f;
        } else {
            verticalSpeed -= gravity * Time.deltaTime;
        }

        // Jump.
        if (Input.GetButtonDown("Jump") && grounded) {
            verticalSpeed = jumpVerticalSpeed;
        }
        
        Vector3 motion = transform.forward * inputForward + Vector3.up * verticalSpeed;
        motion *= Time.deltaTime;
        collisionFlags = controller.Move(motion);
    }
}
