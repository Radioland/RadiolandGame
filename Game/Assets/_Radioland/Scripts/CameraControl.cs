using UnityEngine;
using System.Collections;

// The player orbits around the camera, see CharacterMovement.StickToWorldspace.
// Reference tutorial video: https://www.youtube.com/watch?v=lnguV1v38z4
// Reference tutorial GitHub: https://github.com/jm991/UnityThirdPersonTutorial

public class CameraControl : MonoBehaviour
{
    public Transform cameraTransform;

    [SerializeField] private Transform followTransform;
    [SerializeField] private CharacterMovement characterMovement;
    private Camera cameraComponent;

    // Distances from the player.
    [Header("Positioning")]
    [SerializeField] [Range(2.0f, 40.0f)] private float targetRadius = 6.0f;
    [SerializeField] private float defaultDistanceUp = 2.0f;
    [SerializeField] private float minDistanceUp = 1.0f;
    [SerializeField] private float maxDistanceUp = 10.0f;
    private float distanceUp;

    // Obstacle/occulsion avoidance.
    [Header("Obstacle avoidance")]
    [SerializeField] [Tooltip("Extra space between obstacle and camera")]
    private float compensationOffset = 0.2f;
    [SerializeField] private LayerMask cameraBlockLayers;
    private Vector3 nearClipDimensions = Vector3.zero; // width, height, radius
    private Vector3[] viewFrustum;

    // Camera speeds (controller and mouse free look as well as default orbit).
    [Header("Speeds")]
    [SerializeField] private float rotateSpeed = 3.0f;
    [SerializeField] private float zoomSpeed = 0.2f;

    // Mouse look.
    [Header("Mouse look")]
    [SerializeField] private Vector2 mouseSensitivity = new Vector2(0.7f, 0.4f);
    [SerializeField] private Vector2 mouseSmoothing = new Vector2(2.0f, 2.0f);
    private Vector2 smoothMouse;

    private Vector3 targetPosition;
    private Vector3 characterOffset;
    private Vector3 lookDir;
    private Vector3 curLookDir;

    // Smoothing and damping.
    [Header("Smoothing")]
    [SerializeField] private float maxSpeed = 1.5f;
    [SerializeField] private float camSmoothDampTime = 0.1f;
    [SerializeField] private float lookLerpDampTime = 0.2f;
    private Vector3 velocityLookDir = Vector3.zero;
    private Vector3 velocityCamSmooth = Vector3.zero;
    private GameObject lookLerpObject;
    private Vector3 lookLerpObjectSmooth = Vector3.zero;

    // Camera reset.
    private float lastResetTime;
    private float resetDuration = 0.3f;

    private void Awake() {
        if (!followTransform) { followTransform = transform; }
        if (!cameraTransform) { cameraTransform = Camera.main.transform; }
        if (!characterMovement) {
            Debug.LogWarning("No character movement set on CameraControl!");
        }

        cameraComponent = cameraTransform.GetComponent<Camera>();
        targetPosition = new Vector3(0, 100000, 0);
        smoothMouse = Vector2.zero;

        distanceUp = defaultDistanceUp;
        lookDir = followTransform.forward;
        curLookDir = followTransform.forward;

        characterOffset = followTransform.position + new Vector3(0f, distanceUp, 0f);

        lastResetTime = -1000.0f;

        lookLerpObject = new GameObject("Camera Look Lerp Object");
    }

    private void Start() {

    }

    private void Update() {
        #if !UNITY_EDITOR
        Cursor.lockState = UnityEngine.CursorLockMode.Locked;
        Cursor.visible = false;
        #endif
    }

    private void LateUpdate() {
        viewFrustum = DebugDraw.CalculateViewFrustum(cameraComponent, ref nearClipDimensions);

        cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation,
                                                        Quaternion.identity, Time.deltaTime);

        // Get input values from controller/keyboard.
        // TODO: replace with better controller/mouse input management.
        float leftX = Input.GetAxis("Horizontal");
        float leftY = Input.GetAxis("Vertical");
        float rightX = Input.GetAxis("RightStickX");
        float rightY = Input.GetAxis("RightStickY");

        ApplyMouseLook(ref rightX, ref rightY);

        // Free look using rightX and rightY.
        cameraTransform.RotateAround(characterOffset, followTransform.up,
                                     rotateSpeed * (Mathf.Abs(rightX) > 0.1f ? rightX : 0f));
        distanceUp += zoomSpeed * (Mathf.Abs(rightY) > 0.1f ? rightY : 0f);
        distanceUp = Mathf.Clamp(distanceUp, minDistanceUp, maxDistanceUp);

