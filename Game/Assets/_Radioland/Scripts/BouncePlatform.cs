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
    public QuadraticCurve trajectory;
    [SerializeField] private float triggeredSpeed = 30f;
    [SerializeField] private float triggeredNewSmoothDampTimes = 10.0f;

    [SerializeField] private LayerMask bounceTriggerLayers;

    private bool bouncing;

    private void Awake() {
        bouncing = false;
    }

    private void Start() {

    }

    private void Update() {

    }

    public void OnTriggerStay(Collider other) {
        if (bounceMode != BounceMode.Triggered || !bouncing) { return; }

        if (((1<<other.gameObject.layer) & bounceTriggerLayers) != 0) {
            CharacterMovement characterMovement = other.transform.root.GetComponent<CharacterMovement>();
            if (characterMovement) {
                if (trajectory) {
                    characterMovement.Bounce(trajectory);
                } else {
                    characterMovement.Bounce(triggeredSpeed, transform.up,
                                             newSmoothDampTimes:triggeredNewSmoothDampTimes);
                }
            }
        }
    }

    public void StartBouncing() {
        bouncing = true;
    }

    public void StopBouncing() {
        bouncing = false;
    }
}
