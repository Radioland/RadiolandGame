using UnityEngine;
using System.Collections;

public class RestoreEnergyOnCollision : MonoBehaviour
{
    [SerializeField] private float energy = 0.1f;

    private void Awake() {

    }

    private void Start() {

    }

    private void Update() {

    }

    private void OnCollisionEnter(Collision collision) {
        RestoreEnergyIfAble(collision.collider.gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        RestoreEnergyIfAble(other.gameObject);
    }

    private void RestoreEnergyIfAble(GameObject otherObject) {
        PowerupManager powerupManager = otherObject.GetComponentInChildren<PowerupManager>();

        if (powerupManager) {
            powerupManager.energy += energy;
            this.enabled = false;
        }
    }
}
