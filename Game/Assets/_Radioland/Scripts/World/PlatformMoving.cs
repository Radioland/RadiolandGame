using UnityEngine;
using System.Collections;

public class PlatformMoving : MonoBehaviour {

    private float startTime;
    public Vector3 velocity;
    private Vector3 topVelocity;

    public float movementTime;
    public float pauseTime;

    void Start () {
        topVelocity = velocity;
        startTime = Time.time;
    }
    
    void Update () {
        if (Time.time - startTime > movementTime) {
            velocity = new Vector3(0,0,0);
        }
        if (Time.time - startTime > pauseTime + movementTime) {
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
