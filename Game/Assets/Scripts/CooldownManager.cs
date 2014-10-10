using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CooldownManager : MonoBehaviour {
	PowerupManager powerupScript;
	Text text;
	float displayValue;

	void Awake() {
		powerupScript = GameObject.Find("TestCharacter").GetComponent<PowerupManager>();
		text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		displayValue = 5 - (Time.time - powerupScript.powerupStart);
		if (displayValue < 0) {
			displayValue = 0;
		}
		text.text = displayValue.ToString();
	}


}
