using UnityEngine;
using System.Collections;

public class FollowPlatforms : MonoBehaviour
{
    [SerializeField] private float raycastDistance = 0.1f;

    private RaycastHit hit;
    private CharacterController controller;
    private CharacterMovement characterMovement;

    void Awake() {
        controller = gameObject.GetComponent<CharacterController>();
        characterMovement = gameObject.GetComponent<CharacterMovement>();

        if (!controller) {
            Debug.LogWarning(transform.GetPath() + " does not have a CharacterController.");
            this.enabled = false;
        }
    }

    void Start() {

    }

    void Update() {
        Platform platform = GetPlatformUnder();
        if (platform) {
            Push(platform);
        }
    }

    // Called via SendMessage in CharacterMovement.
    void Jump() {
        if (!characterMovement) { return; }

        Platform platform = GetPlatformUnder();
        if (platform) {
            characterMovement.AddVelocity(platform.lastVelocity);
        }
    }

    Platform GetPlatformUnder() {
        Vector3 start = transform.position + Vector3.up; // Adds 1.0f up.
        float distance = 1.0f + raycastDistance; // Counteracts the 1.0f up.

        if (Physics.Raycast(start, Vector3.down, out hit, distance)) {
            return hit.collider.gameObject.GetComponent<Platform>();
        }

        return null;
    }

    void Push(Platform platform) {
        Vector3 movement = platform.lastVelocity * Time.deltaTime;
        // Apply an extra push vertically to prevent falling through platforms.
        movement.y = movement.y * 1.5f + 0.01f;
        controller.Move(movement);

        // Alternate push method.
        //Vector3 normal = collision.contacts[0].normal;
        //float platformSpeed = platform.lastVelocity.magnitude;
        //controller.Move(normal);
    }

    void OnCollisionEnter(Collision collision) {
        Platform platform = collision.gameObject.GetComponent<Platform>();
        if (platform) {
            Push(platform);
        }
    }

    void OnCollisionStay(Collision collision) {
        Platform platform = collision.gameObject.GetComponent<Platform>();
        if (platform) {
            Push(platform);
        }
    }
}
