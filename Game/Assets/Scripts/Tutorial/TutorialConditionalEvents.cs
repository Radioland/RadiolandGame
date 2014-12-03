using UnityEngine;
using System.Collections;

public class TutorialConditionalEvents : MonoBehaviour
{
    [SerializeField] private EffectManager reachFullEnergyEffects;

    private GameObject player;
    private CharacterMovement characterMovement;
    private PowerupManager powerupManager;

    void Awake() {
        player = GameObject.FindWithTag("Player");
        powerupManager = player.GetComponentInChildren<PowerupManager>();
    }

    void Start() {

    }

    void Update() {
        if (powerupManager.IsFullEnergy() && reachFullEnergyEffects && reachFullEnergyEffects.enabled) {
            reachFullEnergyEffects.StartEvent();
            reachFullEnergyEffects.enabled = false;
        }
    }
}
