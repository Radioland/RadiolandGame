using UnityEngine;
using System.Collections;

public class Leaves : MonoBehaviour
{
    [SerializeField] private EffectManager leavesEffects;

    private CharacterMovement characterMovement;

    void Awake() {
        characterMovement = gameObject.GetComponent<CharacterMovement>();
    }

    void Start() {

    }

    void Update() {

    }

    void OnTriggerStay(Collider other) {
        TriggerLeaves(other);
    }

    void OnTriggerEnter(Collider other) {
        TriggerLeaves(other);
    }

    void TriggerLeaves(Collider other) {
        if ((characterMovement.moving || characterMovement.falling) && other.CompareTag("leaves")) {
            if (leavesEffects) {
                leavesEffects.StartEvent();
            }
        }
    }
}