        // Only update camera look direction if moving or rotating.
        if (characterMovement.controlSpeed > 0.1f || Mathf.Abs(rightX) > 0.1f || Mathf.Abs(rightY) > 0.1f) {
            lookDir = Vector3.Lerp(followTransform.right * (leftX < 0 ? 1f : -1f),
                                   followTransform.forward * (leftY < 0 ? -1f : 1f),
                                   Mathf.Abs(Vector3.Dot(cameraTransform.forward, followTransform.forward)));
            Debug.DrawRay(cameraTransform.position, lookDir, Color.white);

            // Calculate direction from camera to player, kill Y, and normalize to give a valid direction with unit magnitude.
            curLookDir = Vector3.Normalize(followTransform.position - cameraTransform.position);
            curLookDir.y = 0;
            Debug.DrawRay(cameraTransform.position, curLookDir, Color.green);

            // Damping makes it so we don't update targetPosition while pivoting; camera shouldn't rotate around player.
            // Note: unlike in the tutorial, we don't use pivot. lookDirDampTime of 1 works well here.
            curLookDir = Vector3.SmoothDamp(curLookDir, lookDir, ref velocityLookDir, 1);
        }

        // Reset look direction if the reset button is pressed.
        if (Input.GetButtonDown("ResetCamera")) { lastResetTime = Time.time; }
        if (Time.time - lastResetTime < resetDuration) {
            curLookDir = followTransform.forward;
            distanceUp = defaultDistanceUp;
        }

        characterOffset = followTransform.position + (distanceUp * followTransform.up);
        targetPosition = characterOffset + followTransform.up * distanceUp -
                         Vector3.Normalize(curLookDir) * targetRadius;

        CompensateForWalls(characterOffset, ref targetPosition);

        // Smoothly translate to the target position.
        targetPosition = Vector3.SmoothDamp(cameraTransform.position, targetPosition,
                                            ref velocityCamSmooth, camSmoothDampTime);
        Vector3 velocity = targetPosition - cameraTransform.position;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        cameraTransform.position = cameraTransform.position + velocity;

        // Smoothly look to the target position.
        lookLerpObject.transform.position = Vector3.SmoothDamp(lookLerpObject.transform.position,
                                                               followTransform.position,
                                                               ref lookLerpObjectSmooth,
                                                               lookLerpDampTime);
        Vector3 position = lookLerpObject.transform.position;
        position.y = followTransform.position.y;
        lookLerpObject.transform.position = position;
        cameraTransform.LookAt(lookLerpObject.transform);
    }

    private void ApplyMouseLook(ref float rightX, ref float rightY) {
        // http://forum.unity3d.com/threads/a-free-simple-smooth-mouselook.73117/
        // Get raw mouse input for a cleaner reading on more sensitive mice.
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        // Scale input against the sensitivity setting and multiply that against the smoothing value.
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(mouseSensitivity.x * mouseSmoothing.x,
                                                           mouseSensitivity.y * mouseSmoothing.y));

        // Interpolate mouse movement over time to apply smoothing delta.
        smoothMouse.x = Mathf.Lerp(smoothMouse.x, mouseDelta.x, 1.0f / mouseSmoothing.x);
        smoothMouse.y = Mathf.Lerp(smoothMouse.y, mouseDelta.y, 1.0f / mouseSmoothing.y);

        rightX += smoothMouse.x;
        rightY -= smoothMouse.y;
    }

    private void CompensateForWalls(Vector3 fromObject, ref Vector3 toTarget) {
        // Compensate for walls between camera.
        RaycastHit wallHit = new RaycastHit();
        Vector3 direction = Vector3.Normalize(toTarget - fromObject);
        if (Physics.Raycast(fromObject, direction, out wallHit,
                            targetRadius, cameraBlockLayers)) {
            Debug.DrawRay(wallHit.point, wallHit.normal, Color.red);
            toTarget = wallHit.point;
        }

        // Compensate for geometry intersecting with near clip plane.
        Vector3 camPosCache = cameraTransform.position;
        cameraTransform.position = toTarget;
        viewFrustum = DebugDraw.CalculateViewFrustum(cameraComponent, ref nearClipDimensions);

        for (int i = 0; i < (viewFrustum.Length / 2); i++) {
            RaycastHit cWHit = new RaycastHit();
            RaycastHit cCWHit = new RaycastHit();

            // Cast lines in both directions around near clipping plane bounds
            while (Physics.Linecast(viewFrustum[i], viewFrustum[(i + 1) % (viewFrustum.Length / 2)], out cWHit) ||
                   Physics.Linecast(viewFrustum[(i + 1) % (viewFrustum.Length / 2)], viewFrustum[i], out cCWHit)) {
                Vector3 normal = wallHit.normal;
                if (wallHit.normal == Vector3.zero) {
                    // If there's no available wallHit, use normal of geometry intersected by LineCasts instead.
                    if (cWHit.normal == Vector3.zero) {
                        if (cCWHit.normal == Vector3.zero) {
                            Debug.LogError("No available geometry normal from near clip plane LineCasts.", this);
                        } else {
                            normal = cCWHit.normal;
                        }
                    } else {
                        normal = cWHit.normal;
                    }
                }

                toTarget += (compensationOffset * normal);
                cameraTransform.position += toTarget;

                // Recalculate positions of near clip plane.
                viewFrustum = DebugDraw.CalculateViewFrustum(cameraComponent, ref nearClipDimensions);
            }
        }

        cameraTransform.position = camPosCache;
        viewFrustum = DebugDraw.CalculateViewFrustum(cameraComponent, ref nearClipDimensions);
    }
}
