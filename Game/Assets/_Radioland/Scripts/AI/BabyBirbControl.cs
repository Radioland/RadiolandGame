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

    [Header("Activation")]
    [SerializeField] private float minActivateDistance = 10f;
    private const float minActivateSignalStrength = 0.5f;

    [Header("General Movement")]
    [SerializeField] private float slowRadius = 2f;
    [SerializeField] private float timeToTargetSpeed = 1f;

    [Header("Wander Movement")]
    [SerializeField] [Range(0f, 1f)] private float wanderFrequency = 0.5f;
    [SerializeField] private float wanderToChaseRange = 10f;
    [SerializeField] private float wanderXZRange = 3f;
    [SerializeField] private float wanderYMin = -1f;
    [SerializeField] private float wanderYMax = 5f;
    private const float reachTargetRadius = 1f;

    private GameObject player;
    private Transform playerFollowTarget;
    private AIMovement movement;

    private static GameObject followObjectParent;
    private Transform followTarget;
    private float distanceToTarget;

    private void Awake() {
        if (!followObjectParent) {
            followObjectParent = new GameObject("BabyBirb FollowObjectParent");
            followObjectParent.transform.position = Vector3.zero;
        }

        player = GameObject.FindWithTag("Player");
        playerFollowTarget = GameObject.FindWithTag("babybirbtarget").transform;

        GameObject followObject = new GameObject(name + "_followTarget");
        followObject.transform.parent = followObjectParent.transform;
        followTarget = followObject.transform;
        followTarget.position = transform.position;

        movement = gameObject.GetComponent<AIMovement>();
    }

    private void Start() {

    }

    private void Update() {
        switch (state) {
            case BirbState.Perched:
                HandlePerched();
                break;
            case BirbState.Chasing:
                HandleChasing();
                break;
            case BirbState.Wander:
                HandleWander();
                break;
            default:
                break;
        }

        Vector3 vectorToTarget = followTarget.position - transform.position;
        distanceToTarget = vectorToTarget.magnitude;
        float targetSpeed = movement.maxLinearSpeed * (distanceToTarget / slowRadius);

        Vector3 targetVelocity = vectorToTarget.normalized * targetSpeed;

        movement.linearAcceleration = (targetVelocity - movement.linearVelocity) / timeToTargetSpeed;

        if (movement.linearSpeed > 0) {
            Quaternion rotation = Quaternion.LookRotation(movement.linearVelocity, Vector3.up);
            transform.rotation = rotation;
        }

        if (distanceToTarget < reachTargetRadius) {
            UpdateState();
        }
    }

    private void HandlePerched() {

    }

    private void HandleChasing() {

    }

    private void HandleWander() {
        if (distanceToTarget > wanderToChaseRange) {
            SetState(BirbState.Chasing);
        }
    }

    private void UpdateState() {
        switch (state) {
            case BirbState.Perched:
                break;
            case BirbState.Chasing:
                SetState(BirbState.Wander);
                break;
            case BirbState.Wander:
                if (Random.value > wanderFrequency) {
                    SetState(BirbState.Chasing);
                }
                break;
            default:
                break;
        }
    }

    private void ChooseTarget() {
        switch (state) {
            case BirbState.Perched:
                followTarget.position = transform.position;
                break;
            case BirbState.Chasing:
                followTarget.position = playerFollowTarget.position;
                break;
            case BirbState.Wander:
                float offsetX = Random.Range(-wanderXZRange, wanderXZRange);
                float offsetY = Random.Range(-wanderYMin, wanderYMax);
                float offsetZ = Random.Range(-wanderXZRange, wanderXZRange);
                Vector3 offset = new Vector3(offsetX, offsetY, offsetZ);

                followTarget.position = playerFollowTarget.position + offset;
                break;
            default:
                break;
        }
    }

    public void Activate(float signalStrength) {
        if (state == BirbState.Perched &&
            signalStrength > minActivateSignalStrength &&
            Vector3.Distance(transform.position, player.transform.position) < minActivateDistance) {

            SetState(BirbState.Chasing);
        }
    }

    private void SetState(BirbState newState) {
        state = newState;
        ChooseTarget();
    }

    public void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minActivateDistance);
    }

    public void OnDrawGizmosSelected() {
        if (!Application.isPlaying) { return; }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(playerFollowTarget.position, slowRadius);
    }
}
