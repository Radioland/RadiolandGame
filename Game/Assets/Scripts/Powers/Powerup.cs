using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PowerupManager))]

public class Powerup : MonoBehaviour
{
    [SerializeField] protected float duration = 5.0f;
    public float energyCost = 0.2f;
    public bool inUse;

    protected CharacterMovement characterMovement;
    protected PowerupManager powerupManager;
    protected float lastStartedTime;

    public float remainingTime {
        get {
            return Mathf.Max(0.0f, duration - (Time.time - lastStartedTime));
        }
    }

    public virtual void Awake() {
        GameObject playerCharacter = GameObject.FindWithTag("Player");
        characterMovement = playerCharacter.GetComponent<CharacterMovement>();

        if (!characterMovement) {
            Debug.LogWarning("Powerup could not find CharacterMovement component!");
            gameObject.SetActive(false);
        }

        lastStartedTime = -1000.0f;
    }

    public void SetPowerupManager(PowerupManager manager) {
        powerupManager = manager;
    }

    public virtual void Start() {
        if (!powerupManager) {
            Debug.LogWarning("Powerup was not connected to a PowerupManager!");
        }
    }

    public virtual void Update() {
        if (inUse && Time.time - lastStartedTime > duration) {
            EndPowerup();
        }
    }

    public void TryToUsePowerup() {
        if (powerupManager.CanUsePowerup(this)) {
            UsePowerup();
        } else {
            // TODO: effects?
            // Play a failure sound (short tap, beep, or click)?
            // Fizzle particle effect?
            Debug.Log("Tried to use a powerup when not able to.");
        }
    }

    public virtual void UsePowerup() {
        // If re-using this powerup, this first ends the current instance.
        powerupManager.SetActivePowerup(this);

        lastStartedTime = Time.time;
        inUse = true;
        powerupManager.energy -= energyCost;
    }
    
    public virtual void EndPowerup() {
        inUse = false;
        powerupManager.EndPowerup();
    }
}
