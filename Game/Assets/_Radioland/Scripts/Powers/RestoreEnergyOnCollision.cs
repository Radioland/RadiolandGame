using UnityEngine;
using System.Collections;

public class RestoreEnergyOnCollision : MonoBehaviour
{
    [SerializeField] private float energy = 0.1f;

    void Awake() {

    }

    void Start() {

    }

    void Update() {

    }

    void OnCollisionEnter(Collision collision) {
        RestoreEnergyIfAble(collision.collider.gameObject);
    }

    void OnTriggerEnter(Collider other) {
        RestoreEnergyIfAble(other.gameObject);
    }

    void RestoreEnergyIfAble(GameObject otherObject) {
        PowerupManager powerupManager = otherObject.GetComponentInChildren<PowerupManager>();
        
        if (powerupManager) {
            powerupManager.energy += energy;
            this.enabled = false;
        }
    }
}
