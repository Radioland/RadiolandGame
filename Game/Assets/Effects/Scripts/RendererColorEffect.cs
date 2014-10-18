using UnityEngine;
using System.Collections;

public class RendererColorEffect : Effect
{
    public Renderer rendererToChange;
    public Color newColor;
    public bool revertAtEnd = false;
    public bool useFadeOutCurve = false;
    public AnimationCurve fadeOutCurve;

    private Color originalColor;
    
    protected override void Awake() {
        base.Awake();

        originalColor = rendererToChange.material.color;
    }
    
    protected override void Start() {
        base.Start();
    }
    
    protected override void Update() {
        base.Update();

        if (isPlaying && hasStarted && useFadeOutCurve) {
            float curveT = fadeOutCurve.Evaluate(percentDurationElapsed);
            Color lerpColor = Color.Lerp(newColor, originalColor, curveT);
            rendererToChange.material.color = lerpColor;
        }
    }
    
    public override void TriggerEffect() {
        base.TriggerEffect();
    }
    
    public override void StartEffect() {
        base.StartEffect();

        rendererToChange.material.color = newColor;
    }
    
    public override void EndEffect() {
        base.EndEffect();

        if (revertAtEnd) {
            rendererToChange.material.color = originalColor;
        }
    }
}
