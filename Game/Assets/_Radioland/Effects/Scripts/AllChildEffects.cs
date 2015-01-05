using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AllChildEffects : Effect
{
    // Variables to specify in the editor.
    [SerializeField] private bool overrideDurations = false;

    private List<Effect> childEffectsList;

    protected override void Awake() {
        base.Awake();

        childEffectsList = new List<Effect>();
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    public override void TriggerEffect() {
        base.TriggerEffect();

        childEffectsList.Clear();

        Effect [] childEffects = gameObject.GetComponentsInChildren<Effect>();
        foreach (Effect childEffect in childEffects) {
            // Avoid infinite recursion that triggers a stack overflow...
            if (childEffect.gameObject != gameObject) {
                childEffect.StartEffect();
                childEffectsList.Add(childEffect);
            }
        }
    }

    public override void StartEffect() {
        base.StartEffect();
    }

    public override void EndEffect() {
        base.EndEffect();

        if (overrideDurations) {
            foreach (Effect childEffect in childEffectsList) {
                childEffect.EndEffect();
            }
        }
    }
}
