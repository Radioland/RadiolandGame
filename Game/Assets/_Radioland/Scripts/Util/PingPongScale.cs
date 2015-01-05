using UnityEngine;
using System.Collections;

public class PingPongScale : MonoBehaviour
{
    [SerializeField] private float minScale = 1.0f;
    [SerializeField] private float maxScale = 1.0f;
    [SerializeField] private float speed = 1.0f;

    private void Awake() {

    }

    private void Start() {

    }

    private void Update() {
        float pingPong = Mathf.PingPong(Time.time * speed, 1.0f);
        float scaleFactor = Mathf.Lerp(minScale, maxScale, pingPong);
        transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }
}
