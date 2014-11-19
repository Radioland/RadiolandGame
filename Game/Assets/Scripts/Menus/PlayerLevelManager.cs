using UnityEngine;
using System.Collections;

public class PlayerLevelManager : MonoBehaviour {

	private bool paused = false;

	[SerializeField] private GameObject background;

	void Awake() {
		PlayerPrefs.SetInt("level", Application.loadedLevel);
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.P)) {
			if (paused) {
				paused = false;
				Time.timeScale = 1;
				background.SetActive(false);
			}
			else {
				paused = true;
				//Timescale is not 0 because it throws an error with FollowPlatforms.cs line 58
				Time.timeScale = 0.0000000001f;
				background.SetActive(true);
			}
		}
	}
}
