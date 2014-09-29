using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {

	bool antigravity;
	float gravityTimer;

	bool highJump;
	float jumpTimer;

	float horizontalSliderValue;
	float secondaryHorizontalSliderValue;

	bool accessed;

	// Use this for initialization
	void Start () {
		antigravity = false;
		gravityTimer = 0;

		highJump = false;
		jumpTimer = 0;

		horizontalSliderValue = 5f;
		secondaryHorizontalSliderValue = 5f;

		accessed = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (horizontalSliderValue < 2 && secondaryHorizontalSliderValue > 8) {
			if (!accessed) {
				Debug.Log ("some kinda secret was given to the player");
				accessed = true;
			}
		}
	}

	public void SetAntiGravity(bool enabled, float timeLeft) {
		antigravity = enabled;
		gravityTimer = timeLeft;
	}

	public void SetHighJump(bool enabled, float timeLeft) {
		highJump = enabled;
		jumpTimer = timeLeft;
	}

	void OnGUI() {
		GUI.skin = (GUISkin)Resources.Load ("GUI/Powerups");
		horizontalSliderValue = GUI.HorizontalSlider(new Rect(10,Screen.height - Screen.height/6,Screen.width / 10, Screen.height / 20),horizontalSliderValue,0f,10f);
		secondaryHorizontalSliderValue = GUI.HorizontalSlider(new Rect(10,Screen.height - Screen.height/9,Screen.width / 10, Screen.height / 20),secondaryHorizontalSliderValue,0f,10f);
		if (antigravity) {
			GUI.Box(new Rect(10,10,30,20),"" + gravityTimer.ToString("0"));
		}
		if (highJump) {
			GUI.Box(new Rect(50,10,30,20),"" + jumpTimer.ToString("0"));
		}
	}
}
