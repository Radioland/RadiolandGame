using UnityEngine;
using System.Collections;

// The player orbits around the camera, see CharacterMovement.StickToWorldspace.
// Reference tutorial video: https://www.youtube.com/watch?v=lnguV1v38z4
// Reference tutorial GitHub: https://github.com/jm991/UnityThirdPersonTutorial

public class CameraControl : MonoBehaviour
{
    public Transform cameraTransform;

    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform followTransform;
    [SerializeField] private CharacterMovement characterMovement;
    private Camera cameraComponent;

    // Distances from the player.
    [Header("Positioning and zooming")]
    [SerializeField] [Range(2.0f, 30.0f)] private float defaultRadius = 10.0f;
    [SerializeField] private float minRadius = 4f;
    [SerializeField] private float maxRadius = 14f;
    private float radius;
    private float manualTargetRadius;
    [SerializeField] private float maxTargetAhead = 3f;
    [SerializeField] private float defaultAngleUp = 25f;
    [SerializeField] private float minAngleUp = -50f;
    [SerializeField] private float maxAngleUp= 80.0f;
    private float angleUp;

    [Header("Auto zoom")]
    [SerializeField] private bool useAutoZoom = true;
    [SerializeField] private float minAutoZoomIn = 0.7f;
    [SerializeField] private float maxAutoZoomOut = 1.3f;
    private float autoTargetRadiusFactor;
    private float autoTargetRadiusFactorGoal;
    private const float autoZoomUpdateRate = 0.5f; // Seconds between updates.
    private float zoomCheckDistance = 8f;
    private int[] phiValues = { -20, 10, 30 };
    private int numRays = 8;

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
    [SerializeField] private float lookLerpDampTime = 0.6f;
    [SerializeField] private float aimAheadDampTime = 0.2f;
    [SerializeField] private float autoZoomDampTime = 0.5f;
    private Vector3 velocityLookDir = Vector3.zero;
    private Vector3 velocityCamSmooth = Vector3.zero;
    private Transform lookLerpTransform;
    private Vector3 lookLerpObjectSmooth = Vector3.zero;
    private float aimAheadSmooth = 0;
    private float autoZoomSmooth = 0;

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

        radius = defaultRadius;
        manualTargetRadius = defaultRadius;
        autoTargetRadiusFactor = 1f;
        autoTargetRadiusFactorGoal = 1f;
        angleUp = defaultAngleUp;
        lookDirXZ = followTransform.forward;
        curLookDirXZ = followTransform.forward;

        lastResetTime = -1000.0f;

        GameObject lookLerpObject = new GameObject("Camera Look Lerp Object");
        lookLerpTransform = lookLerpObject.transform;

        InvokeRepeating("AutoZoomObserve", 0f, autoZoomUpdateRate);
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
        // Get input values from controller/keyboard.
        // TODO: replace with better controller/mouse input management.
        float rightX = Input.GetAxis("RightStickX");
        float rightY = Input.GetAxis("RightStickY");

        ApplyMouseLook(ref rightX, ref rightY);

        bool movingOrRotating = (characterMovement.controlSpeed > deadZone ||
                                 Mathf.Abs(rightX) > deadZone || Mathf.Abs(rightY) > deadZone);

        UpdateLookDirectionY(rightX, rightY);
        if (movingOrRotating) { UpdateLookDirectionXZ(); }
        HandleReset(); // Fixes look rotations while in effect.

        UpdateRadius();

        SetTargetPosition();

        if (movingOrRotating) {
            SmoothMoveFollowTransform();
        }
        SmoothMoveCameraToTarget();
        SmoothLookAtTarget();

        CompensateForWalls(playerTransform.position, ref targetCameraPosition);
        UpdateAutoZoom();

