using UnityEngine;
using System.Collections;

public class ObjectPickup : MonoBehaviour {
	GameObject wallManager;

	float gravityTime;
	bool lowGravity;

	float jumpTime;
	bool highJump;
	// Use this for initialization
	void Start () {
		wallManager = GameObject.Find("Hidden Wall Manager");

		lowGravity = false;
		gravityTime = Time.time;

		highJump = false;
		jumpTime = Time.time;
	}

	void OnTriggerEnter (Collider c) {
		if (c.gameObject.tag == "pickupable") {
			Destroy(c.gameObject);
			wallManager.GetComponent<HiddenWallManager>().IncrementTargets();
		}
		if (c.gameObject.tag == "antigravity") {
			Destroy(c.gameObject);
			lowGravity = true;
			GetComponent<ThirdPersonController>().gravity = 10;
			gravityTime = Time.time;
		}
		if (c.gameObject.tag == "highjump") {
			Destroy(c.gameObject);
			highJump = true;
			GetComponent<ThirdPersonController>().jumpHeight = 2;
			jumpTime = Time.time;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (lowGravity) {
			GetComponent<GUIManager>().SetAntiGravity(true,5 - (Time.time - gravityTime));
			if (Time.time - gravityTime > 5) {
				lowGravity = false;
				GetComponent<ThirdPersonController>().gravity = 20;
				GetComponent<GUIManager>().SetAntiGravity(false,0);
			}
		}
		if (highJump) {
			GetComponent<GUIManager>().SetHighJump(true,5 - (Time.time - jumpTime));
			if (Time.time - jumpTime > 5) {
				highJump = false;
				GetComponent<ThirdPersonController>().jumpHeight = 1;
				GetComponent<GUIManager>().SetHighJump(false,0);
			}
		}
	}
	
}
