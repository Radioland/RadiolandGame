using UnityEngine;
using System.Collections;
using System.Linq;

public class QuadraticCurve : ICurve
{
    public float speed = 40f;
    public Vector3 acceleration = new Vector3(0f, -30f, 0f);
    public bool stopOnCollision = true;
    public LayerMask layerMask;

    private const float tStep = 0.1f;
    private const float maxT = 10f;

    private void Awake() {

    }

    private void Start() {

    }

    private void Update() {

    }

    public void SetAsCopy(QuadraticCurve original) {
        transform.position = original.transform.position;
        transform.rotation = original.transform.rotation;

        speed = original.speed;
        acceleration = original.acceleration;
        stopOnCollision = original.stopOnCollision;
        layerMask = original.layerMask;
    }

    public override Vector3 GetPoint(float t) {
        Vector3 direction = transform.up;
        Vector3 position = (transform.position +
                            direction * t * speed +
                            acceleration * t * t * 0.5f);

        return position;
    }

    public override Vector3 GetVelocity(float t) {
        Vector3 direction = transform.up;
        Vector3 velocity = (direction * speed +
                            acceleration * t);
        return velocity;
    }

    protected override void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(GetPoint(0), gizmoSphereRadius);

        for (float t = 0; t < maxT - tStep; t += tStep) {
            Gizmos.matrix = Matrix4x4.identity;

            Vector3 p1 = GetPoint(t);
            Vector3 p2 = GetPoint(t + tStep);
            Vector3 direction = p2 - p1;
            float distance = direction.magnitude;
            Matrix4x4 transformMatrix;
            Vector3 center;

            RaycastHit[] hits = Physics.RaycastAll(GetPoint(t), direction, distance, layerMask).OrderBy(h=>h.distance).ToArray();
            bool hitObject = false;
            foreach (RaycastHit hit in hits.Where(hit => hit.collider.gameObject != gameObject)) {
                hitObject = true;

                Gizmos.color = Color.red;
                Gizmos.DrawSphere(hit.point, gizmoSphereRadius);

                Gizmos.color = gizmoLineColor;
                Gizmos.DrawLine(p1, hit.point);

                // Draw boxes.
                direction = hit.point - p1;
                distance = direction.magnitude;
                center = (p1 + hit.point) * 0.5f;
                transformMatrix = Matrix4x4.TRS(center, Quaternion.LookRotation(direction), Vector3.one);
                Gizmos.matrix = transformMatrix;
                Gizmos.color = gizmoBoxColor;
                Gizmos.DrawWireCube(Vector3.zero, new Vector3(gizmoBoxWidth, gizmoBoxWidth, distance));

                if (stopOnCollision) { break; }
            }

            if (stopOnCollision && hitObject) { break; }

            Gizmos.color = gizmoLineColor;
            Gizmos.DrawLine(p1, p2);

            // Draw boxes.
            direction = p2 - p1;
            distance = direction.magnitude;
            center = (p1 + p2) * 0.5f;
            transformMatrix = Matrix4x4.TRS(center, Quaternion.LookRotation(direction), Vector3.one);
            Gizmos.matrix = transformMatrix;
            Gizmos.color = gizmoBoxColor;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(gizmoBoxWidth, gizmoBoxWidth, distance));
        }
    }
}
