using UnityEngine;
using System.Collections;

public class ICurve : MonoBehaviour
{
    private void Awake() {

    }

    private void Start() {

    }

    private void Update() {

    }

    public virtual Vector3 GetPoint(float t) {
        return Vector3.zero;
    }

    public virtual Vector3 GetVelocity(float t) {
        return Vector3.zero;
    }

    public virtual Vector3 GetDirection(float t) {
        return GetVelocity(t).normalized;
    }
}