        cameraTransform.LookAt(lookLerpTransform);
    }

    private void UpdateLookDirectionY(float rightX, float rightY) {
        // Free look using rightX and rightY.
        cameraTransform.RotateAround(followTransform.position, followTransform.up,
                                     rotateAroundSpeed * (Mathf.Abs(rightX) > deadZone ? rightX : 0f));
        angleUp += rotateUpSpeed * (Mathf.Abs(rightY) > deadZone ? rightY : 0f);
        angleUp = Mathf.Clamp(angleUp, minAngleUp, maxAngleUp);
    }

    private void UpdateLookDirectionXZ() {
        float leftX = Input.GetAxis("Horizontal");
        float leftY = Input.GetAxis("Vertical");

        lookDirXZ = Vector3.Lerp(followTransform.right * (leftX < 0 ? 1f : -1f),
                                 followTransform.forward * (leftY < 0 ? -1f : 1f),
                                 Mathf.Abs(Vector3.Dot(cameraTransform.forward, followTransform.forward)));

        // Calculate direction from camera to player, kill Y, and normalize to give a valid direction with unit magnitude.
        curLookDirXZ = Vector3.Normalize(followTransform.position - cameraTransform.position);
        curLookDirXZ.y = 0;

        // Damping makes it so we don't update targetCameraPosition while pivoting; camera shouldn't rotate around player.
        // Note: unlike in the tutorial, we don't use pivot. lookDirDampTime of 1 works well here.
        curLookDirXZ = Vector3.SmoothDamp(curLookDirXZ, lookDirXZ, ref velocityLookDir, 1);

        Debug.DrawRay(cameraTransform.position, lookDirXZ, Color.white);
        Debug.DrawRay(cameraTransform.position, curLookDirXZ, Color.red);
    }

    private void UpdateRadius() {
        if (angleUp < 0) {
            float angleUpNormalized = Mathf.InverseLerp(minAngleUp, 0, angleUp);
            manualTargetRadius = Mathf.Lerp(minRadius, defaultRadius, angleUpNormalized);
        } else {
            float angleUpNormalized = Mathf.InverseLerp(0, maxAngleUp, angleUp);
            manualTargetRadius = Mathf.Lerp(defaultRadius, maxRadius, angleUpNormalized);
        }
        radius = Mathf.Clamp(manualTargetRadius * autoTargetRadiusFactor, minRadius, maxRadius);
    }

    private void SetTargetPosition() {
        float xzAngle = Mathf.Atan2(curLookDirXZ.z, curLookDirXZ.x) + Mathf.PI;
        float angleUpRadians = angleUp * Mathf.Deg2Rad;
        Vector3 targetOffset = new Vector3(Mathf.Cos(xzAngle) * Mathf.Cos(angleUpRadians),
                                           Mathf.Sin(angleUpRadians),
                                           Mathf.Sin(xzAngle) * Mathf.Cos(angleUpRadians)) * radius;
        targetCameraPosition = followTransform.position + targetOffset;
        Debug.DrawLine(followTransform.position, followTransform.position + targetOffset, Color.white);
    }

    private void SmoothMoveFollowTransform() {
        // Aim ahead of followTransform based on current speed.
        Vector3 localFollowPosition = followTransform.localPosition;
        float targetAimAhead = 1f;
        targetAimAhead = Mathf.Lerp(0f, maxTargetAhead, Mathf.Pow(characterMovement.GetPercentWalkSpeed(), 2f));
        float aimAhead = Mathf.SmoothDamp(localFollowPosition.z, targetAimAhead, ref aimAheadSmooth, aimAheadDampTime);
        localFollowPosition.z = aimAhead;
        followTransform.localPosition = localFollowPosition;

        Debug.DrawLine(cameraTransform.position, followTransform.position, Color.cyan);
    }

    private void SmoothMoveCameraToTarget() {
        // Smoothly translate to the target position.
        cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, targetCameraPosition,
                                                  ref velocityCamSmooth, camSmoothDampTime, maxSpeed / Time.deltaTime);
    }

    private void SmoothLookAtTarget() {
        // Smoothly look to the target position (in xz - look directly at y).
        Vector3 lookPosition = Vector3.SmoothDamp(lookLerpTransform.position, followTransform.position,
                                                  ref lookLerpObjectSmooth, lookLerpDampTime);
        lookPosition.y = followTransform.position.y; // Look directly at y.
        lookLerpTransform.position = lookPosition;

        Debug.DrawLine(cameraTransform.position, lookLerpTransform.position, Color.blue);
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

    private void AutoZoomObserve() {
        Vector3 fromPosition = playerTransform.position;

        int numHits = 0;
        // Adjust zoom dynamically based on how densely packed the area around the player is.
        foreach (int phiValue in phiValues) {
            float phi = phiValue * Mathf.Deg2Rad;
            for (int i = 0; i < numRays; i++) {
                float theta = Mathf.PI * 2f / numRays * i;
                Vector3 direction = new Vector3(Mathf.Cos(theta) * Mathf.Cos(phi),
                                                Mathf.Sin(phi),
                                                Mathf.Sin(theta) * Mathf.Cos(phi)).normalized;

                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(fromPosition, direction, out hit, zoomCheckDistance, cameraBlockLayers)) {
                    numHits++;
                    Debug.DrawLine(fromPosition, hit.point, Color.red, autoZoomUpdateRate);
                } else {
                    Debug.DrawLine(fromPosition, fromPosition + direction * zoomCheckDistance, Color.green, autoZoomUpdateRate);
                }
            }
        }

        float target = numHits * 1f / (numRays * phiValues.Length);
        if (target < 0.5) {
            // Few hits, zoom out.
            target = Mathf.Lerp(1f, maxAutoZoomOut, (0.5f - target) / 0.5f);
        } else {
            // Many hits, zoom in.
            target = Mathf.Lerp(minAutoZoomIn, 1f, (target - 0.5f) / 0.5f);
        }
        autoTargetRadiusFactorGoal = target;
    }

    private void UpdateAutoZoom() {
        if (useAutoZoom) {
            autoTargetRadiusFactor = Mathf.SmoothDamp(autoTargetRadiusFactor, autoTargetRadiusFactorGoal,
                ref autoZoomSmooth, autoZoomDampTime);
        } else {
            autoTargetRadiusFactor = 1f;
        }
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
    }

    private void HandleReset() {
        // Reset look direction if the reset button is pressed.
        if (Input.GetButtonDown("ResetCamera")) { lastResetTime = Time.time; }
        if (Time.time - lastResetTime < resetDuration) {
            curLookDirXZ = followTransform.forward;
            angleUp = defaultAngleUp;
        }
    }
}
