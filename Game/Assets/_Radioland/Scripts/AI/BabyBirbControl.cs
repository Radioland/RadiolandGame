using UnityEngine;
using System.Collections;

public class BabyBirbControl : MonoBehaviour
{
    public enum BirbState
    {
        Perched, Wander, Chasing
    }

    public BirbState state;

    private const float minActivateSignalStrength = 0.5f;
    [SerializeField] private float minActivateDistance = 10f;

    private GameObject player;

    private void Awake() {
        player = GameObject.FindWithTag("Player");
    }

    private void Start() {

    }

    private void Update() {

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
