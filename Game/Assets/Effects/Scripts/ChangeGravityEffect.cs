using UnityEngine;
using System.Collections;

public class ChangeGravityEffect : Effect
{
    public GameObject objectToChange;
    public bool defaultToSelf = true;
    public bool useGravity = true;

    protected override void Awake() {
        base.Awake();

        if (!objectToChange && defaultToSelf) {
            objectToChange = gameObject;
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

        if (objectToChange) {
            objectToChange.rigidbody.useGravity = useGravity;
        }
    }

    public override void EndEffect() {
        base.EndEffect();
    }
}
