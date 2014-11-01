using UnityEngine;
using System.Collections;


public class CameraControl : MonoBehaviour
{
    private enum CameraState
    {
        PLAYER_FOLLOW, MOUSE_CONTROL
    }

    public Transform cameraTransform;

    [SerializeField] private Transform targetTransform;
    [SerializeField] private CharacterMovement characterMovement;
    private DebugMovement debugMovement;

    // The camera moves around a sphere centered on the player.
    // The horizontal and vertical angles are updated based on the current state.
    // Smoothing affects how floaty, tight, or jumpy the camera feels.

    [SerializeField] [Range(2.0f, 40.0f)] private float radius = 10.0f;
    [SerializeField] private float defaultVerticalAngle = 20.0f;
    [SerializeField] private float minVerticalAngle = 0.0f;
    [SerializeField] private float maxVerticalAngle = 65.0f;
    [Tooltip("Set based on model's orientation.")]
    [SerializeField] private float offsetHorizontal = -90.0f;
    [SerializeField] [Range(0.0f, 1.0f)] private float minTargetScreenY = 0.35f;
    [SerializeField] [Range(0.0f, 1.0f)] private float maxTargetScreenY = 0.65f;
    [SerializeField] [Tooltip("Slerp time (lower is more gradual, [0,1])")]
    private float lookHorizSpeed = 0.65f;
    [SerializeField] [Tooltip("Slerp time (lower is more gradual, [0,1])")]
    private float lookUpSpeed = 0.15f;
    [SerializeField] [Tooltip("Time to reach position (lower is faster)")]
    private float moveSmoothTime = 0.2f;
    [SerializeField] private float mouseLookSpeed = 0.5f;

    private Camera cameraComponent;
    private Vector3 targetPosition;
    private float targetHorizontalAngle; // Y-Axis Euler Angle
    private float targetVerticalAngle; // X-Axis Euler Angle
    private float lastMouseX;
    private float lastMouseY;
    private CameraState cameraState;

    private Vector3 velocityCamSmooth = Vector3.zero;
    private Quaternion verticalRotation;
    private Quaternion horizRotation;

    void Awake() {
        if (!targetTransform) { targetTransform = transform; }
        if (!cameraTransform) { cameraTransform = Camera.main.transform; }
        if (!characterMovement) {
            Debug.LogWarning("No character movement set on CameraControl!");
        }
        debugMovement = gameObject.GetComponent<DebugMovement>();

        cameraComponent = cameraTransform.GetComponent<Camera>();
        targetPosition = new Vector3(0, 100000, 0);
        targetVerticalAngle = defaultVerticalAngle;
        targetHorizontalAngle = offsetHorizontal;
        verticalRotation = Quaternion.identity;
        horizRotation = Quaternion.identity;
        lastMouseX = 0.0f;
        lastMouseY = 0.0f;
        cameraState = CameraState.PLAYER_FOLLOW;
    }

    void Start() {

    }

    void Update() {

    }

    void LateUpdate() {
        // Follow behind the player.
        if (!Input.GetMouseButton(1)) {
            cameraState = CameraState.PLAYER_FOLLOW;
            // Default behavior.
            // Rotate horizontalAngle towards the player's orientation.
            // Maintain a constant verticalAngle.
            if (characterMovement.moving || (debugMovement && debugMovement.isActive)) {
                targetHorizontalAngle = -targetTransform.eulerAngles.y + offsetHorizontal;
            }
        } else {
            cameraState = CameraState.MOUSE_CONTROL;
            // Mouse controlled camera rotation.
            float mouseDeltaX = Input.mousePosition.x - lastMouseX;
            float mouseDeltaY = Input.mousePosition.y - lastMouseY;
            targetHorizontalAngle -= mouseDeltaX * mouseLookSpeed;
            targetVerticalAngle -= mouseDeltaY * mouseLookSpeed;
        }

        targetVerticalAngle = Mathf.Clamp(targetVerticalAngle, minVerticalAngle, maxVerticalAngle);

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
            // Follow when falling or near the edge of the viewport.
            Vector3 targetViewportPoint = cameraComponent.WorldToViewportPoint(targetTransform.position);
            if (newTargetPosition.y < targetPosition.y ||
                targetViewportPoint.y > maxTargetScreenY ||
                targetViewportPoint.y < minTargetScreenY) {
                targetPosition.y = newTargetPosition.y;
            }
        } else {
            targetPosition.y = newTargetPosition.y;
        }

        cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position,
                                                      targetPosition,
                                                      ref velocityCamSmooth,
                                                      moveSmoothTime);

        if (cameraState == CameraState.PLAYER_FOLLOW) {
            // Update rotation, adjusting for the player leaving the viewport.
            // Alternately: cameraTransform.LookAt(targetTransform.position);

            // First rotate around the Y axis.
            Vector3 offsetToCenter = targetTransform.position - cameraTransform.position;
            Vector3 offsetXZ = new Vector3(offsetToCenter.x, 0, offsetToCenter.z);
            Quaternion targetHorizRotation = Quaternion.LookRotation(offsetXZ);
            horizRotation = Quaternion.Slerp(horizRotation, targetHorizRotation, lookHorizSpeed);
            cameraTransform.rotation = horizRotation;

            // Default to the target vertical angle (not at the player if they are jumping).
            Quaternion targetVerticalRotation = Quaternion.Euler(targetVerticalAngle, 0, 0);

            // If the player is near an edge of the viewport, aim at them instead.
            /*
            Vector3 from = cameraTransform.forward;
            Vector3 to = targetTransform.position - cameraTransform.position;
            float remainingAngle = targetVerticalAngle - Vector3.Angle(from, to);

            Vector3 targetViewportPoint = cameraComponent.WorldToViewportPoint(targetTransform.position);
            if (targetViewportPoint.y > maxTargetScreenY) {
                targetVerticalRotation *= Quaternion.Euler(remainingAngle, 0, 0);
            } else if (targetViewportPoint.y < minTargetScreenY) {
                targetVerticalRotation *= Quaternion.Euler(-remainingAngle, 0, 0);
            }
            */

            // Smoothly rotate to the target.
            verticalRotation = Quaternion.Slerp(verticalRotation, targetVerticalRotation, lookUpSpeed);
            cameraTransform.rotation *= verticalRotation;
        } else if (cameraState == CameraState.MOUSE_CONTROL) {
            cameraTransform.LookAt(targetTransform.position);
            // Update rotations so that there isn't a sharp jump when switching back to follow mode.
            verticalRotation = Quaternion.Euler(cameraTransform.eulerAngles.x, 0, 0);
            horizRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        }

        lastMouseX = Input.mousePosition.x;
        lastMouseY = Input.mousePosition.y;
    }
}
