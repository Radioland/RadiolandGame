using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PowerupManager))]

public class Powerup : MonoBehaviour
{
    [SerializeField] protected float duration = 5.0f;
    public float energyCost = 0.2f;
    public Color color;
    public bool primed;
    public bool inUse;

    [HideInInspector] public RadioStation radioStation;

    protected CharacterMovement characterMovement;
    protected PowerupManager powerupManager;
    protected float lastTriggeredTime;
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

        lastTriggeredTime = -1000.0f;
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
        // Unprime if duration is exceeded, do not stop in the middle of a usage.
        if (primed && !inUse && (Time.time - lastStartedTime > duration)) {
            EndPowerup();
        }
    }

    public bool HasStrongSignal() {
        return (!radioStation || radioStation.StrongSignal());
    }

    public bool HasSufficientResources() {
        return (!powerupManager || powerupManager.CanUsePowerup(this));
    }

    public virtual bool CanUsePowerup() {
        return !primed && HasSufficientResources() && HasStrongSignal();
    }

    public void TryToUsePowerup() {
        lastTriggeredTime = Time.time;

        if (CanUsePowerup()) {
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
        primed = true;
        powerupManager.energy -= energyCost;
    }

    public virtual void EndPowerup() {
        primed = false;
        inUse = false;
        powerupManager.SignalEndPowerup();
    }
}
