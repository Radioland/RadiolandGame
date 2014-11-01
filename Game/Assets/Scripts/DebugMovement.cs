using UnityEngine;
using System.Collections;

// Noclip flight movement for rapid testing and development.
public class DebugMovement : MonoBehaviour
{
    public bool isActive;
    [SerializeField] private float speed;
    [SerializeField] [Range(0.0f, 540.0f)] private float degRotPerSec = 200.0f;

    private CharacterMovement characterMovement;

    void Awake() {
        isActive = false;

        characterMovement = gameObject.GetComponent<CharacterMovement>();
    }

    void Start() {

    }

    void Update() {
        // Toggle enabled state.
        if (Input.GetKeyDown(KeyCode.J)) {
            isActive = !isActive;

            characterMovement.enabled = !isActive;
        }

        if (isActive) {
            // Rotate with horizontal input.
            float inputRotation = Input.GetAxis("Horizontal") * degRotPerSec * Time.deltaTime;
            Vector3 rotationEulerAngles = new Vector3(0.0f, inputRotation, 0.0f);
            transform.Rotate(rotationEulerAngles);

            float y = Input.GetAxis("Strafe") + (Input.GetButton("Jump") ? 1 : 0);
            float z = Input.GetAxis("Vertical");

            Vector3 inputDirection = transform.forward * z + transform.up * y;

            Vector3 position = transform.position;
            position += inputDirection * speed * Time.deltaTime;
            transform.position = position;
        }
    }
}
