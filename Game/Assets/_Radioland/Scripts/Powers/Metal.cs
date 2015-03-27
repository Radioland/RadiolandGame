using UnityEngine;
using System.Collections;

public class Metal : Powerup
{
    [SerializeField] private float mass = 10.0f;
    [SerializeField] private float gravity = 60.0f;
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] private float airSmoothDampTime = 0.10f;
    [SerializeField] private EffectManager metalEffects;

    public override void Awake() {
        base.Awake();

        Messenger.AddListener("BounceTriggered", OnBounceTriggered);
    }

    public override void Start() {
        base.Start();
    }

    public override void Update() {
        base.Update();

        if (inUse && Time.time - lastStartedTime > duration) {
            EndPowerup();
        }
    }

    protected void OnBounceTriggered() {
        if (inUse) {
            EndPowerup();
        }
    }

    public override void UsePowerup() {
        base.UsePowerup();

        inUse = true;

        if (metalEffects) {
            metalEffects.StartEvent();
        }

        characterMovement.SetMass(mass);
        characterMovement.SetGravity(gravity);
        characterMovement.SetJumpHeight(jumpHeight);
        characterMovement.SetAirSmoothDampTime(airSmoothDampTime);
    }

    public override void EndPowerup() {
        base.EndPowerup();

        if (metalEffects) {
            metalEffects.StopEvent();
        }

        characterMovement.ResetMass();
        characterMovement.ResetGravity();
        characterMovement.ResetJumpHeight();
        characterMovement.ResetAirSmoothDampTime();
    }
}
