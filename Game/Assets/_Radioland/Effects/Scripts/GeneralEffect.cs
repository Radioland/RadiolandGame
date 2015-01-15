using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class GeneralEffect : Effect
{
    [SerializeField] private UnityEvent triggerEvent;
    [SerializeField] private UnityEvent startEvent;
    [SerializeField] private UnityEvent endEvent;

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

        triggerEvent.Invoke();
    }

    public override void StartEffect() {
        base.StartEffect();

        startEvent.Invoke();
    }

    public override void EndEffect() {
        base.EndEffect();

        endEvent.Invoke();
    }
}
