using UnityEngine;

public class AIMovement : MonoBehaviour
{
    public float maxLinearAcceleration = 5f;
    public float maxLinearSpeed = 1f;

    [HideInInspector] public Vector3 linearVelocity;
    [HideInInspector] public Vector3 linearAcceleration;

    [HideInInspector] public float linearSpeed;
    [HideInInspector] public Vector3 heading;

    private void Start() {
        linearVelocity = Vector3.zero;
        linearAcceleration = Vector3.zero;
    }

    private void Update() {
        linearAcceleration = Vector3.ClampMagnitude(linearAcceleration, maxLinearAcceleration);
        linearVelocity += linearAcceleration * Time.deltaTime;

        linearVelocity = Vector3.ClampMagnitude(linearVelocity, maxLinearSpeed);
        transform.localPosition = transform.localPosition + linearVelocity * Time.deltaTime;

        linearSpeed = linearVelocity.magnitude;
        heading = linearVelocity.normalized;
    }
}
