using UnityEngine;
using System.Collections;

public class TutorialConditionalEvents : MonoBehaviour
{
    [SerializeField] private EffectManager reachStationEffects;
    [SerializeField] private RadioStation reachStationTarget;
    [SerializeField] private EffectManager useNoEnergyEffects;
    [SerializeField] private EffectManager reachFullEnergyEffects;

    private GameObject player;
    private CharacterMovement characterMovement;
    private PowerupManager powerupManager;

    private void Awake() {
        player = GameObject.FindWithTag("Player");
        powerupManager = player.GetComponentInChildren<PowerupManager>();
    }

    private void Start() {

    }

    private void Update() {
        if (reachStationTarget.StrongSignal() && reachStationEffects && reachStationEffects.enabled) {
            reachStationEffects.StartEvent();
            reachStationEffects.enabled = false;
        }

        if (reachStationTarget.StrongSignal() && Input.GetButtonDown("UsePower") &&
            !powerupManager.IsFullEnergy() && useNoEnergyEffects && useNoEnergyEffects.enabled) {
            useNoEnergyEffects.StartEvent();
            useNoEnergyEffects.enabled = false;
        }

        if (powerupManager.IsFullEnergy() && reachFullEnergyEffects && reachFullEnergyEffects.enabled) {
            reachFullEnergyEffects.StartEvent();
            reachFullEnergyEffects.enabled = false;
        }
    }
}
