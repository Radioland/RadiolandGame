using UnityEngine;

public class TransformLerp : MonoBehaviour
{
    [SerializeField] private Vector3 finalTranslationDelta;
    [SerializeField] private Vector3 finalLocalRotation;
    [SerializeField] private float duration = 3f;

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private Quaternion initialRotation;
    private Quaternion targetRotation;

    private float currentTime;
    private float timeDamp;

    private void Awake() {
        initialPosition = transform.position;
        targetPosition = initialPosition + finalTranslationDelta;

        initialRotation = transform.localRotation;
        targetRotation = Quaternion.Euler(finalLocalRotation);

        currentTime = 0f;
        timeDamp = 0f;
    }

    private void Start() {

    }

    private void Update() {

    }

    public void SetTime(float t) {
        currentTime = t;

        transform.position = Vector3.Lerp(initialPosition, targetPosition, currentTime);
        transform.localRotation = Quaternion.Lerp(initialRotation, targetRotation, currentTime);
    }

    public void SetTimeSmooth(float t) {
        SetTime(Mathf.SmoothDamp(currentTime, t, ref timeDamp, duration));
    }
}
