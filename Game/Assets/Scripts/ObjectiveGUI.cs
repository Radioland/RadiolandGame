using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObjectiveGUI : MonoBehaviour {
	int targetsFound;
	int totalTargets;
	
	Text text;
	// Use this for initialization
	void Start () {
		targetsFound = 0;
		totalTargets = 5;
		text = GetComponent<Text>();
	}
	
	void OnTriggerEnter (Collider c) {
		if (c.gameObject.tag == "pickupable") {
			Destroy(c.gameObject);
			targetsFound += 1;
		}
	}
	
	// Update is called once per frame
	void Update () {
		text.text = targetsFound.ToString() + "/" + totalTargets.ToString();
	}
	
	public void SetTotalTargets(int targets) {
		totalTargets = targets;
	}
	
	public void IncrementTargets() {
		targetsFound += 1;
	}
}
