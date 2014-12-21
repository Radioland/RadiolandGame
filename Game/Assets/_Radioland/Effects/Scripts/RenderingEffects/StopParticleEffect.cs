﻿using UnityEngine;
using System.Collections;

public class StopParticleEffect : Effect
{
    // Variables to specify in the editor.
    [SerializeField] private ParticleSystem effectParticleSystem;
    
    protected override void Awake() {
        base.Awake();
    }
    
    protected override void Start() {
        base.Start();
    }
    
    protected override void Update() {
        base.Update();
    }
    
    public override void TriggerEffect() {
        base.TriggerEffect();
    }
    
    public override void StartEffect() {
        base.StartEffect();
        
        effectParticleSystem.enableEmission = false;
    }
    
    public override void EndEffect() {
        base.EndEffect();
    }
}