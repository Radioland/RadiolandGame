using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WobbleBend : MonoBehaviour
{
    #region Internal representation.
    [SerializeField] [Tooltip("Local coordinates")] private Vector3 referenceStart;
    [SerializeField] [Tooltip("Local coordinates")] private Vector3 referenceEnd;
    [SerializeField] private ICurve bendCurveA;
    [SerializeField] private ICurve bendCurveB;
    [SerializeField] private bool bendChildren = true;
    [SerializeField] private float bendSpeed = 0.2f;
    #endregion Internal representation.

    #region Public interface for reference points.
    public Vector3 GetStartWorld() { return transform.TransformPoint(referenceStart); }
    public Vector3 GetStartLocal() { return referenceStart; }
    public void SetStartWorld(Vector3 start) { referenceStart = transform.InverseTransformPoint(start); }
    public void SetStartLocal(Vector3 start) { referenceStart = start; }

    public Vector3 GetEndWorld() { return transform.TransformPoint(referenceEnd); }
    public Vector3 GetEndLocal() { return referenceEnd; }
    public void SetEndWorld(Vector3 end) { referenceEnd = transform.InverseTransformPoint(end); }
    public void SetEndLocal(Vector3 end) { referenceEnd = end; }
    #endregion Public interface for reference points.

    // Key: meshFilter InstanceID, Value: original meshFilter.sharedMesh.vertices
    private Dictionary<int, Vector3[]> originalVertices;

    public void Reset() {
        referenceStart = Vector3.zero;
        referenceEnd = new Vector3(5f, 0f, 0f);
    }

    private void Awake() {
        originalVertices = new Dictionary<int, Vector3[]>();

        if (bendChildren) {
            MeshFilter[] childMeshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter childMeshFilter in childMeshFilters) {
                originalVertices.Add(childMeshFilter.GetInstanceID(),
                                     childMeshFilter.sharedMesh.vertices);
            }
        } else {
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            originalVertices.Add(meshFilter.GetInstanceID(), meshFilter.sharedMesh.vertices);
        }
    }

    private void Start() {

    }

    private void Update() {
        PerformBend(Mathf.PingPong(Time.time * bendSpeed, 1f));
    }

    public void PerformBend(float t) {
        if (bendChildren) {
            MeshFilter[] childMeshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter childMeshFilter in childMeshFilters) {
                Bend(childMeshFilter, t);
            }
        } else {
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            Bend(meshFilter, t);
        }
    }

    private void Bend(MeshFilter meshFilter, float t) {
        Vector3[] vertices = originalVertices[meshFilter.GetInstanceID()].Clone() as Vector3[];

        Vector3 referenceVector = GetEndWorld() - GetStartWorld();
        float referenceLength = referenceVector.magnitude;

        Vector3 referenceForward = referenceVector.normalized;
        Vector3 referenceUp, referenceRight;
        CurveUtils.GetUpAndRight(referenceForward, out referenceUp, out referenceRight);

        for (int i = 0; i < vertices.Length; i++) {
            Vector3 vertexPosition = transform.TransformPoint(vertices[i]); // to world-space
            Vector3 differenceFromStart = vertexPosition - GetStartWorld();
            float lengthAlongReference = Vector3.Dot(referenceForward, differenceFromStart);
            float percentAlongReference = lengthAlongReference / referenceLength;

            Vector3 pointOnReference = GetStartWorld() + referenceVector * percentAlongReference;
            Vector3 differenceFromReference = vertexPosition - pointOnReference;
            float pointReferenceUp = Vector3.Dot(differenceFromReference, referenceUp);
            float pointReferenceRight = Vector3.Dot(differenceFromReference, referenceRight);

            Vector3 bendPointA = bendCurveA.GetPoint(percentAlongReference);
            Vector3 bendForwardA = bendCurveA.GetDirection(percentAlongReference);
            Vector3 bendUpA, bendRightA;
            CurveUtils.GetUpAndRight(bendForwardA, out bendUpA, out bendRightA);

            Vector3 bendPointB = bendCurveB.GetPoint(percentAlongReference);
            Vector3 bendForwardB = bendCurveB.GetDirection(percentAlongReference);
            Vector3 bendUpB, bendRightB;
            CurveUtils.GetUpAndRight(bendForwardB, out bendUpB, out bendRightB);

            Vector3 bendPoint = Vector3.Lerp(bendPointA, bendPointB, t);
            Vector3 bendUp = Vector3.Lerp(bendUpA, bendUpB, t);
            Vector3 bendRight = Vector3.Lerp(bendRightA, bendRightB, t);

            vertexPosition = bendPoint + bendUp * pointReferenceUp + bendRight * pointReferenceRight;
            vertices[i] = transform.InverseTransformPoint(vertexPosition); // to object-space
        }

        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.RecalculateBounds();
    }
}
