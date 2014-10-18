using UnityEngine;
using System.Collections;

public class ChangeGravityEffect : Effect
{
    // Variables to specify in the editor.
    [SerializeField] private GameObject objectToChange;
    [SerializeField] private bool defaultToSelf = true;
    [SerializeField] private bool useGravity = true;

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
