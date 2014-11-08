using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    public Transform cameraTransform;

    [SerializeField] private Transform followTransform;
    [SerializeField] private CharacterMovement characterMovement;
    private DebugMovement debugMovement;

    // The camera moves around a sphere centered on the player.
    // The horizontal and vertical angles are updated based on the current state.
    // Smoothing affects how floaty, tight, or jumpy the camera feels.
    // When blocked, the camera zooms in.

    [SerializeField] [Range(2.0f, 40.0f)] private float defaultRadius = 6.0f;
    [SerializeField] [Range(2.0f, 40.0f)] private float minRadius = 2.0f;
    [SerializeField] [Tooltip("Extra space between obstacle and camera")]
    private float zoomBuffer = 0.2f;
    [SerializeField] private LayerMask cameraBlockLayers;
    [SerializeField] [Range(0.0f, 1.0f)] private float minTargetScreenY = 0.35f;
    [SerializeField] [Range(0.0f, 1.0f)] private float maxTargetScreenY = 0.65f;
    [SerializeField] private float mouseLookSpeed = 0.1f;

    private Camera cameraComponent;
    private float targetRadius;
    private Vector3 targetPosition;
    private float lastMouseX;
    private float lastMouseY;

    private Vector3 lookDir;
    private Vector3 curLookDir;
    private Vector3 characterOffset;

    [SerializeField]
    private float distanceUp = 2.0f;
    [SerializeField]
    private float minDistanceUp = 1.0f;
    [SerializeField]
    private float maxDistanceUp = 10.0f;

    // Smoothing and damping.
    [SerializeField]
    private float camSmoothDampTime = 0.1f;
    [SerializeField]
    private float lookDirDampTime = 0.1f;
    private Vector3 velocityLookDir = Vector3.zero;
    private Vector3 velocityCamSmooth = Vector3.zero;
    // =====================================================================

    void Awake() {
        if (!followTransform) { followTransform = transform; }
        if (!cameraTransform) { cameraTransform = Camera.main.transform; }
        if (!characterMovement) {
            Debug.LogWarning("No character movement set on CameraControl!");
        }
        debugMovement = gameObject.GetComponent<DebugMovement>();

        cameraComponent = cameraTransform.GetComponent<Camera>();
        targetRadius = defaultRadius;
        targetPosition = new Vector3(0, 100000, 0);
        lastMouseX = Input.mousePosition.x;
        lastMouseY = Input.mousePosition.y;

        lookDir = followTransform.forward;
        curLookDir = followTransform.forward;

        characterOffset = followTransform.position + new Vector3(0f, distanceUp, 0f);
    }

    void Start() {

    }

    void Update() {

    }

    void LateUpdate() {
        // Get input values from controller/keyboard.
        float leftX = Input.GetAxis("Horizontal");
        float leftY = Input.GetAxis("Vertical");
        float rightX = Input.GetAxis("RightStickX");
        float rightY = Input.GetAxis("RightStickY");

        // TODO: replace with better control/mouse switching.
        if (Mathf.Abs(rightX) < 0.1f) {
            rightX = (Input.mousePosition.x - lastMouseX) * mouseLookSpeed;
        }
        if (Mathf.Abs(rightY) < 0.1f) {
            rightY = (Input.mousePosition.y - lastMouseY) * mouseLookSpeed;
        }

        cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation,
                                                        Quaternion.identity, Time.deltaTime);

        cameraTransform.RotateAround(characterOffset, followTransform.up,
                                     2.0f * (Mathf.Abs(rightX) > 0.1f ? rightX : 0f));
        distanceUp += 0.1f * (Mathf.Abs(rightY) > 0.1f ? rightY : 0f);
        distanceUp = Mathf.Clamp(distanceUp, minDistanceUp, maxDistanceUp);
        cameraTransform.LookAt(followTransform);

        // Only update camera look direction if moving
        if (characterMovement.speed > 0.1f || Mathf.Abs(rightX) > 0.1f || Mathf.Abs(rightY) > 0.1f) {
            lookDir = Vector3.Lerp(followTransform.right * (leftX < 0 ? 1f : -1f),
                                   followTransform.forward * (leftY < 0 ? -1f : 1f),
                                   Mathf.Abs(Vector3.Dot(cameraTransform.forward, followTransform.forward)));
            Debug.DrawRay(cameraTransform.position, lookDir, Color.white);

            // Calculate direction from camera to player, kill Y, and normalize to give a valid direction with unit magnitude
            curLookDir = Vector3.Normalize(followTransform.position - cameraTransform.position);
            curLookDir.y = 0;
            Debug.DrawRay(cameraTransform.position, curLookDir, Color.green);

            // Damping makes it so we don't update targetPosition while pivoting; camera shouldn't rotate around player
            curLookDir = Vector3.SmoothDamp(curLookDir, lookDir, ref velocityLookDir, lookDirDampTime);
        }

        characterOffset = followTransform.position + (distanceUp * followTransform.up);

        //targetPosition = characterOffset +
        Vector3 newTargetPosition = characterOffset +
                                    followTransform.up * distanceUp -
                                    Vector3.Normalize(curLookDir) * targetRadius;
        targetPosition.x = newTargetPosition.x;
        targetPosition.z = newTargetPosition.z;

        /*
        // TODO: jump camera rotation and movement.
        if (characterMovement.jumping) {
            // Follow when falling or near the edge of the viewport.
            Vector3 targetViewportPoint = cameraComponent.WorldToViewportPoint(followTransform.position);
            if (newTargetPosition.y < targetPosition.y ||
                targetViewportPoint.y > maxTargetScreenY ||
                targetViewportPoint.y < minTargetScreenY) {
                targetPosition.y = newTargetPosition.y;
            }
        } else {
            targetPosition.y = newTargetPosition.y;
        }
        */
        targetPosition.y = newTargetPosition.y;

        // Smoothly translate to the target position.
        cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, targetPosition,
                                                      ref velocityCamSmooth, camSmoothDampTime);
        cameraTransform.LookAt(followTransform);

        // Zoom in if blocked.
        // TODO: test unblock code from tutorial.
        Debug.DrawLine(cameraTransform.position, followTransform.position);
        Vector3 direction = cameraTransform.position - followTransform.position;
        RaycastHit hit;
        if (Physics.Raycast(followTransform.position, direction, out hit,
                            defaultRadius, cameraBlockLayers)) {
            targetRadius = Mathf.Max(minRadius, hit.distance - zoomBuffer);
            Debug.DrawLine(followTransform.position, hit.point, Color.red);
        } else {
            targetRadius = defaultRadius;
        }


        lastMouseX = Input.mousePosition.x;
        lastMouseY = Input.mousePosition.y;
    }
}
