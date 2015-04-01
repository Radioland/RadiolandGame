using UnityEngine;
using System.Collections;
using System.Linq;

public class FollowPlatforms : MonoBehaviour
{
    [SerializeField] private float raycastDistance = 0.1f;
    [SerializeField] private float lingerTime = 1.0f;
    [SerializeField] private LayerMask layerMask;

    private CharacterController controller;
    private CharacterMovement characterMovement;
    private Platform latestPlatform;
    private float lastPlatformUnderTime;

    private void Awake() {
        controller = gameObject.GetComponent<CharacterController>();
        characterMovement = gameObject.GetComponent<CharacterMovement>();

        if (!controller) {
            Debug.LogWarning(transform.GetPath() + " does not have a CharacterController.");
            this.enabled = false;
        }

        lastPlatformUnderTime = -1000f;

        Messenger.AddListener("Jump", OnJump);
        Messenger.AddListener<float>("Grounded", OnGrounded);
    }

    private void Start() {

    }

    private void Update() {
        Platform platform = GetPlatformUnder();
        if (platform) {
            latestPlatform = platform;
            lastPlatformUnderTime = Time.time;
        }

        if (Time.time - lastPlatformUnderTime > lingerTime) {
            latestPlatform = null;
        }

        if (latestPlatform) {
            Push(latestPlatform);
        }
    }

    private void OnJump() {
        if (!characterMovement) { return; }

        if (latestPlatform) {
            characterMovement.AddVelocity(latestPlatform.lastVelocity);
            latestPlatform = null;
        }
    }

    private void OnGrounded(float verticalSpeed) {
        SpringPlatform springPlatform = GetSpringPlatformUnder();
        if (springPlatform) {
            springPlatform.ApplyForce(verticalSpeed * characterMovement.mass);
        }
    }

    private Platform GetPlatformUnder() {
        Vector3 start = transform.position + Vector3.up; // Adds 1.0f up.
        float distance = 1.0f + raycastDistance; // Counteracts the 1.0f up.

        Debug.DrawLine(start, start + Vector3.down * distance, Color.red);

        RaycastHit[] hits = Physics.RaycastAll(start, Vector3.down, distance, layerMask).OrderBy(h=>h.distance).ToArray();
        return hits.Select(hit => GetPlatformOnObject(hit.collider.gameObject)).FirstOrDefault(platform => platform);
    }

    private SpringPlatform GetSpringPlatformUnder() {
        Vector3 start = transform.position + Vector3.up; // Adds 1.0f up.
        float distance = 1.0f + raycastDistance; // Counteracts the 1.0f up.

        RaycastHit[] hits = Physics.RaycastAll(start, Vector3.down, distance, layerMask).OrderBy(h=>h.distance).ToArray();
        return hits.Select(hit => GetSpringPlatformOnObject(hit.collider.gameObject)).FirstOrDefault(platform => platform);
    }

    private void Push(Platform platform) {
        if (Time.timeScale < 0.01f) { return; }

        Vector3 movement = platform.lastVelocity * Time.deltaTime;
        // Apply an extra push vertically to prevent falling through platforms.
        movement.y = movement.y * 1.5f + 0.02f;

        if (movement.magnitude > 0.001f) {
            controller.Move(movement);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        Platform platform = GetPlatformOnObject(collision.gameObject);
        if (platform) { Push(platform); }
    }

    private void OnCollisionStay(Collision collision) {
        Platform platform = GetPlatformOnObject(collision.gameObject);
        if (platform) { Push(platform); }
    }

    private Platform GetPlatformOnObject(GameObject theObject) {
        Platform platform = theObject.GetComponent<Platform>();
        if (platform) { return platform; }

        platform = theObject.transform.parent.GetComponent<Platform>();
        if (platform) { return platform; }

        return null;
    }

    private SpringPlatform GetSpringPlatformOnObject(GameObject theObject) {
        SpringPlatform platform = theObject.GetComponent<SpringPlatform>();
        if (platform) { return platform; }

        platform = theObject.transform.parent.GetComponent<SpringPlatform>();
        if (platform) { return platform; }

        return null;
    }
}
