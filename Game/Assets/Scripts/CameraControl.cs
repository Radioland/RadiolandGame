using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    public Transform cameraTransform;

    [SerializeField] private Transform targetTransform;
    [SerializeField] private CharacterMovement characterMovement;

    // The camera moves around a sphere centered on the player.
    // The radius is affected by the current zoom level.
    // The two angles are controlled by mouse input.
    [SerializeField] [Range(2.0f, 40.0f)] private float radius = 10.0f;
    [SerializeField] private float defaultVerticalAngle = 20.0f;
    [Tooltip("Set based on model's orientation.")]
    [SerializeField] private float offsetHorizontal = -90.0f;
    [SerializeField] [Range(0.0f, 1.0f)] private float maxTargetScreenY = 0.75f;

    private Camera cameraComponent;
    private Vector3 targetPosition;
    private float targetHorizontalAngle; // Y-Axis Euler Angle
    private float targetVerticalAngle; // X-Axis Euler Angle
    private float lastMouseX;
    private float lastMouseY;

    private Vector3 velocityCamSmooth = Vector3.zero;
    [SerializeField]
    private float camSmoothDampTime = 0.1f;

    void Awake() {
        if (!targetTransform) { targetTransform = transform; }
        if (!cameraTransform) { cameraTransform = Camera.main.transform; }
        if (!characterMovement) {
            Debug.LogWarning("No character movement set on CameraControl!");
        }

        cameraComponent = cameraTransform.GetComponent<Camera>();
        targetPosition = new Vector3(0, 100000, 0);
        targetVerticalAngle = defaultVerticalAngle;
        targetHorizontalAngle = offsetHorizontal;
        lastMouseX = 0.0f;
        lastMouseY = 0.0f;
    }

    void Start() {

    }

    void Update() {

    }

    void LateUpdate() {
        // Follow behind the player.
        if (!Input.GetMouseButton(1)) {
            // Default behavior.
            // Rotate horizontalAngle towards the player's orientation.
            // Maintain a constant verticalAngle.
            if (characterMovement.moving) {
                targetHorizontalAngle = -targetTransform.eulerAngles.y + offsetHorizontal;
                targetVerticalAngle = defaultVerticalAngle;
            }
        } else {
            // Mouse controlled camera rotation.
            float mouseDeltaX = Input.mousePosition.x - lastMouseX;
            float mouseDeltaY = Input.mousePosition.y - lastMouseY;
            targetHorizontalAngle -= mouseDeltaX;
            targetVerticalAngle -= mouseDeltaY;
        }

        float phi = targetHorizontalAngle * Mathf.Deg2Rad;
        float theta = targetVerticalAngle * Mathf.Deg2Rad;

        // Update position.
        // Calculate the point on the unit sphere with the provided angles.
        float x = Mathf.Cos(theta) * Mathf.Cos(phi);
        float y = Mathf.Sin(theta);
        float z = Mathf.Cos(theta) * Mathf.Sin(phi);
        // Offset from the player by the vector to that point at the given radius.
        Vector3 offset = new Vector3(x, y, z);
        Vector3 newTargetPosition = targetTransform.position + offset * radius;

        targetPosition.x = newTargetPosition.x;
        targetPosition.z = newTargetPosition.z;
        if (characterMovement.jumping) {
            // Follow when falling.
            if (newTargetPosition.y < targetPosition.y) {
                targetPosition.y = newTargetPosition.y;
            }
        } else {
            targetPosition.y = newTargetPosition.y;
        }

        cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position,
                                                      targetPosition,
                                                      ref velocityCamSmooth,
                                                      camSmoothDampTime);

        // Update rotation.
        Vector3 offsetToCenter = targetTransform.position - cameraTransform.position;
        Vector3 offsetXZ = new Vector3(offsetToCenter.x, 0, offsetToCenter.z);
        Quaternion yRotation = Quaternion.LookRotation(offsetXZ);

        cameraTransform.rotation = yRotation;

        cameraTransform.rotation *= Quaternion.Euler(targetVerticalAngle, 0, 0);
        //cameraTransform.LookAt(targetTransform.position);

        lastMouseX = Input.mousePosition.x;
        lastMouseY = Input.mousePosition.y;
    }
}
