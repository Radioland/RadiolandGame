using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SmoothMoveToTarget))]
public class BabyBirbControl : MonoBehaviour
{
    private enum BirbState
    {
        Perched, Wander, Chasing
    }

    [SerializeField] private BirbState state;

    private const float minActivateSignalStrength = 0.5f;
    [SerializeField] private float minActivateDistance = 10f;

    private GameObject player;
    private GameObject followTarget;
    private SmoothMoveToTarget smoothMoveToTarget;

    private void Awake() {
        player = GameObject.FindWithTag("Player");
        followTarget = GameObject.FindWithTag("babybirbtarget");

        smoothMoveToTarget = gameObject.GetComponent<SmoothMoveToTarget>();
    }

    private void Start() {
        smoothMoveToTarget.SetTarget(followTarget.transform);
        smoothMoveToTarget.enabled = (state == BirbState.Chasing);
    }

    private void Update() {
        smoothMoveToTarget.enabled = (state == BirbState.Chasing);
    }

    public void Activate(float signalStrength) {
        if (state == BirbState.Perched &&
            signalStrength > minActivateSignalStrength &&
            Vector3.Distance(transform.position, player.transform.position) < minActivateDistance) {

            state = BirbState.Chasing;
        }
    }

    public void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minActivateDistance);
    }
}
