using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFadeEffect : Effect {

    // Variables to specify in the editor.
    [SerializeField] private CanvasGroup canvasGroup;
    [Tooltip("Fade the screen out (and the texture in)? Otherwise does the opposite.")]
    [SerializeField] private AnimationCurve alphaAdjust;

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

        if (isPlaying && hasStarted) {
            canvasGroup.alpha = alphaAdjust.Evaluate(percentDurationElapsed);
        }
    }

    public override void TriggerEffect() {
        base.TriggerEffect();
    }

    public override void StartEffect() {
        base.StartEffect();

        canvasGroup.alpha = alphaAdjust.Evaluate(0f);
    }

    public override void EndEffect() {
        base.EndEffect();

        canvasGroup.alpha = alphaAdjust.Evaluate(1f);
    }
}
