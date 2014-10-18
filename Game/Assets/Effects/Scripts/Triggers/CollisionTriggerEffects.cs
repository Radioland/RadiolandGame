using UnityEngine;
using System.Collections;

public class CollisionTriggerEffects : TriggerEffects
{
    public string testColliderTag;
    public bool ignorePerpendicular = false;

    private Vector3 lastNonZeroVelocity;

    void Awake() {
        lastNonZeroVelocity = Vector3.zero;
    }

    void Start() {

    }

    void Update() {
        if (ignorePerpendicular) {
            if (rigidbody.velocity.magnitude > 0) {
                lastNonZeroVelocity = rigidbody.velocity;
            }
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (!ignorePerpendicular) {
            StartEventIfMatch(collision.collider.gameObject.tag);
        } else {
            float dotProduct = Vector3.Dot(lastNonZeroVelocity, collision.contacts[0].normal);
            if (Mathf.Abs(dotProduct) > 0) {
                StartEventIfMatch(collision.collider.gameObject.tag);
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        StartEventIfMatch(other.gameObject.tag);
    }

    void StartEventIfMatch(string colliderTag) {
        if (testColliderTag.Length == 0 ||
            colliderTag.Equals(testColliderTag)) {
            StartEvent();
        }
    }
}
