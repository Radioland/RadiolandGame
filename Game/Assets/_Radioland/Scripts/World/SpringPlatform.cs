using UnityEngine;
using System.Collections;

public class SpringPlatform : MonoBehaviour
{
    [SerializeField] private float springStrength = 14.0f;
    [SerializeField] private float dampingStrength = 0.8f;
    [SerializeField] private float mass = 8.0f;

    private float restHeight;
    private float currentVelocity;

    private void Awake() {
        restHeight = transform.position.y;

        currentVelocity = 0.0f;
    }

    private void Start() {

    }

    private void Update() {
        // Spring:       F = -kX
        // Acceleration: F =  ma
        // Damping:      F = -cv

        float distance = transform.position.y - restHeight;
        if (Mathf.Abs(distance) > 0) {
            // Accelerate based on distance.
            float acceleration = - springStrength * Mathf.Pow(distance, 2) * Mathf.Sign(distance);
            currentVelocity += acceleration * Time.deltaTime;

            // Damp based on velocity.
            acceleration = - dampingStrength * currentVelocity;
            currentVelocity += acceleration * Time.deltaTime;
        }

        transform.position = transform.position + Vector3.up * currentVelocity * Time.deltaTime;
    }

    public void ApplyForce(float force) {
        currentVelocity += force / mass;
    }

    public static SpringPlatform GetSpringPlatformOnObject(GameObject theObject) {
        SpringPlatform platform = theObject.GetComponent<SpringPlatform>();
        if (platform) { return platform; }

        if (theObject.transform.parent) {
            platform = theObject.transform.parent.GetComponent<SpringPlatform>();
            if (platform) { return platform; }
        }

        return null;
    }
}
