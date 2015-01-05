using UnityEngine;
using System.Collections;

public class MaxLifetime : MonoBehaviour
{
    public float maxTimeToLive = 10.0f;

    private float creationTime;

    private void Awake() {
        creationTime = Time.time;
    }

    private void Start() {

    }

    private void Update() {
        if (Time.time - creationTime > maxTimeToLive) {
            Destroy(gameObject);
        }
    }
}
