using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour
{
    [Tooltip("Euler angle degrees per second.")]
    [SerializeField] private Vector3 angularSpeed;

    private void Awake() {

    }

    private void Start() {

    }

    private void Update() {
        transform.Rotate(angularSpeed * Time.deltaTime);
    }
}
