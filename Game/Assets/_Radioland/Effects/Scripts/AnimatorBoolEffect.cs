using UnityEngine;
using System.Collections;

public class AnimatorBoolEffect : Effect
{
    // Variables to specify in the editor.
    [SerializeField] private Animator animator;
    [SerializeField] private string boolToSet;

    protected override void Awake() {
        base.Awake();

        if (!animator) {
            animator = gameObject.GetComponent<Animator>();
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

        if (animator && animator.gameObject.activeInHierarchy) {
            animator.SetBool(boolToSet, true);
        }
    }

    public override void EndEffect() {
        base.EndEffect();

        if (animator && animator.gameObject.activeInHierarchy) {
            animator.SetBool(boolToSet, false);
        }
    }
}
