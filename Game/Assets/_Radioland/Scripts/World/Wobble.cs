using UnityEngine;
using System.Collections;

public class Wobble : MonoBehaviour
{
    // Variables to specify in the editor.

    [SerializeField] [Tooltip("Euler angles (degrees).")] private Vector3 minRotationChange;
    [SerializeField] [Tooltip("Euler angles (degrees).")] private Vector3 maxRotationChange;
    [SerializeField] private float minWobbleTime = 4.0f;
    [SerializeField] private float maxWobbleTime = 4.0f;
    [SerializeField] private float minPauseTime = 0.0f;
    [SerializeField] private float maxPauseTime = 0.0f;
    [SerializeField] private float minCooldown = 0.0f;
    [SerializeField] private float maxCooldown = 0.0f;

    private float lastStartedTime;
    private Quaternion initialRotation;
    private Quaternion rotationChange;
    private Quaternion targetRotation;
    private float wobbleTime;
    private float pauseTime;
    private float cooldown;

    private void Reset() {
        minRotationChange = new Vector3(-10f, -10f, -10f);
        maxRotationChange = new Vector3(10f, 10f, 10f);
    }

    private void Awake() {
        StartWobble();
    }

    private void Start() {

    }

    private void StartWobble() {
        lastStartedTime = Time.time;

        initialRotation = transform.localRotation;
        rotationChange = Quaternion.Euler(Random.Range(minRotationChange.x, maxRotationChange.x),
                                          Random.Range(minRotationChange.y, maxRotationChange.y),
                                          Random.Range(minRotationChange.z, maxRotationChange.z));
        targetRotation = initialRotation * rotationChange;

        wobbleTime = Random.Range(minWobbleTime, maxWobbleTime);
        pauseTime = Random.Range(minPauseTime, maxPauseTime);
        cooldown = Random.Range(minCooldown, maxCooldown);
    }

    private void Update() {

        if (Time.time < lastStartedTime + wobbleTime) {
            // Wobble to targetRotation and then back.
            if (Time.time < lastStartedTime + wobbleTime / 2.0f) {
                // Wobble from initialRotation to targetRotation.
                float t = Mathf.InverseLerp(lastStartedTime,
                                            lastStartedTime + wobbleTime / 2.0f,
                                            Time.time);
                transform.localRotation = Quaternion.Lerp(initialRotation, targetRotation, t);
            } else if (Time.time < lastStartedTime + wobbleTime / 2.0f + pauseTime) {
                // Pause at targetRotation.
                transform.localRotation = targetRotation;
            } else {
                // Wobble from targetRotation to initialRotation.
                float t = Mathf.InverseLerp(lastStartedTime + wobbleTime / 2.0f + pauseTime,
                                            lastStartedTime + wobbleTime,
                                            Time.time);
                transform.localRotation = Quaternion.Lerp(targetRotation, initialRotation, t);
            }
        } else if (Time.time < lastStartedTime + wobbleTime + cooldown) {
            // Wait until cooldown passes.
            transform.localRotation = initialRotation;
        } else {
            StartWobble();
        }
    }
}
