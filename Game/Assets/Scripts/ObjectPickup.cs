using UnityEngine;
using System.Collections;

public class ObjectPickup : MonoBehaviour {
	GameObject wallManager;
	// Use this for initialization
	void Start () {
		wallManager = GameObject.Find("Hidden Wall Manager");
	}

	void OnTriggerEnter (Collider c) {
		if (c.gameObject.tag == "pickupable") {
			Destroy(c.gameObject);
			wallManager.GetComponent<HiddenWallManager>().IncrementTargets();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
