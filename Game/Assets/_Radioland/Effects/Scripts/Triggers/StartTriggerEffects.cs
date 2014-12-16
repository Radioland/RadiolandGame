using UnityEngine;
using System.Collections;

public class StartTriggerEffects : TriggerEffects
{
    protected override void Awake() {
        base.Awake();
    }

    void Start() {
        StartEvent();
    }

    void Update() {

    }
}
