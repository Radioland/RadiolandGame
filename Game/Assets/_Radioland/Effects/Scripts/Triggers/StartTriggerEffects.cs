using UnityEngine;
using System.Collections;

public class StartTriggerEffects : TriggerEffects
{
    protected override void Awake() {
        base.Awake();
    }

    private void Start() {
        StartEvent();
    }

    private void Update() {

    }
}
