using UnityEngine;
using System.Collections;

public class SmoothMoveToTarget : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private string targetTag;
    [SerializeField] private Vector3 initialVelocity;
    [SerializeField] private float accelerationScale = 0.2f;
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
        Vector3 acceleration = difference * accelerationScale;
        velocity += acceleration * Time.deltaTime;

        transform.Translate(velocity * Time.deltaTime);

        if (lookAtTarget) {
            transform.LookAt(target);
        }
    }

    public void SetTarget(Transform newTarget) {
        target = newTarget;
    }
}
