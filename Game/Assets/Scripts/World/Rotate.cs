using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour
{
    [Tooltip("Euler angle degrees per second.")]
    [SerializeField] private Vector3 angularSpeed;

    void Awake() {

    }

    void Start() {

    }

    void Update() {
        transform.Rotate(angularSpeed * Time.deltaTime);
    }
}
