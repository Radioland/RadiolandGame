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
    [Header("Positioning and zooming")]
    [SerializeField] [Range(2.0f, 30.0f)] private float defaultRadius = 10.0f;
    [SerializeField] private float minRadius = 4f;
    [SerializeField] private float maxRadius = 14f;
    private float radius;

    [SerializeField] private float defaultAngleUp = 25f;
    [SerializeField] private float minAngleUp = -50f;
    [SerializeField] private float maxAngleUp= 80.0f;
    private float angleUp;

    // Obstacle/occulsion avoidance.
    [Header("Obstacle avoidance")]
    [SerializeField] [Tooltip("Extra space between obstacle and camera")]
    private float compensationOffset = 0.2f;
    [SerializeField] private LayerMask cameraBlockLayers;
    private Vector3 nearClipDimensions = Vector3.zero; // width, height, radius
    private Vector3[] viewFrustum;

    // Camera speeds (controller and mouse free look as well as default orbit).
    [Header("Speeds")]
    [SerializeField] private float rotateAroundSpeed = 2f;
    [SerializeField] private float rotateUpSpeed = 2.5f;

    // Mouse look.
    [Header("Mouse look")]
    [SerializeField] private Vector2 mouseSensitivity = new Vector2(0.35f, 0.3f);
    [SerializeField] private Vector2 mouseSmoothing = new Vector2(2.0f, 2.0f);
    private Vector2 smoothMouse;

    private Vector3 targetCameraPosition;
    private Vector3 lookDirXZ;
    private Vector3 curLookDirXZ;
    private static float deadZone = 0.1f;

    // Smoothing and damping.
    [Header("Smoothing")]
    [SerializeField] private float maxSpeed = 1.5f;
    [SerializeField] private float camSmoothDampTime = 0.1f;
    [SerializeField] private float lookLerpDampTime = 0.2f;
    private Vector3 velocityLookDir = Vector3.zero;
    private Vector3 velocityCamSmooth = Vector3.zero;
    private Transform lookLerpTransform;
    private Vector3 lookLerpObjectSmooth = Vector3.zero;

    // Camera reset.
    private float lastResetTime;
    private float resetDuration = 0.3f;

    private void Awake() {
        if (!followTransform) { followTransform = transform; }
        if (!cameraTransform) { cameraTransform = Camera.main.transform; }
        if (!characterMovement) { Debug.LogWarning("No character movement set on CameraControl!"); }

        cameraComponent = cameraTransform.GetComponent<Camera>();
        targetCameraPosition = new Vector3(0, 100000, 0);
        smoothMouse = Vector2.zero;

        angleUp = defaultAngleUp;
        lookDirXZ = followTransform.forward;
        curLookDirXZ = followTransform.forward;

        lastResetTime = -1000.0f;

        GameObject lookLerpObject = new GameObject("Camera Look Lerp Object");
        lookLerpTransform = lookLerpObject.transform;
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
        cameraTransform.RotateAround(followTransform.position, followTransform.up,
                                     rotateAroundSpeed * (Mathf.Abs(rightX) > deadZone ? rightX : 0f));
        angleUp += rotateUpSpeed * (Mathf.Abs(rightY) > deadZone ? rightY : 0f);
        angleUp = Mathf.Clamp(angleUp, minAngleUp, maxAngleUp);

        if (angleUp < 0) {
            float angleUpNormalized = Mathf.InverseLerp(minAngleUp, 0, angleUp);
            radius = Mathf.Lerp(minRadius, defaultRadius, angleUpNormalized);
        } else {
            float angleUpNormalized = Mathf.InverseLerp(0, maxAngleUp, angleUp);
            radius = Mathf.Lerp(defaultRadius, maxRadius, angleUpNormalized);
        }

        // Only update camera look direction if moving or rotating.
        if (characterMovement.controlSpeed > deadZone ||
            Mathf.Abs(rightX) > deadZone || Mathf.Abs(rightY) > deadZone) {
            lookDirXZ = Vector3.Lerp(followTransform.right * (leftX < 0 ? 1f : -1f),
                                     followTransform.forward * (leftY < 0 ? -1f : 1f),
                                     Mathf.Abs(Vector3.Dot(cameraTransform.forward, followTransform.forward)));

            // Calculate direction from camera to player, kill Y, and normalize to give a valid direction with unit magnitude.
            curLookDirXZ = Vector3.Normalize(followTransform.position - cameraTransform.position);
            curLookDirXZ.y = 0;

            // Damping makes it so we don't update targetCameraPosition while pivoting; camera shouldn't rotate around player.
            // Note: unlike in the tutorial, we don't use pivot. lookDirDampTime of 1 works well here.
            curLookDirXZ = Vector3.SmoothDamp(curLookDirXZ, lookDirXZ, ref velocityLookDir, 1);
        }
        Debug.DrawRay(cameraTransform.position, lookDirXZ, Color.white);
        Debug.DrawRay(cameraTransform.position, curLookDirXZ, Color.red);

        // Reset look direction if the reset button is pressed.
        if (Input.GetButtonDown("ResetCamera")) { lastResetTime = Time.time; }
        if (Time.time - lastResetTime < resetDuration) {
            curLookDirXZ = followTransform.forward;
            angleUp = defaultAngleUp;
        }

        float xzAngle = Mathf.Atan2(curLookDirXZ.z, curLookDirXZ.x) + Mathf.PI;
        float angleUpRadians = angleUp * Mathf.Deg2Rad;
        Vector3 targetOffset = new Vector3(Mathf.Cos(xzAngle) * Mathf.Cos(angleUpRadians),
                                           Mathf.Sin(angleUpRadians),
                                           Mathf.Sin(xzAngle) * Mathf.Cos(angleUpRadians)) * radius;
        targetCameraPosition = followTransform.position + targetOffset;
        Debug.DrawLine(followTransform.position, followTransform.position + targetOffset, Color.white);

        CompensateForWalls(followTransform.position, ref targetCameraPosition);

        // Smoothly translate to the target position.
        targetCameraPosition = Vector3.SmoothDamp(cameraTransform.position, targetCameraPosition,
                                            ref velocityCamSmooth, camSmoothDampTime);
        Vector3 velocity = targetCameraPosition - cameraTransform.position;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        cameraTransform.position = cameraTransform.position + velocity;

        // Smoothly look to the target position (in xz - look directly at y).
        Vector3 lookPosition = Vector3.SmoothDamp(lookLerpTransform.position, followTransform.position,
                                                  ref lookLerpObjectSmooth, lookLerpDampTime);
        lookPosition.y = followTransform.position.y; // Look directly at y.
        lookLerpTransform.position = lookPosition;
        cameraTransform.LookAt(lookLerpTransform);

        Debug.DrawLine(cameraTransform.position, lookLerpTransform.position, Color.blue);
        Debug.DrawLine(cameraTransform.position, followTransform.position, Color.cyan);
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
                            defaultRadius, cameraBlockLayers)) {
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
