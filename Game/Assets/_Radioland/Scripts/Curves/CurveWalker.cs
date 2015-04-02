using UnityEngine;
using System.Collections;
using System.Linq;

public enum CurveWalkerMode
{
    Once,
    Loop,
    PingPong
}

public class CurveWalker : MonoBehaviour
{
    [SerializeField] private ICurve curve;

    [Header("Basic Settings")]
    [SerializeField] private float duration = 2f;
    [SerializeField] private float pauseTime = 0f;
    [SerializeField] private Interpolate.EaseType easeType = Interpolate.EaseType.EaseInOutQuad;
    [SerializeField] private bool lookForward = false;
    [SerializeField] private CurveWalkerMode mode = CurveWalkerMode.PingPong;

    [Header("Obstacle Settings")]
    [SerializeField] private bool ignoreObstacles = true;

    private float progress = 0f;
    private float easedProgress = 0f;
    private float timePaused = 0f;
    private bool goingForward = true;
    private bool paused = false;
    private Interpolate.Function easeFunction;

    private Collider myCollider;
    private Vector3 checkPosition;

    private void Awake() {
        easeFunction = Interpolate.Ease(easeType);

        myCollider = gameObject.GetComponentInChildren<Collider>();
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

        if (!ignoreObstacles) {
            int orientation = goingForward ? 1 : -1;
            float forward = myCollider ? myCollider.bounds.extents.x : 0f;
            Vector3 direction = curve.GetDirection(easedProgress);
            checkPosition = transform.position + direction * forward * orientation;

            Collider[] hitColliders = Physics.OverlapSphere(checkPosition, 0.3f);

            Debug.DrawRay(transform.position, direction, Color.magenta);

            if (hitColliders.Any(hitCollider => hitCollider != myCollider)) { return; }
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

        easedProgress = easeFunction(0f, 1f, progress, 1f);

        Vector3 position = curve.GetPoint(easedProgress);
        transform.position = position;
        if (lookForward) {
            transform.LookAt(position + curve.GetDirection(easedProgress));
        }
    }

    public void OnDrawGizmos() {
        if (ignoreObstacles) { return; }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(checkPosition, 1f);
    }
}
