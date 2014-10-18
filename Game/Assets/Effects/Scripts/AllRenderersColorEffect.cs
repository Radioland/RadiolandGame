using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AllRenderersColorEffect : Effect
{
    // Variables to specify in the editor.
    [SerializeField] private GameObject gameObjectToChange;
    [SerializeField] private Color newColor;
    [SerializeField] private bool revertAtEnd = false;
    [SerializeField] private bool useFadeOutCurve = false;
    [SerializeField] private AnimationCurve fadeOutCurve;
    [SerializeField] private string shaderColorName = "_Color";
    [SerializeField] private bool isForeverLoop = false;

    private Renderer[] renderers;
    private List<Color> rendererOriginalColors;

    protected override void Awake() {
        base.Awake();

        renderers = gameObjectToChange.GetComponentsInChildren<Renderer>();

        rendererOriginalColors = new List<Color>();
        foreach (Renderer thisRenderer in renderers) {
            if (thisRenderer.material.HasProperty(shaderColorName)) {
                rendererOriginalColors.Add(thisRenderer.material.GetColor(shaderColorName));
            } else {
                Debug.LogWarning(thisRenderer.GetPath() + " has no material color named " + shaderColorName);
                // Keep renderers and rendererOriginalColors lists the same size (hack).
                rendererOriginalColors.Add(Color.white);
            }
        }
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();

        if (isPlaying && hasStarted && useFadeOutCurve) {
            float curveT = fadeOutCurve.Evaluate(percentDurationElapsed);

            for (int i = 0; i < renderers.Length; i++) {
                if (renderers[i].material.HasProperty(shaderColorName)) {
                    Color lerpColor = Color.Lerp(newColor, rendererOriginalColors[i], curveT);
                    renderers[i].material.SetColor(shaderColorName, lerpColor);
                }
            }
        }
    }

    public override void TriggerEffect() {
        base.TriggerEffect();
    }

    public override void StartEffect() {
        base.StartEffect();

        for (int i = 0; i < renderers.Length; i++) {
            if (renderers[i].material.HasProperty(shaderColorName)) {
                renderers[i].material.SetColor(shaderColorName, newColor);
            }
        }
    }

    public override void EndEffect() {
        if (!isForeverLoop) {
            base.EndEffect();

            if (revertAtEnd) {
                for (int i = 0; i < renderers.Length; i++) {
                    if (renderers[i].material.HasProperty(shaderColorName)) {
                        renderers[i].material.SetColor(shaderColorName, rendererOriginalColors[i]);
                    }
                }
            }
        }
    }
}
