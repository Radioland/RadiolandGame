using UnityEngine;
using System.Collections;

public class HiddenWallManager : MonoBehaviour {
	public int targetsFound;
	// Use this for initialization
	void Start () {
		targetsFound = 0;
	}
	
	// Update is called once per frame
	void Update () {
		 
	}

	public void IncrementTargets() {
		targetsFound += 1;
		if (targetsFound == 2) {
			Destroy(GameObject.Find("Wall1").gameObject);
		}
		if (targetsFound == 5) {
			Destroy(GameObject.Find("Wall2").gameObject);
		}
	}
}
