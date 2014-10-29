using UnityEngine;
using System.Collections;

public class JumpPowerup : Powerup
{
    [SerializeField] protected EffectManager onJumpEffects;

    protected bool effectsStarted;

    public override void Awake() {
        base.Awake();

        effectsStarted = false;
    }

    public override void Start() {
        base.Start();
    }

    public override void Update() {
        base.Update();
    }

    // Called via SendMessage in CharacterMovement.
    protected void StartJump() {
        if (inUse) {
            onJumpEffects.StartEvent();
            effectsStarted = true;
        }
    }

    // Called via SendMessage in CharacterMovement.
    protected void Grounded() {
        if (effectsStarted) {
            onJumpEffects.StopEvent();
            effectsStarted = false;
        }
    }

    public override void UsePowerup() {
        base.UsePowerup();
    }

    public override void EndPowerup() {
        base.EndPowerup();
    }
}
