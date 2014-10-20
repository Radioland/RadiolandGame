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
    [SerializeField] [Range(0.0f, 1.0f)] private float minTargetScreenY = 0.35f;
    [SerializeField] [Range(0.0f, 1.0f)] private float maxTargetScreenY = 0.65f;
    [SerializeField] [Range(0.0f, 1.0f)] private float lookUpSmooth = 0.15f;

    private Camera cameraComponent;
    private Vector3 targetPosition;
    private float targetHorizontalAngle; // Y-Axis Euler Angle
    private float targetVerticalAngle; // X-Axis Euler Angle
    private float lastMouseX;
    private float lastMouseY;

    private Vector3 velocityCamSmooth = Vector3.zero;
    [SerializeField] private float camSmoothDampTime = 0.1f;
    private Quaternion verticalRotation;

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
        verticalRotation = Quaternion.identity;
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

        // Update rotation, adjusting for the player leaving the viewport.
        // Alternately: cameraTransform.LookAt(targetTransform.position);

        // First rotate around the Y axis.
        Vector3 offsetToCenter = targetTransform.position - cameraTransform.position;
        Vector3 offsetXZ = new Vector3(offsetToCenter.x, 0, offsetToCenter.z);
        Quaternion yRotation = Quaternion.LookRotation(offsetXZ);
        cameraTransform.rotation = yRotation;

        // Default to the target vertical angle (not at the player if they are jumping).
        Quaternion targetVerticalRotation = Quaternion.Euler(targetVerticalAngle, 0, 0);

        // If the player is near an edge of the viewport, aim at them instead.
        Vector3 from = cameraTransform.forward;
        Vector3 to = targetTransform.position - cameraTransform.position;
        float remainingAngle = targetVerticalAngle - Vector3.Angle(from, to);

        Vector3 targetViewportPoint = cameraComponent.WorldToViewportPoint(targetTransform.position);
        if (targetViewportPoint.y > maxTargetScreenY) {
            targetVerticalRotation *= Quaternion.Euler(remainingAngle, 0, 0);
        } else if (targetViewportPoint.y < minTargetScreenY) {
            targetVerticalRotation *= Quaternion.Euler(-remainingAngle, 0, 0);
        }

        // Smoothly rotate to the target.
        verticalRotation = Quaternion.Slerp(verticalRotation, targetVerticalRotation, lookUpSmooth);
        cameraTransform.rotation *= verticalRotation;

        lastMouseX = Input.mousePosition.x;
        lastMouseY = Input.mousePosition.y;
    }
}
