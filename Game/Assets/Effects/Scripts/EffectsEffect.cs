using UnityEngine;
using System.Collections;

public class EffectsEffect : Effect
{
    // Variables to specify in the editor.
    [SerializeField] private EffectManager effects;
    [SerializeField] private bool overrideDurations = false;
    
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
        
        if (effects) {
            effects.StartEvent();
        }
    }
    
    public override void EndEffect() {
        base.EndEffect();

        if (effects && overrideDurations) {
            effects.StopEvent();
        }
    }
}
