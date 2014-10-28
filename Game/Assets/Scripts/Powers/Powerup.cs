using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PowerupManager))]

public class Powerup : MonoBehaviour
{
    [SerializeField] protected float duration = 5.0f;

    protected CharacterMovement characterMovement;
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

    public virtual void Start() {

    }

    public virtual void Update() {
        if (Time.time - lastStartedTime > duration) {
            EndPowerup();
        }
    }

    public virtual void UsePowerup() {
        lastStartedTime = Time.time;
    }
    
    public virtual void EndPowerup() {
        
    }
}
