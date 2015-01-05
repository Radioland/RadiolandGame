using UnityEngine;
using System.Collections;

public class CollisionTriggerEffects : TriggerEffects
{
    // Variables to specify in the editor.
    [Tooltip("Leave empty to trigger on any tag.")]
    [SerializeField] private string testColliderTag;

    protected override void Awake() {
        base.Awake();
    }

    private void Start() {

    }

    private void Update() {

    }

    private void OnCollisionEnter(Collision collision) {
        StartEventIfMatch(collision.collider.gameObject.tag);
    }

    private void OnTriggerEnter(Collider other) {
        StartEventIfMatch(other.gameObject.tag);
    }

    private void StartEventIfMatch(string colliderTag) {
        if (testColliderTag.Length == 0 ||
            colliderTag.Equals(testColliderTag)) {
            StartEvent();
        }
    }
}
