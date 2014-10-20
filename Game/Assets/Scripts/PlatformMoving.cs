using UnityEngine;
using System.Collections;

public class PlatformMoving : MonoBehaviour {

	float startTime;

	public float xVelocity;
	public float yVelocity;
	public float zVelocity;

	// Use this for initialization
	void Start () {
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time - startTime > 2) {
			xVelocity = -1 * xVelocity;
			startTime = Time.time;
		}

		transform.localPosition += new Vector3(xVelocity * Time.deltaTime,yVelocity * Time.deltaTime,zVelocity * Time.deltaTime);
	
	}
}
