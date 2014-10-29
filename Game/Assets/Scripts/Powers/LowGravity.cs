using UnityEngine;
using System.Collections;

public class LowGravity : Powerup
{
    [SerializeField] private float gravity = 4.0f;
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

        if (Input.GetKeyDown(KeyCode.G)) {
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

        Debug.Log("Used Low Gravity.");
        
        characterMovement.SetGravity(gravity);
    }
    
    public override void EndPowerup() {
        base.EndPowerup();
        
        characterMovement.ResetGravity();
    }
}
