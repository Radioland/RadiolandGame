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
    [SerializeField] private float pauseTime = 0f;
    [SerializeField] private Interpolate.EaseType easeType = Interpolate.EaseType.EaseInOutQuad;
    [SerializeField] private bool lookForward = false;
    [SerializeField] private CurveWalkerMode mode = CurveWalkerMode.PingPong;

    private float progress = 0f;
    private float timePaused = 0f;
    private bool goingForward = true;
    private bool paused = false;
    private Interpolate.Function easeFunction;

    private void Awake() {
        easeFunction = Interpolate.Ease(easeType);
    }

    private void Start() {

    }

    private void Update() {
        if (paused) {
            timePaused += Time.deltaTime;
            if (timePaused > pauseTime) {
                timePaused = 0f;
                paused = false;
            } else {
                return;
            }
        }

        if (goingForward) {
            progress += Time.deltaTime / duration;
            if (progress > 1f) {
                paused = true;
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
                paused = true;
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
