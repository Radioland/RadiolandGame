using UnityEngine;
using System.Collections;

public enum SplineWalkerMode
{
    Once,
    Loop,
    PingPong
}

public class SplineWalker : MonoBehaviour
{
    [SerializeField] private BezierSpline spline;
    [SerializeField] private float duration = 2f;
    [SerializeField] private bool lookForward = false;
    [SerializeField] private SplineWalkerMode mode = SplineWalkerMode.PingPong;

    private float progress;
    private bool goingForward = true;

    private void Awake() {

    }

    private void Start() {

    }

    private void Update() {
        if (goingForward) {
            progress += Time.deltaTime / duration;
            if (progress > 1f) {
                if (mode == SplineWalkerMode.Once) {
                    progress = 1f;
                } else if (mode == SplineWalkerMode.Loop) {
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

        Vector3 position = spline.GetPoint(progress);
        transform.position = position;
        if (lookForward) {
            transform.LookAt(position + spline.GetDirection(progress));
        }
    }
}
