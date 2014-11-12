using UnityEngine;
using System.Collections;

public class SmoothMoveToTarget : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private string targetTag;
    [SerializeField] private Vector3 initialVelocity;
    [SerializeField] private float smoothX = 0.3f;
    [SerializeField] private float smoothY = 0.3f;
    [SerializeField] private float smoothZ = 0.3f;
    [SerializeField] private bool lookAtTarget = true;

    private Vector3 velocity;

    void Awake() {
        if (!target && targetTag.Length > 0) {
            target = GameObject.FindWithTag(targetTag).transform;
        }

        velocity = initialVelocity;
    }

    void Start() {

    }

    void Update() {
        if (!target) { return; }

        Vector3 difference = target.position - transform.position;
        Vector3 targetVelocity = difference;

        velocity.x = Mathf.Lerp(velocity.x, targetVelocity.x, smoothX);
        velocity.y = Mathf.Lerp(velocity.y, targetVelocity.y, smoothY);
        velocity.z = Mathf.Lerp(velocity.z, targetVelocity.z, smoothZ);

        transform.position = transform.position + velocity * Time.deltaTime;

        if (lookAtTarget) {
            transform.LookAt(target);
        }
    }

    public void SetTarget(Transform newTarget) {
        target = newTarget;
    }
}
