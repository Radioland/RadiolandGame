using UnityEngine;
using System.Collections;

public class RendererColorEffect : Effect
{
    // Variables to specify in the editor.
    [SerializeField] private Renderer rendererToChange;
    [SerializeField] private Color newColor;
    [SerializeField] private bool revertAtEnd = false;
    [SerializeField] private bool useFadeOutCurve = false;
    [SerializeField] private AnimationCurve fadeOutCurve;
    [SerializeField] private string shaderColorName = "_Color";

    private Color originalColor;

    protected override void Awake() {
        base.Awake();

        if (rendererToChange.material.HasProperty(shaderColorName)) {
            originalColor = rendererToChange.material.GetColor(shaderColorName);
        } else {
            Debug.LogWarning(rendererToChange.GetPath() + " has no material color named " + shaderColorName);
        }
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();

        if (isPlaying && hasStarted && useFadeOutCurve) {
            float curveT = fadeOutCurve.Evaluate(percentDurationElapsed);
            Color lerpColor = Color.Lerp(newColor, originalColor, curveT);
            if (rendererToChange.material.HasProperty(shaderColorName)) {
                rendererToChange.material.SetColor(shaderColorName, lerpColor);
            }
        }
    }

    public override void TriggerEffect() {
        base.TriggerEffect();
    }

    public override void StartEffect() {
        base.StartEffect();

        if (rendererToChange.material.HasProperty(shaderColorName)) {
            rendererToChange.material.SetColor(shaderColorName, newColor);
        }
    }

    public override void EndEffect() {
        base.EndEffect();

        if (revertAtEnd) {
            if (rendererToChange.material.HasProperty(shaderColorName)) {
                rendererToChange.material.SetColor(shaderColorName, originalColor);
            }
        }
    }
}
