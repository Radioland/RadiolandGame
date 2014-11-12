using UnityEngine;
using System.Collections;

public class EnergySource : MonoBehaviour
{
    [SerializeField] private float maxRestoreRadius = 10.0f;
    [SerializeField] private TriggerEffects restoreEffects;

    private Transform playerTransform;
    private PowerupManager powerupManager;

    void Awake() {
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

        if (!powerupManager.IsFullEnergy() && distance < maxRestoreRadius) {
            restoreEffects.enabled = true;
        } else {
            restoreEffects.enabled = false;
        }
    }
    
    void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, maxRestoreRadius);
    }
}
