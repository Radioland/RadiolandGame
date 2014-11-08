using UnityEngine;
using System.Collections;

#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.

public class CameraControl : MonoBehaviour
{
    private enum CameraState
    {
        PLAYER_FOLLOW, MOUSE_CONTROL
    }

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
    private float targetRadius;
    private Vector3 targetPosition;
    private float targetHorizontalAngle; // Y-Axis Euler Angle
    private float targetVerticalAngle; // X-Axis Euler Angle
    private float lastMouseX;
    private float lastMouseY;
    private CameraState cameraState;

    private Vector3 velocityCamSmooth = Vector3.zero;
    private Quaternion verticalRotation;
    private Quaternion horizRotation;
    
    // =====================================================================
    // Private vars from third person tutorial.
    private Vector3 lookDir;
    private Vector3 curLookDir;
    private Vector3 characterOffset;

    [SerializeField]
    private float distanceUp = 2.0f;
    [SerializeField]
    private float distanceAway = 6.0f;
    
    // Smoothing and damping.
    [SerializeField]
    private float camSmoothDampTime = 0.1f;
    private Vector3 velocityLookDir = Vector3.zero;
    [SerializeField]
    private float lookDirDampTime = 0.1f;
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
        targetVerticalAngle = defaultVerticalAngle;
        targetHorizontalAngle = offsetHorizontal;
        verticalRotation = Quaternion.identity;
        horizRotation = Quaternion.identity;
        lastMouseX = 0.0f;
        lastMouseY = 0.0f;
        cameraState = CameraState.PLAYER_FOLLOW;

        
        lookDir = followTransform.forward;
        curLookDir = followTransform.forward;

        characterOffset = followTransform.position + new Vector3(0f, distanceUp, 0f);
    }

    void Start() {

    }

    void Update() {

    }

    void LateUpdate() {
        
        // Pull values from controller/keyboard.
        float rightX = Input.GetAxis("RightStickX");
        float rightY = Input.GetAxis("RightStickY");
        float leftX = Input.GetAxis("Horizontal");
        float leftY = Input.GetAxis("Vertical");

        cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation,
                                                        Quaternion.identity, Time.deltaTime);
        
        // Only update camera look direction if moving
        //if (characterMovement.speed > 0.1f && follow.IsInLocomotion() && !follow.IsInPivot()) {
        if (characterMovement.speed > 0.1f) {
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
        
        targetPosition = characterOffset +
                         followTransform.up * distanceUp -
                         Vector3.Normalize(curLookDir) * targetRadius;
        Debug.DrawLine(followTransform.position, targetPosition, Color.magenta);

        // Smoothly translate to the target position.
        cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, targetPosition,
                                                      ref velocityCamSmooth, camSmoothDampTime);
        cameraTransform.LookAt(followTransform);
    }
    
    #if OLD
    void LateUpdate() {
        // Follow behind the player.
        /*
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
        */

        // Controller
        //targetHorizontalAngle = -targetTransform.eulerAngles.y + offsetHorizontal -
        //                        Input.GetAxis("RightHorizontal") * 45.0f;
        //targetVerticalAngle = defaultVerticalAngle - Input.GetAxis("RightVertical") * 45.0f;
        targetHorizontalAngle += Input.GetAxis("RightStickX") * 5.0f;
        targetVerticalAngle -= Input.GetAxis("RightStickY") * 5.0f;

        targetVerticalAngle = Mathf.Clamp(targetVerticalAngle, minVerticalAngle, maxVerticalAngle);
        targetHorizontalAngle = -targetTransform.eulerAngles.y + offsetHorizontal;
        
        float phi = targetHorizontalAngle * Mathf.Deg2Rad;
        float theta = targetVerticalAngle * Mathf.Deg2Rad;

        // Update position.
        // Calculate the point on the unit sphere with the provided angles.
        float x = Mathf.Cos(theta) * Mathf.Cos(phi);
        float y = Mathf.Sin(theta);
        float z = Mathf.Cos(theta) * Mathf.Sin(phi);
        // Offset from the player by the vector to that point at the given radius.
        Vector3 offset = new Vector3(x, y, z);
        Vector3 newTargetPosition = targetTransform.position + offset * targetRadius;

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

        // Zoom in if blocked.
        Debug.DrawLine(cameraTransform.position, targetTransform.position);
        Vector3 direction = cameraTransform.position - targetTransform.position;
        RaycastHit hit;
        if (Physics.Raycast(targetTransform.position, direction, out hit,
                            defaultRadius, cameraBlockLayers)) {
            targetRadius = Mathf.Max(minRadius, hit.distance - zoomBuffer);
            Debug.DrawLine(targetTransform.position, hit.point, Color.red);
        } else {
            targetRadius = defaultRadius;
        }

        lastMouseX = Input.mousePosition.x;
        lastMouseY = Input.mousePosition.y;
    }
#endif
}
