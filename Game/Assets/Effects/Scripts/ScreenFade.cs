using UnityEngine;
using System.Collections;

public class ScreenFade : Effect {

	public Texture2D m_color;
	public bool fadeOut = true;
	public AnimationCurve alphaAdjust;

	private float alphaFadeValue;

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
		if (isPlaying && hasStarted) {
			GUI.color = new Color(alphaFadeValue, alphaFadeValue, alphaFadeValue, alphaFadeValue);
			GUI.DrawTexture( new Rect(0, 0, Screen.width, Screen.height ), m_color );
		}
	}
	
	public override void TriggerEffect() {
		base.TriggerEffect();
	}
	
	public override void StartEffect() {
		base.StartEffect();
	}
	
	public override void EndEffect() {
		base.EndEffect();
	}
}
