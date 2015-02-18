using UnityEngine;
using System.Collections;

public class GhostMovement : MonoBehaviour {
	[SerializeField] private string state;
	private Transform target;
	private Vector3 targetPosition;

	private float startTime;

	private float velocity;
	private float maxVelocity;
	private float acceleration;
	private float rotationSpeed;

	private Quaternion wanderRotation;
	private float wanderRotationDuration;

	void Start () {
		target = GameObject.Find ("Gunther").transform;
		startTime = 0;
		wanderRotation = Random.rotation;
		wanderRotationDuration = Random.Range(0.5f, 2f);
		acceleration = Random.Range(0f,3f);
		rotationSpeed = 0.002f;
		maxVelocity = 2;
	}

	void Update () {
		if (velocity <= maxVelocity) {
			velocity = velocity + acceleration * Time.deltaTime;
		}

		if (state == "wander") {
			if (Time.time - startTime > 5) {
				Wander ();
			}
			if (Time.time - startTime < wanderRotationDuration) {
				transform.rotation = Quaternion.Slerp(transform.rotation, wanderRotation, Time.time * rotationSpeed);
			}
			else {
				transform.position += transform.forward * Time.deltaTime * velocity;
			}
		}
		else if (state == "chase") {
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position), 3f * Time.deltaTime);
			transform.position += transform.forward * velocity * Time.deltaTime;
		}
	}
	void Wander() {
		startTime = Time.time;
		wanderRotation = Random.rotation;
		wanderRotationDuration = Random.Range(0.5f, 2f);
		acceleration = Random.Range(0f,3f);
		velocity = 0;
	}
	public void ChangeState(string newState) {
		state = newState;
	}
}
