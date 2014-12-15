using UnityEngine;
using System.Collections;

public class MaxLifetime : MonoBehaviour
{
    public float maxTimeToLive = 10.0f;
    
    private float creationTime;
    
    void Awake() {
        creationTime = Time.time;
    }
    
    void Start() {
        
    }
    
    void Update() {
        if (Time.time - creationTime > maxTimeToLive) {
            Destroy(gameObject);
        }
    }
}
