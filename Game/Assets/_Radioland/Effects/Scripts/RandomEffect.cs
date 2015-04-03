using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomEffect : Effect
{
    // Variables to specify in the editor.
    [SerializeField] private List<Effect> effects;

    private Effect currentEffect;

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

        currentEffect = effects[Random.Range(0, effects.Count)];
        currentEffect.TriggerEffect();
    }

    public override void EndEffect() {
        base.EndEffect();

        if (currentEffect) {
            currentEffect.EndEffect();
        }
    }
}
