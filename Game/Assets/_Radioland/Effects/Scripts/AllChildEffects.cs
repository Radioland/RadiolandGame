using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AllChildEffects : Effect
{
    // Variables to specify in the editor.
    [SerializeField] private bool overrideDurations = false;

    private List<EffectManager> childEffectManagersList;

    protected override void Awake() {
        base.Awake();

        childEffectManagersList = new List<EffectManager>();
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

        childEffectManagersList.Clear();

        EffectManager[] childEffectManagers = gameObject.GetComponentsInChildren<EffectManager>();
        foreach (EffectManager childEffectManager in childEffectManagers) {
            // Avoid infinite recursion that triggers a stack overflow...
            if (childEffectManager.gameObject != gameObject) {
                childEffectManager.StartEvent();
                childEffectManagersList.Add(childEffectManager);
            }

        }
    }

    public override void EndEffect() {
        base.EndEffect();

        if (overrideDurations) {
            foreach (EffectManager childEffectManager in childEffectManagersList) {
                childEffectManager.StopEvent();
            }
        }
    }
}
