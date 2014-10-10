using UnityEngine;
using System.Collections;

public class PowerupManager : MonoBehaviour {
	public float powerupStart = 0;
	public float cooldownTime;
	CharacterMovement characterScript;
	// Use this for initialization
	void Start () {
		cooldownTime = 5f;
		characterScript = GetComponent<CharacterMovement>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time - powerupStart > cooldownTime) {
			EndPowerups();
			if (Input.GetKeyDown(KeyCode.H)) {
				HigherJump();
				powerupStart = Time.time;
				Debug.Log ("High Jump");
			}
			if (Input.GetKeyDown(KeyCode.G)) {
				LowerGravity();
				powerupStart = Time.time;
				Debug.Log ("Low Gravity");
			}
		}
	}

	void EndPowerups() {
		characterScript.SetJumpHeight(2f);
		characterScript.SetGravity(30f);
	}

	void HigherJump() {
		characterScript.SetJumpHeight(4f);
	}

	void LowerGravity() {
		characterScript.SetGravity(5f);
	}
}
