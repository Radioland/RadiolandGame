using UnityEngine;
using System.Collections;

public class ScreenFadeEffect : Effect {

    // Variables to specify in the editor.
    [SerializeField] private Texture2D fadeTexture;
    [Tooltip("Fade the screen out (and the texture in)? Otherwise does the opposite.")]
    [SerializeField] private bool fadeOut = true;
    [SerializeField] private AnimationCurve alphaAdjust;
    [SerializeField] private bool stayAfterEnd = false;

    private float alphaFadeValue;
    private bool hasPlayed;

    protected override void Awake() {
        base.Awake();
    }
    
    protected override void Start() {
        base.Start();
    }
    
    protected override void Update() {
        base.Update();

        if (isPlaying && hasStarted) {
            alphaFadeValue = Mathf.Clamp01(percentDurationElapsed);
            if (!fadeOut) {
                alphaFadeValue = 1.0f - alphaFadeValue;
            }
            float curveT = alphaAdjust.Evaluate(percentDurationElapsed);
            alphaFadeValue *= curveT;
        }
    }

    void OnGUI() {
        if ((isPlaying && hasStarted) || (hasPlayed && stayAfterEnd)) {
            GUI.color = new Color(alphaFadeValue, alphaFadeValue, alphaFadeValue, alphaFadeValue);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
        }
    }
    
    public override void TriggerEffect() {
        base.TriggerEffect();
    }
    
    public override void StartEffect() {
        base.StartEffect();

        hasPlayed = true;
    }
    
    public override void EndEffect() {
        base.EndEffect();
    }
}
