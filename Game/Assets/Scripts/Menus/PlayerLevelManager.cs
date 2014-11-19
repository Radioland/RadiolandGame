using UnityEngine;
using System.Collections;

public class PlayerLevelManager : MonoBehaviour {

	void Awake() {
		PlayerPrefs.SetInt("level", Application.loadedLevel);
	}
	// Update is called once per frame
	void Update () {
	
	}
}
