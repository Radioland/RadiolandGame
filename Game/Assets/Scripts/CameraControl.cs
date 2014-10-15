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
    [SerializeField]
    [Range(5.0f, 30.0f)] private float radius = 10.0f;
    [SerializeField]
    private float defaultVerticalAngle = 20.0f;
    [SerializeField]
    private float offsetHorizontal = -90.0f; // Set based on model's orientation.

    private float horizontalAngle; // Y-Axis Euler Angle
    private float verticalAngle; // X-Axis Euler Angle
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

        verticalAngle = defaultVerticalAngle;
        horizontalAngle = offsetHorizontal;
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
            if (characterMovement.playerMoving) {
                horizontalAngle = -targetTransform.eulerAngles.y + offsetHorizontal;
                verticalAngle = defaultVerticalAngle;
            }
        } else {
            // Mouse controlled camera rotation.
            float mouseDeltaX = Input.mousePosition.x - lastMouseX;
            float mouseDeltaY = Input.mousePosition.y - lastMouseY;
            horizontalAngle -= mouseDeltaX;
            verticalAngle -= mouseDeltaY;
        }

        float phi = horizontalAngle * Mathf.Deg2Rad;
        float theta = verticalAngle * Mathf.Deg2Rad;

        // Calculate the point on the unit sphere with the provided angles.
        float x = Mathf.Cos(theta) * Mathf.Cos(phi);
        float y = Mathf.Sin(theta);
        float z = Mathf.Cos(theta) * Mathf.Sin(phi);
        // Offset from the player by the vector to that point at the given radius.
        Vector3 offset = new Vector3(x, y, z);
        Vector3 targetPosition = targetTransform.position + offset * radius;
        cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position,
                                                      targetPosition,
                                                      ref velocityCamSmooth,
                                                      camSmoothDampTime);

        cameraTransform.LookAt(targetTransform.position);

        lastMouseX = Input.mousePosition.x;
        lastMouseY = Input.mousePosition.y;
    }
}
