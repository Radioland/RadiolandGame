﻿using UnityEngine;
using System.Collections;

public class JumpPowerup : Powerup
{
    [SerializeField] protected EffectManager onJumpEffects;
    [SerializeField] private float groundedExtraTime = 0.1f;

    protected Animator animator;

    public override void Awake() {
        base.Awake();

        animator = gameObject.GetComponentInChildren<Animator>();
        if (!animator) {
            Debug.LogWarning("No animator found on " + transform.GetPath());
        }
    }

    public override void Start() {
        base.Start();
    }

    public override void Update() {
        base.Update();

        if ((Time.time - lastTriggeredTime < groundedExtraTime) && CanUsePowerup()) {
            UsePowerup();
        }
    }

    public override bool CanUsePowerup() {
        return base.CanUsePowerup() && characterMovement.grounded;
    }

    // Called via SendMessage in CharacterMovement.
    protected virtual void StartJump() {
        if (primed && !inUse) {
            inUse = true;
        }
    }

    // Called via SendMessage in CharacterMovement.
    protected void Grounded() {
        if (inUse) {
            EndPowerup();
        }
    }

    public override void UsePowerup() {
        base.UsePowerup();

        if (onJumpEffects) {
            onJumpEffects.StartEvent();
        }
    }

    public override void EndPowerup() {
        base.EndPowerup();

        if (onJumpEffects) {
            onJumpEffects.StopEvent();
        }
    }
}
