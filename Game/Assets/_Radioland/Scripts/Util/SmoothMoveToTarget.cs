using UnityEngine;
using System.Collections;

public class SmoothMoveToTarget : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private string targetTag;
    [SerializeField] private Vector3 initialVelocity;
    [SerializeField] private float minSpeedScale = 1.0f;
    [SerializeField] private float maxSpeedScale = 1.0f;
    [SerializeField] private float speedRampTime = 10.0f;
    [SerializeField] private float minXZSpeed = 0.0f;
    [SerializeField] private float smoothX = 0.3f;
    [SerializeField] private float smoothY = 0.3f;
    [SerializeField] private float smoothZ = 0.3f;
    [SerializeField] private bool lookAtTarget = true;

    private Vector3 velocity;
    private float speedScale;
    private float timeStarted;

    private void Awake() {
        if (!target && targetTag.Length > 0) {
            target = GameObject.FindWithTag(targetTag).transform;
        }

        velocity = initialVelocity;
        timeStarted = Time.time;
    }

    private void Start() {

    }

    private void Update() {
        if (!target) { return; }

        float t = (Time.time - timeStarted) / speedRampTime;
        speedScale = Mathf.Lerp(minSpeedScale, maxSpeedScale, t);

        Vector3 difference = target.position - transform.position;
        Vector3 targetVelocity = difference * speedScale;

        velocity.x = Mathf.Lerp(velocity.x, targetVelocity.x, smoothX);
        velocity.y = Mathf.Lerp(velocity.y, targetVelocity.y, smoothY);
        velocity.z = Mathf.Lerp(velocity.z, targetVelocity.z, smoothZ);

        Vector2 xzVelocity = new Vector2(velocity.x, velocity.z);
        if (xzVelocity.magnitude < minXZSpeed) {
            xzVelocity = xzVelocity.normalized * minXZSpeed;
        }
        velocity.x = xzVelocity.x;
        velocity.z = xzVelocity.y;

        transform.position = transform.position + velocity * Time.deltaTime;

        if (lookAtTarget) {
            transform.LookAt(target);
        }
    }

    public void SetTarget(Transform newTarget) {
        target = newTarget;
    }
}
