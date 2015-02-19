using UnityEngine;
using System.Collections;

public enum BounceMode
{
    Static,
    Triggered
}

public class BouncePlatform : MonoBehaviour
{
    public BounceMode bounceMode = BounceMode.Static;
    public float staticElasticity = 0.5f;
    [SerializeField] private float triggeredSpeed = 30f;
    [SerializeField] private float triggeredDuration = 0.5f;
    [SerializeField] private float triggeredNewSmoothDampTimes = 10.0f;

    [SerializeField] private LayerMask bounceTriggerLayers;

    private float lastBounceTime;

    private void Awake() {
        lastBounceTime = -1000f;
    }

    private void Start() {

    }

    private void Update() {

    }

    public void TriggerBounce() {
        lastBounceTime = Time.time;
    }

    public void OnTriggerStay(Collider other) {
        if (bounceMode != BounceMode.Triggered ||
            Time.time - lastBounceTime > triggeredDuration) { return; }

        if (((1<<other.gameObject.layer) & bounceTriggerLayers) != 0) {
            CharacterMovement characterMovement = other.transform.root.GetComponent<CharacterMovement>();
            if (characterMovement) {
                characterMovement.Bounce(triggeredSpeed, transform.up,
                                         newSmoothDampTimes:triggeredNewSmoothDampTimes);
            }
        }
    }
}
