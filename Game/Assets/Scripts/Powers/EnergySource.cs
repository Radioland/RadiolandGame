using UnityEngine;
using System.Collections;

public class EnergySource : MonoBehaviour
{
    [SerializeField] private float maxRestoreRadius = 10.0f;
    [SerializeField] private TriggerEffects normalRestoreEffects;
    [SerializeField] private TriggerEffects danceRestoreEffects;
    
    private Animator animator;
    private int danceHash = Animator.StringToHash("isDance");

    private Transform playerTransform;
    private PowerupManager powerupManager;
    private Dance dance;

    void Awake() {
        animator = gameObject.GetComponentInChildren<Animator>();

        GameObject player = GameObject.FindWithTag("Player");

        playerTransform = player.transform;

        powerupManager = player.GetComponent<PowerupManager>();
        if (!powerupManager) {
            Debug.LogWarning(this.GetPath() + " could not find PowerupManager on " +
                             playerTransform.GetPath());
        }
        dance = player.GetComponent<Dance>();
        if (!dance) {
            Debug.LogWarning(this.GetPath() + " could not find Dance on " +
                             playerTransform.GetPath());
        }
    }

    void Start() {

    }

    void Update() {
        float distance = Vector3.Distance(playerTransform.position, transform.position);

        //if (!powerupManager.IsFullEnergy() && distance < maxRestoreRadius) {
        if (distance < maxRestoreRadius) {
            if (dance.dancing) {
                animator.SetBool(danceHash, true);
                normalRestoreEffects.enabled = false;
                danceRestoreEffects.enabled  = true;
            } else {
                animator.SetBool(danceHash, false);
                normalRestoreEffects.enabled = true;
                danceRestoreEffects.enabled  = false;
            }
        } else {
            normalRestoreEffects.enabled = false;
            danceRestoreEffects.enabled  = false;
        }
    }
    
    void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, maxRestoreRadius);
    }
}
