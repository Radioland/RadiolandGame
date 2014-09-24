using UnityEngine;
using System.Collections;

public class PlatformInteraction : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	void OnTriggerEnter(Collider c) {
		if (c.gameObject.tag == "kill") {
			Destroy(gameObject);
			Instantiate((GameObject)Resources.Load ("Prefabs/Character"));
		}


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
