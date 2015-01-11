using UnityEngine;
using System.Collections;

public class TranslateEffect : Effect
{
    [SerializeField] private Vector3 finalTranslationDelta;

    private void Reset() {
        timing = new EffectTiming();
        timing.timed = true;
    }

    protected override void Awake() {
        base.Awake();
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();

        if (hasStarted) {
            transform.position = transform.position +
                                 finalTranslationDelta / timing.duration * Time.deltaTime;
        }
    }

    public override void TriggerEffect() {
        base.TriggerEffect();
    }

    public override void StartEffect() {
        base.StartEffect();
    }

    public override void EndEffect() {
        base.EndEffect();
    }
}
