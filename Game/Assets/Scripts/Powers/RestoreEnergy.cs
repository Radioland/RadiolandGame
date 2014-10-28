using UnityEngine;
using System.Collections;

public class RestoreEnergy : MonoBehaviour
{
    [SerializeField] private float maxRestoreRate = 0.1f;
    [SerializeField] private float maxRestoreRadius = 10.0f;

    private Transform playerTransform;
    private PowerupManager powerupManager;

    void Awake() {
        // Assumes only one player.
        GameObject player = GameObject.FindWithTag("Player");
        playerTransform = player.transform;
        powerupManager = player.GetComponent<PowerupManager>();
        if (!powerupManager) {
            Debug.LogWarning(this.GetPath() + " could not find PowerupManager on " +
                             playerTransform.GetPath());
        }
    }

    void Start() {

    }

    void Update() {
        float distance = Vector3.Distance(playerTransform.position, transform.position);

        float contribution = (1.0f - Mathf.InverseLerp(0.0f, maxRestoreRadius, distance));

        float energy = contribution * maxRestoreRate * Time.deltaTime;
        powerupManager.energy += energy;
    }

    void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, maxRestoreRadius);
    }
}
