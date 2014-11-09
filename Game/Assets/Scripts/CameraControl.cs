using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    public Transform cameraTransform;

    [SerializeField] private Transform followTransform;
    [SerializeField] private CharacterMovement characterMovement;
    private Camera cameraComponent;

    // Distances from the player.
    [SerializeField] [Range(2.0f, 40.0f)] private float targetRadius = 6.0f;
    [SerializeField] private float distanceUp = 2.0f;
    [SerializeField] private float minDistanceUp = 1.0f;
    [SerializeField] private float maxDistanceUp = 10.0f;

    // Obstacle/occulsion avoidance.
    [SerializeField] [Tooltip("Extra space between obstacle and camera")]
    private float compensationOffset = 0.2f;
    [SerializeField] private LayerMask cameraBlockLayers;
    private Vector3 nearClipDimensions = Vector3.zero; // width, height, radius
    private Vector3[] viewFrustum;

    // Camera speeds.
    [SerializeField] private float rotateSpeed = 3.0f;
    [SerializeField] private float zoomSpeed = 0.2f;
    [SerializeField] private float mouseRotateSpeed = 4.0f;
    [SerializeField] private float mouseZoomSpeed = 1.0f;

    private Vector3 targetPosition;
    private Vector3 characterOffset;
    private Vector3 lookDir;
    private Vector3 curLookDir;
    private float lastMouseX;
    private float lastMouseY;

    // Smoothing and damping.
    [SerializeField] private float camSmoothDampTime = 0.1f;
    [SerializeField] private float lookDirDampTime = 1.0f;
    private Vector3 velocityLookDir = Vector3.zero;
    private Vector3 velocityCamSmooth = Vector3.zero;

    void Awake() {
        if (!followTransform) { followTransform = transform; }
        if (!cameraTransform) { cameraTransform = Camera.main.transform; }
        if (!characterMovement) {
            Debug.LogWarning("No character movement set on CameraControl!");
        }

        cameraComponent = cameraTransform.GetComponent<Camera>();
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
        viewFrustum = DebugDraw.CalculateViewFrustum(cameraComponent, ref nearClipDimensions);

        cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation,
                                                        Quaternion.identity, Time.deltaTime);

        // Get input values from controller/keyboard.
        // TODO: replace with better controller/mouse input management.
        float leftX = Input.GetAxis("Horizontal");
        float leftY = Input.GetAxis("Vertical");
        float rightX = Input.GetAxis("RightStickX");
        float rightY = Input.GetAxis("RightStickY");

        // Compute percentage of the screen that the mouse travelled over.
        float mouseDeltaX = (Input.mousePosition.x - lastMouseX) / Screen.width;
        float mouseDeltaY = (Input.mousePosition.y - lastMouseY) / Screen.height;

        // Scale based on an arbitrary maximum.
        // TODO: add more intuitive control over mouse camera speeds.
        float scaledX = Mathf.InverseLerp(0.0f, 0.05f, Mathf.Abs(mouseDeltaX));
        float scaledY = Mathf.InverseLerp(0.0f, 0.05f, Mathf.Abs(mouseDeltaY));
        rightX += Mathf.Sign(mouseDeltaX) * scaledX * mouseRotateSpeed;
        rightY += Mathf.Sign(mouseDeltaY) * scaledY * mouseZoomSpeed;

        // Free look using rightX and rightY.
        cameraTransform.RotateAround(characterOffset, followTransform.up,
                                     rotateSpeed * (Mathf.Abs(rightX) > 0.1f ? rightX : 0f));
        distanceUp += zoomSpeed * (Mathf.Abs(rightY) > 0.1f ? rightY : 0f);
        distanceUp = Mathf.Clamp(distanceUp, minDistanceUp, maxDistanceUp);

        // Only update camera look direction if moving or rotating.
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

        targetPosition = characterOffset + followTransform.up * distanceUp -
                         Vector3.Normalize(curLookDir) * targetRadius;

        CompensateForWalls(characterOffset, ref targetPosition);

        // Smoothly translate to the target position.
        cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, targetPosition,
                                                      ref velocityCamSmooth, camSmoothDampTime);
        cameraTransform.LookAt(followTransform);

        lastMouseX = Input.mousePosition.x;
        lastMouseY = Input.mousePosition.y;
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
