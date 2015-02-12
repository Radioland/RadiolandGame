using UnityEngine;
using System.Collections;

public class ICurve : MonoBehaviour
{
    private const float gizmoSphereRadius = 0.8f;

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

    protected virtual void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(GetPoint(0), gizmoSphereRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GetPoint(1), gizmoSphereRadius);
    }
}
