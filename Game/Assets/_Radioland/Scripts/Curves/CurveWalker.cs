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
    private Vector3[] checkPositions = new Vector3[3];
    private const float aheadScale = 0.8f;
    private float aheadDistance;
    private float sizeDistance;
    private float checkScale = 0.2f;
    private float checkSize;

    private void Awake() {
        easeFunction = Interpolate.Ease(easeType);

        myCollider = gameObject.GetComponentInChildren<Collider>();
        aheadDistance = myCollider ? myCollider.bounds.extents.x * aheadScale : 1f;
        sizeDistance = myCollider ? myCollider.bounds.extents.z * aheadScale : 1f;
        checkSize = myCollider ? myCollider.bounds.extents.magnitude * checkScale : 0.3f;
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
            Vector3 forward = curve.GetDirection(easedProgress);
            Vector3 right = Vector3.Cross(forward, transform.up);
            Debug.DrawRay(transform.position, forward, Color.magenta);

            // Check forward.
            int orientation = goingForward ? 1 : -1;
            checkPositions[0] = transform.position + forward * aheadDistance * orientation;

            // Check right and left.
            checkPositions[1] = transform.position + right * sizeDistance;
            checkPositions[2] = transform.position - right * sizeDistance;

            foreach (Vector3 checkPosition in checkPositions) {
                Collider[] hitColliders = Physics.OverlapSphere(checkPosition, checkSize);
                if (hitColliders.Any(hitCollider => hitCollider != myCollider)) { return; }
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

        easedProgress = easeFunction(0f, 1f, progress, 1f);

        Vector3 position = curve.GetPoint(easedProgress);
        transform.position = position;
        if (lookForward) {
            transform.LookAt(position + curve.GetDirection(easedProgress));
        }
    }

    public void OnDrawGizmos() {
        if (ignoreObstacles || !Application.isPlaying) { return; }

        Gizmos.color = Color.red;
        foreach (Vector3 checkPosition in checkPositions) {
            Gizmos.DrawWireSphere(checkPosition, checkSize);
        }
    }
}
