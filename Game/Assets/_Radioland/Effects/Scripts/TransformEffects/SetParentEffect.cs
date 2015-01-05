using UnityEngine;
using System.Collections;

public class SetParentEffect : Effect
{
    // Variables to specify in the editor.
    [Tooltip("Leave empty to orphan on trigger.")]
    [SerializeField] private GameObject newParent;

    private static GameObject orphanedEffectParent;

    protected override void Awake() {
        base.Awake();

        if (!orphanedEffectParent) {
            orphanedEffectParent = new GameObject();
            orphanedEffectParent.name = "OrphanedEffectParent";
            orphanedEffectParent.transform.position = Vector3.zero;
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

        if (newParent) {
            gameObject.transform.parent = newParent.transform;
        } else {
            gameObject.transform.parent = orphanedEffectParent.transform;
        }
    }

    public override void EndEffect() {
        base.EndEffect();
    }
}
