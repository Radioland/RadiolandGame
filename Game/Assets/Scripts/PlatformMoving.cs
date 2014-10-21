using UnityEngine;
using System.Collections;

public class PlatformMoving : MonoBehaviour {

    private float startTime;
    public Vector3 velocity;
	private Vector3 topVelocity;

    void Start () {
        startTime = Time.time;
		topVelocity = velocity;
    }
    
    void FixedUpdate () {
        if (Time.time - startTime > 2) {
			velocity = new Vector3(0,0,0);
        }
		if (Time.time - startTime > 3) {
			velocity = -1 * topVelocity;
			topVelocity = velocity;
			startTime = Time.time;
		}

        transform.localPosition += velocity * Time.deltaTime;
    
    }

    public Vector3 GetVelocity() {
        return velocity * Time.deltaTime;
    }
}
