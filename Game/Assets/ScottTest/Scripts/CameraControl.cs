using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    public Transform cameraTransform;

    // The camera moves around a sphere centered on the player.
    // The radius is affected by the current zoom level.
    // The two angles are controlled by mouse input.
    [Range(5.0f, 30.0f)] public float radius = 10.0f;
    public float defaultVerticalAngle = 20.0f;
    public float offsetHorizontal = -90.0f; // Set based on model's orientation.

    private float horizontalAngle; // Y-Axis Euler Angle
    private float verticalAngle; // X-Axis Euler Angle
    private float lastMouseX;
    private float lastMouseY;

    void Awake() {
        if (!cameraTransform) { cameraTransform = Camera.main.transform; }

        verticalAngle = defaultVerticalAngle;
        horizontalAngle = 0.0f;
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
            horizontalAngle = -transform.eulerAngles.y + offsetHorizontal;
            verticalAngle = defaultVerticalAngle;
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
        cameraTransform.position = transform.position + offset * radius;

        cameraTransform.LookAt(transform.position);

        lastMouseX = Input.mousePosition.x;
        lastMouseY = Input.mousePosition.y;
    }
}
