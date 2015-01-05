using UnityEngine;
using System.Collections;

public class Leaves : MonoBehaviour
{
    [SerializeField] private EffectManager leavesEffects;
    [SerializeField] private AnimationCurve speedScale;
    [SerializeField] private RandomAudioEffect effectAudio;

    private CharacterMovement characterMovement;

    private void Awake() {
        characterMovement = gameObject.GetComponent<CharacterMovement>();
    }

    private void Start() {

    }

    private void Update() {

    }

    private void OnTriggerStay(Collider other) {
        TriggerLeaves(other);
    }

    private void OnTriggerEnter(Collider other) {
        TriggerLeaves(other);
    }

    private void TriggerLeaves(Collider other) {
        if ((characterMovement.moving || characterMovement.falling) && other.CompareTag("leaves")) {
            float scale = speedScale.Evaluate(characterMovement.controlSpeed);
            if (effectAudio) { effectAudio.ScaleVolume(scale); }

            if (leavesEffects) {
                leavesEffects.StartEvent();
            }
        }
    }
}
