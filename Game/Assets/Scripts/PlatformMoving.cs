using UnityEngine;
using System.Collections;

public class PlatformMoving : MonoBehaviour {

    float startTime;
    public Vector3 velocity;

    void Start () {
        startTime = Time.time;
    }
    
    void FixedUpdate () {
        if (Time.time - startTime > 2) {
            velocity = -1 * velocity;
            startTime = Time.time;
        }

        transform.localPosition += velocity * Time.deltaTime;
    
    }

    public Vector3 GetVelocity() {
        return velocity * Time.deltaTime;
    }
}
