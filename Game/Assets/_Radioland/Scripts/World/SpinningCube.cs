using UnityEngine;
using System.Collections;

public class SpinningCube : MonoBehaviour {
	[SerializeField] private float spinDelay;
	[SerializeField] private float flipDuration = 1f;
	[SerializeField] private bool oppositeDirection = false;
	private float rotationTime;
	private bool rotating;
	private float rotationDegrees;

	private Quaternion initialRotation;

	void Start () {
		rotationTime = Time.time;
		initialRotation = transform.rotation;
		rotating = true;
	}

	void Update () {
		if (rotating) {
			float t = (Time.time - rotationTime) / flipDuration;
			float angle = Mathf.Lerp(0f, 90f, t);
			if (oppositeDirection) { angle *= -1; }
			transform.rotation = initialRotation * Quaternion.Euler(0f, 0f, angle);
			if (t > 1) {
				rotating = false;
				rotationTime = Time.time;
			}
		}
		else {
			if (Time.time - rotationTime > spinDelay) {
				rotating = true;
				rotationTime = Time.time;
				initialRotation = transform.rotation;
			}
		}
	}
}
