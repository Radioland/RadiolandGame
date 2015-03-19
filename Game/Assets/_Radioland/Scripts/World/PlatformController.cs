using UnityEngine;
using System.Collections;

public class PlatformController : MonoBehaviour {

	private GameObject[] jumpControlledPlatforms;

	void Start () {
		jumpControlledPlatforms = GameObject.FindGameObjectsWithTag("jumpplatform");
	}

	void Update () {
	
	}

	public void ControlPlatforms() {
		for (int i = 0; i < jumpControlledPlatforms.Length; i++) {
			jumpControlledPlatforms[i].SendMessage("PerformAction");
		}
	}
}
