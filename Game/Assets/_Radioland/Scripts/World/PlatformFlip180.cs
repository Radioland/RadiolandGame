using UnityEngine;
using System.Collections;

public class PlatformFlip180 : MonoBehaviour
{
    [SerializeField] private float flipDuration = 1f;
    [SerializeField] private bool flipNegative = true;

    private bool flipping;
    private bool flippingForward;
    private float lastFlippedTime;
    private Quaternion initialRotation;

    private void Awake() {
        flipping = false;
        flippingForward = true;
        lastFlippedTime = Time.time;
        initialRotation = transform.localRotation;
    }

    private void Start() {
        Messenger.AddListener("SingleJumpStarted", OnSingleJumpStarted);
    }

    private void Update() {
        if (flipping) {
            float t = (Time.time - lastFlippedTime) / flipDuration;
            float angle = flippingForward ? Mathf.Lerp(0f, 180f, t) : Mathf.Lerp(180f, 0f, t);
            if (flipNegative) { angle *= -1; }

            transform.localRotation = initialRotation * Quaternion.Euler(0f, 0f, angle);

            if (t > 1) {
                flipping = false;
                flippingForward = !flippingForward;
            }
        }
    }

    private void OnSingleJumpStarted() {
        if (!flipping) {
            StartFlipping();
        }
    }

    private void StartFlipping() {
        lastFlippedTime = Time.time;
        flipping = true;
    }
}
