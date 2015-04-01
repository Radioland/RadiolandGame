using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AIMovement))]
public class BabyBirbControl : MonoBehaviour
{
    private enum BirbState
    {
        Perched, Wander, Chasing
    }

    [SerializeField] private BirbState state;
    private GameObject player;
    private Transform followTarget;
    private AIMovement movement;

    [Header("Activation")]
    private const float minActivateSignalStrength = 0.5f;
    [SerializeField] private float minActivateDistance = 10f;

    [Header("Chasing")]
    [SerializeField] private float slowRadius = 2f;
    [SerializeField] private float timeToTargetSpeed = 1f;

    private void Awake() {
        player = GameObject.FindWithTag("Player");
        followTarget = GameObject.FindWithTag("babybirbtarget").transform;

        movement = gameObject.GetComponent<AIMovement>();
    }

    private void Start() {

    }

    private void Update() {
        switch (state) {
            case BirbState.Perched:
                break;
            case BirbState.Chasing:
                HandleChase();
                break;
            case BirbState.Wander:
                break;
            default:
                break;
        }

        transform.LookAt(followTarget);
    }

    private void HandleChase() {
        Vector3 directionToTarget = followTarget.position - transform.position;
        float distance = directionToTarget.magnitude;
        float targetSpeed = movement.maxLinearSpeed * (distance / slowRadius);

        Vector3 targetVelocity = directionToTarget.normalized * targetSpeed;

        movement.linearAcceleration = (targetVelocity - movement.linearVelocity) / timeToTargetSpeed;
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

    public void OnDrawGizmosSelected() {
        if (!Application.isPlaying) { return; }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(followTarget.position, slowRadius);
    }
}
