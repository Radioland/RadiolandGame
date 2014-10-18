using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AllRenderersColorEffect : Effect
{
    public GameObject gameObjectToChange;
    public Color newColor;
    public bool revertAtEnd = false;
    public bool useFadeOutCurve = false;
    public AnimationCurve fadeOutCurve;
	public string shaderColorName = "_Color";
	public bool isForeverLoop = false;

    private Renderer[] renderers;
    private List<Color> rendererOriginalColors;

    protected override void Awake() {
        base.Awake();

        renderers = gameObjectToChange.GetComponentsInChildren<Renderer>();

        rendererOriginalColors = new List<Color>();
        foreach (Renderer thisRenderer in renderers) {
            //rendererOriginalColors.Add(thisRenderer.material.color);
            if (thisRenderer.material.HasProperty(shaderColorName)) {
			    rendererOriginalColors.Add(thisRenderer.material.GetColor (shaderColorName));
            } else {
                Debug.LogWarning(thisRenderer.GetPath() + " has no material color named " + shaderColorName);
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
                //renderers[i].material.color = lerpColor;
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
            //renderers[i].material.color = newColor;
			renderers[i].material.SetColor(shaderColorName, newColor);
        }
    }

    public override void EndEffect() {
		if (!isForeverLoop) {
	        base.EndEffect();

	        if (revertAtEnd) {
                for (int i = 0; i < renderers.Length; i++) {
                    //renderers[i].material.color = rendererOriginalColors[i];
                    if (renderers[i].material.HasProperty(shaderColorName)) {
                        renderers[i].material.SetColor(shaderColorName, rendererOriginalColors[i]);
                    }
	            }
	        }
        }
    }
}
