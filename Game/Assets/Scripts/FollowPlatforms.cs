using UnityEngine;
using System.Collections;

public class FollowPlatforms : MonoBehaviour
{
    [SerializeField] private float raycastDistance = 0.1f;

    private RaycastHit hit;
    private CharacterController controller;

    void Awake() {
        controller = gameObject.GetComponent<CharacterController>();

        if (!controller) {
            Debug.LogWarning(transform.GetPath() + " does not have a CharacterController.");
            this.enabled = false;
        }
    }

    void Start() {

    }

    void Update() {
        Vector3 start = transform.position + Vector3.up; // Adds 1.0f up.
        float distance = 1.0f + raycastDistance; // Counteracts the 1.0f up.

        if (Physics.Raycast(start, Vector3.down, out hit, distance)) {
            Platform platform = hit.collider.gameObject.GetComponent<Platform>();
            if (platform) {
                controller.Move(platform.deltaPosition);
            }
        }
    }

    void Push(Platform platform) {
        controller.Move(platform.lastVelocity * Time.deltaTime * 1.1f);

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
