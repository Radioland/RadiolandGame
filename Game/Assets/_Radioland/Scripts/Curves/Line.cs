using UnityEngine;

public class Line : ICurve
{
    public Vector3 p0;
    public Vector3 p1;

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
        return p1 - p0;
    }

    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();

        Gizmos.color = Color.gray;
        Gizmos.DrawLine(GetPoint(0), GetPoint(1));
    }
}
