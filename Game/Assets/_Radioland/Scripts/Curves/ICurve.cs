using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ICurve : MonoBehaviour
{
    protected static float gizmoSphereRadius = 0.8f;
    protected static float gizmoBoxWidth = 0.9f;

    protected static readonly Color gizmoLineColor = Color.blue;
    protected static readonly Color gizmoBoxColor = new Color(0.2f, 0.3f, 0.5f);

    [HideInInspector] public readonly bool loop = false;
    [HideInInspector] public readonly int curveCount = 1;

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
        gizmoSphereRadius = PlayerPrefs.GetFloat("curveGizmoSphereRadius", gizmoSphereRadius);
        gizmoBoxWidth = PlayerPrefs.GetFloat("curveGizmoBoxWidth", gizmoBoxWidth);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(GetPoint(0), gizmoSphereRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GetPoint(1), gizmoSphereRadius);
    }
}
