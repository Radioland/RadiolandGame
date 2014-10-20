using UnityEngine;
using System.Collections;

public class PlatformMoving : MonoBehaviour {

    float startTime;
    public Vector3 velocity;
    // Use this for initialization
    void Start () {
        startTime = Time.time;
    }
    
    // Update is called once per frame
    void Update () {
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
