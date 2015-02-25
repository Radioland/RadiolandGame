using UnityEngine;
using System.Collections;

public enum CurveWalkerMode
{
    Once,
    Loop,
    PingPong
}

public class CurveWalker : MonoBehaviour
{
    [SerializeField] private ICurve curve;
    [SerializeField] private float duration = 2f;
    [SerializeField] private Interpolate.EaseType easeType = Interpolate.EaseType.EaseInOutQuad;
    [SerializeField] private bool lookForward = false;
    [SerializeField] private CurveWalkerMode mode = CurveWalkerMode.PingPong;

    private float progress;
    private bool goingForward = true;
    private Interpolate.Function easeFunction;

    private void Awake() {
        easeFunction = Interpolate.Ease(easeType);
    }

    private void Start() {

    }

    private void Update() {
        if (goingForward) {
            progress += Time.deltaTime / duration;
            if (progress > 1f) {
                if (mode == CurveWalkerMode.Once) {
                    progress = 1f;
                } else if (mode == CurveWalkerMode.Loop) {
                    progress -= 1f;
                } else {
                    progress = 2f - progress;
                    goingForward = false;
                }
            }
        } else {
            progress -= Time.deltaTime / duration;
            if (progress < 0f) {
                progress = -progress;
                goingForward = true;
            }
        }

        float easedProgress = easeFunction(0f, 1f, progress, 1f);

        Vector3 position = curve.GetPoint(easedProgress);
        transform.position = position;
        if (lookForward) {
            transform.LookAt(position + curve.GetDirection(easedProgress));
        }
    }
}
