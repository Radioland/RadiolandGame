using UnityEngine;
using System.Collections;

public class ShrinkCube : MonoBehaviour {
	private Vector3 scaleValue;
	private Vector3 shrinkValue;
	private Vector3 initialScale;
	//Random value for speed.  Lower numbers = more speed
	private float shrinkDivider = 2;
	private bool shouldShrink = false;
	private bool shouldGrow = false;
	private bool disappeared = false;
	private float disappearedTime;
	private float growDelay = 5;

	void Start () {
		initialScale = transform.localScale;
		scaleValue = transform.localScale;
		shrinkValue = new Vector3(scaleValue.x / shrinkDivider, scaleValue.y / shrinkDivider, scaleValue.z / shrinkDivider);
	}

	
	void Update () {
		if (shouldShrink) {
			if (transform.localScale.x >= 0) {
				scaleValue -= shrinkValue * Time.deltaTime;
				transform.localScale = scaleValue;
			}
			else {
				shouldShrink = false;
				GetComponents<BoxCollider>()[0].enabled = false;
				GetComponents<BoxCollider>()[1].enabled = false;
				disappearedTime = Time.time;
				disappeared = true;
			}
		}
		if (Time.time - disappearedTime > growDelay && disappeared) {
			GetComponents<BoxCollider>()[0].enabled = true;
			GetComponents<BoxCollider>()[1].enabled = true;
			shouldGrow = true;
		}
		if (shouldGrow) {
			if (transform.localScale.x <= initialScale.x) {
				scaleValue += shrinkValue * Time.deltaTime;
				transform.localScale = scaleValue;
			}
			else {
				disappeared = false;
				transform.localScale = initialScale;
				scaleValue = initialScale;
				shouldGrow = false;
			}
		}
	}

	void OnTriggerEnter(Collider c) {
		if (c.gameObject.tag == "Player") {
			shouldShrink = true;
			shouldGrow = false;
			disappeared = false;
		}
	}
}
