using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BendMesh : MonoBehaviour
{
    #region Internal representation.
    [SerializeField] [Tooltip("Local coordinates")] private Vector3 referenceStart;
    [SerializeField] [Tooltip("Local coordinates")] private Vector3 referenceEnd;
    [SerializeField] private ICurve bendCurve;
    [SerializeField] private bool bendChildren = true;
    [SerializeField] private bool bendOnPlay = true;
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

    private const string bendMeshAssetPath = "Assets/_Radioland/ProceduralMeshes/";

    public void Reset() {
        referenceStart = Vector3.zero;
        referenceEnd = new Vector3(5f, 0f, 0f);
    }

    private void Awake() {
        if (bendOnPlay) {
            PerformBend(false);
        }
    }

    private void Start() {

    }

    private void Update() {

    }

    public void PerformBend(bool saveMeshes) {
        if (!bendCurve) { return; }

        Debug.Log("Bending Mesh on " + transform.GetPath() + ".");

        if (bendChildren) {
            MeshFilter[] childMeshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter childMeshFilter in childMeshFilters) {
                Bend(childMeshFilter, saveMeshes);
            }
        } else {
            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            Bend(meshFilter, saveMeshes);
        }
    }

    private void Bend(MeshFilter meshFilter, bool saveMeshes) {
        Mesh mesh;
        if (saveMeshes) {
            mesh = new Mesh();
            mesh.name = meshFilter.name + "_b";
            mesh.vertices = meshFilter.sharedMesh.vertices;
            mesh.uv = meshFilter.sharedMesh.uv;
            mesh.triangles = meshFilter.sharedMesh.triangles;
        } else {
            mesh = meshFilter.mesh;
        }

        Vector3[] vertices = mesh.vertices;

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

            Vector3 bendPoint = bendCurve.GetPoint(percentAlongReference);
            Vector3 bendForward = bendCurve.GetDirection(percentAlongReference);
            Vector3 bendUp, bendRight;
            CurveUtils.GetUpAndRight(bendForward, out bendUp, out bendRight);

            vertexPosition = bendPoint + bendUp * pointReferenceUp + bendRight * pointReferenceRight;
            vertices[i] = transform.InverseTransformPoint(vertexPosition); // to object-space
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        if (saveMeshes) {
            #if UNITY_EDITOR
            string meshName = meshFilter.name + "_mesh " + meshFilter.GetInstanceID() + ".asset";
            AssetDatabase.CreateAsset(mesh, bendMeshAssetPath + meshName);
            meshFilter.mesh = mesh;

            bendOnPlay = false;
            #endif
        }
    }
}
