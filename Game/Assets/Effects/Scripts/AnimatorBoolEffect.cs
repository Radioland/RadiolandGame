using UnityEngine;
using System.Collections;

public class AnimatorBoolEffect : Effect
{
    public Animator animator;
    public string boolToSet;
    
    static GameObject orphanedEffectParent;
    
    protected override void Awake() {
        base.Awake();

        if (!animator) {
            animator = gameObject.GetComponent<Animator>();
        }
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

        animator.SetBool(boolToSet, true);
    }
    
    public override void EndEffect() {
        base.EndEffect();
        
        animator.SetBool(boolToSet, false);
    }
}
