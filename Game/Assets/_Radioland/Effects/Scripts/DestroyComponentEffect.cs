using UnityEngine;
using System.Collections;

public class DestroyComponentEffect : Effect
{
    // Variables to specify in the editor.
    [SerializeField] private Component componentToDestroy;

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

        if (componentToDestroy) {
            Destroy(componentToDestroy);
        }
    }

    public override void EndEffect() {
        base.EndEffect();
    }
}
