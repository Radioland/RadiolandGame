using UnityEngine;
using System.Collections;

public class PingPongRotate : MonoBehaviour
{
    [SerializeField] private Vector3 minRotation;
    [SerializeField] private Vector3 maxRotation;
    [SerializeField] private float speed = 1.0f;

    private void Awake() {

    }

    private void Start() {

    }

    private void Update() {
        float pingPong = Mathf.PingPong(Time.time * speed, 1.0f);
        Quaternion thisRotation = Quaternion.Slerp(Quaternion.Euler(minRotation), Quaternion.Euler(maxRotation), pingPong);
        transform.localRotation = thisRotation;
    }
}
