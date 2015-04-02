using UnityEngine;

public class Line : ICurve
{
    public Vector3 p0;
    public Vector3 p1;

    private void Reset() {
        p0 = Vector3.zero;
        p1 = new Vector3(0f, 0f, 5f);
    }

    private void Awake() {

    }

    private void Start() {

    }

    private void Update() {

    }

    public override Vector3 GetPoint(float t) {
        return transform.TransformPoint(Vector3.Lerp(p0, p1, t));
    }

    public override Vector3 GetVelocity(float t) {
        return transform.TransformPoint(p1) - transform.TransformPoint(p0);
    }

    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();

        Vector3 p0w = transform.TransformPoint(p0);
        Vector3 p1w = transform.TransformPoint(p1);

        Gizmos.color = gizmoLineColor;
        Gizmos.DrawLine(p0w, p1w);

        Vector3 center = (p0w + p1w) * 0.5f;
        float distance = Vector3.Distance(p0w, p1w);
        Quaternion direction = Quaternion.LookRotation(p1w - p0w);

        Matrix4x4 transformMatrix = Matrix4x4.TRS(center, direction, Vector3.one);
        Gizmos.matrix = transformMatrix;

        Gizmos.color = gizmoBoxColor;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(gizmoBoxWidth, gizmoBoxWidth, distance));
    }
}
