using UnityEngine;
using System.Collections;

public class HighJump : Powerup
{
    [SerializeField] private float jumpHeight = 4.0f;
    [SerializeField] private EffectManager onJumpEffects;

    private bool effectsStarted;

    public override void Awake() {
        base.Awake();

        effectsStarted = false;
    }
    
    public override void Start() {
        base.Start();
    }
    
    public override void Update() {
        base.Update();

        if (Input.GetKeyDown(KeyCode.H)) {
            UsePowerup();
        }
    }

    // Called via SendMessage in CharacterMovement.
    void StartJump() {
        if (inUse) {
            onJumpEffects.StartEvent();
            effectsStarted = true;
        }
    }

    // Called via SendMessage in CharacterMovement.
    void Grounded() {
        if (effectsStarted) {
            onJumpEffects.StopEvent();
            effectsStarted = false;
        }
    }
    
    public override void UsePowerup() {
        base.UsePowerup();

        Debug.Log("Used High Jump.");

        characterMovement.SetJumpHeight(jumpHeight);
    }
    
    public override void EndPowerup() {
        base.EndPowerup();

        characterMovement.ResetJumpHeight();
    }
}
