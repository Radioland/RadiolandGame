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
    [SerializeField] private bool lookForward = false;
    [SerializeField] private CurveWalkerMode mode = CurveWalkerMode.PingPong;

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

        Vector3 position = curve.GetPoint(progress);
        transform.position = position;
        if (lookForward) {
            transform.LookAt(position + curve.GetDirection(progress));
        }
    }
}
