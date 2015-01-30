using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreateTube : MonoBehaviour
{
    [SerializeField] private BezierSpline spline;
    [SerializeField] private GameObject proceduralMeshPrefab;
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private int lengthSegmentsPerCurve = 10;
    [SerializeField] private int radiusSegments = 8;
    [SerializeField] private int maxRefinementDepth = 2;

    [SerializeField] [Tooltip("Do not save created meshes as assets.")]
    private bool debug = false;

    // Refine recursively when the forward vectors are too dissimilar.
    // The dot product of perpendicular vectors is zero.
    // The dot product of parallel unit vectors is one.
    // Recurse in tube segment creation when the dot product of segment
    // start and end forward (direction) vectors is below this value.
    private const float refineTheshold = 0.98f;

    private const float gizmoScale = 0.5f;
    private const string tubeMeshAssetPath = "Assets/_Radioland/ProceduralMeshes/";

    private List<Vector3> vertices;
    private List<Vector2> uvs;
    private List<int> triangles;

    private void Reset() {
        spline = gameObject.GetComponentInChildren<BezierSpline>();

        if (spline) { Debug.Log("Found " + spline.GetPath() + " for " + this.GetPath()); }
    }

    private void Awake() {

    }

    private void Start() {

    }

    private void Update() {

    }

    public GameObject CreateNew() {
        if (!proceduralMeshPrefab) {
            Debug.LogError("Procedural Mesh Prefab is not set for " +
                           this.GetPath() + ", aborting CreateNew.");
            return null;
        }

        if (!spline) {
            Debug.LogError("Spline is not set for " +
                            this.GetPath() + ", aborting CreateNew.");
            return null;
        }

        GameObject tube = (GameObject)Instantiate(proceduralMeshPrefab);
        tube.transform.parent = transform;
        tube.transform.position = spline.transform.position;

        MeshFilter tubeMeshFilter = tube.GetComponent<MeshFilter>();
        if (!tubeMeshFilter) { tubeMeshFilter = tube.AddComponent<MeshFilter>(); }
        Mesh tubeMesh = new Mesh();
        tubeMesh.name = "Tube Mesh";
        tubeMesh.Clear();

        vertices = new List<Vector3>();
        uvs = new List<Vector2>();
        triangles = new List<int>();

        float radiansPerSegment = Mathf.PI * 2f / radiusSegments;

        if (!spline.loop) {
            Vector3 curveCenter, curveForward, curveUp, curveRight;
            float nextTheta, nextX, nextY;

            // Create the start cap.
            curveCenter = spline.GetPoint(0f);
            curveForward = spline.GetDirection(0f);
            CurveUtils.GetUpAndRight(curveForward, out curveUp, out curveRight);

            int centerVertex = vertices.Count;
            vertices.Add(curveCenter);
            vertices.Add(curveCenter + curveRight * radius);

            for (int i = 1; i <= radiusSegments; i++) {
                nextTheta = radiansPerSegment * i;

                nextX = radius * Mathf.Cos(nextTheta);
                nextY = radius * Mathf.Sin(nextTheta);

                vertices.Add(curveCenter + curveRight * nextX + curveUp * nextY);

                triangles.Add(centerVertex);
                triangles.Add(vertices.Count - 2);
                triangles.Add(vertices.Count - 1);
            }

            // Create the end cap.
            curveCenter = spline.GetPoint(1f);
            curveForward = spline.GetDirection(1f);
            CurveUtils.GetUpAndRight(curveForward, out curveUp, out curveRight);

            centerVertex = vertices.Count;
            vertices.Add(curveCenter);
            vertices.Add(curveCenter + curveRight * radius);

            for (int i = 1; i <= radiusSegments; i++) {
                nextTheta = radiansPerSegment * i;

                nextX = radius * Mathf.Cos(nextTheta);
                nextY = radius * Mathf.Sin(nextTheta);

                vertices.Add(curveCenter + curveRight * nextX + curveUp * nextY);

                triangles.Add(centerVertex);
                triangles.Add(vertices.Count - 1);
                triangles.Add(vertices.Count - 2);
            }
        }

        // Create segments along the length of the curve.
        int lengthSegments = lengthSegmentsPerCurve * spline.CurveCount;
        for (int i = 0; i < lengthSegments; i++) {
            float startT = i / (float)lengthSegments;
            float endT = (i + 1) / (float)lengthSegments;

            CreateTubeSegment(startT, endT, 0);
        }

        for (int i = 0; i < vertices.Count; i++) {
            // Transform from world-space to local-space.
            vertices[i] = tube.transform.InverseTransformPoint(vertices[i]);
            uvs.Add(new Vector2(0, 0));
        }

        tubeMesh.vertices = vertices.ToArray();
        tubeMesh.triangles = triangles.ToArray();
        tubeMesh.uv = uvs.ToArray();

        tubeMesh.RecalculateNormals();
        tubeMesh.RecalculateBounds();
        tubeMesh.Optimize();
        tubeMeshFilter.mesh = tubeMesh;

        MeshCollider meshCollider = tubeMeshFilter.gameObject.GetComponent<MeshCollider>();
        if (meshCollider) {
            DestroyImmediate(meshCollider);
            tubeMeshFilter.gameObject.AddComponent<MeshCollider>();
        }

        if (debug) {
            tube.AddComponent<DestroyMeshWithObject>();
        } else {
            #if UNITY_EDITOR
            string meshName = "Tube Mesh " + tube.GetInstanceID() + ".asset";
            AssetDatabase.CreateAsset(tubeMesh, tubeMeshAssetPath + meshName);
            #endif
        }

        if (tubeMesh.triangles.Length == 0) {
            Debug.LogWarning("Failed to set triangles, aborting tube creation.");
            DestroyImmediate(tube);
            return null;
        }

        return tube;
    }

    /// <summary>
    /// Gets an averaged forward vector around time t.
    /// </summary>
    /// <param name="t">Time along the curve.</param>
    /// <returns></returns>
    private Vector3 GetAveragedForward(float t) {
        int lengthSegments = lengthSegmentsPerCurve * spline.CurveCount;

        float halfwayPreviousT = t - (0.5f / (float)lengthSegments);
        float halfwayNextT = t + (0.5f / (float)lengthSegments);

        Vector3 halfwayPreviousForward = spline.GetDirection(halfwayPreviousT);
        Vector3 halfwayNextForward = spline.GetDirection(halfwayNextT);
        return (halfwayPreviousForward + halfwayNextForward).normalized;
    }

    private void CreateTubeSegment(float startT, float endT, int currentDepth) {
        float radiansPerSegment = Mathf.PI * 2f / radiusSegments;

        Vector3 startCurveCenter = spline.GetPoint(startT);
        Vector3 endCurveCenter = spline.GetPoint(endT);

        // Hack to avoid "pinching" where the forward vector is zero.
        // This also smoothes some sections and works well with recursive refinement.
        Vector3 startCurveForward = Mathf.Approximately(startT, 0f) ?
            spline.GetDirection(startT) : GetAveragedForward(startT);
        Vector3 endCurveForward = Mathf.Approximately(endT, 1f) ?
            spline.GetDirection(endT) : GetAveragedForward(endT);

        // Refine recursively up to a max depth if forward vectors are too dissimilar.
        float dotProduct = Vector3.Dot(spline.GetDirection(startT), spline.GetDirection(endT));
        if (dotProduct < refineTheshold && currentDepth < maxRefinementDepth) {
            float middleT = (startT + endT) / 2.0f;
            CreateTubeSegment(startT, middleT, currentDepth + 1);
            CreateTubeSegment(middleT, endT, currentDepth + 1);
            return;
        }

        Vector3 startCurveUp, startCurveRight, endCurveUp, endCurveRight;
        CurveUtils.GetUpAndRight(startCurveForward, out startCurveUp, out startCurveRight);
        CurveUtils.GetUpAndRight(endCurveForward, out endCurveUp, out endCurveRight);

        if (startT <= 0f) {
            for (int i = 0; i < radiusSegments; i++) {
                float startTheta = radiansPerSegment * i;

                float startX = radius * Mathf.Cos(startTheta);
                float startY = radius * Mathf.Sin(startTheta);
                vertices.Add(startCurveCenter + startCurveRight * startX + startCurveUp * startY);
            }
        }

        // Compensate for twists in the spline.
        // If the up vectors are close to antiparallel, the spline twists.
        // Twist vertex pairings using a {radiusSegments / 2} offset.
        // [0:0, 1:1, 2:2, 3:3, 4:4, 5:5] twists to [0:3, 1:4, 2:5, 3:0, 4:1, 5:2]
        bool twisted = Vector3.Dot(startCurveUp, endCurveUp) < 0;

        int previousStartVertex = vertices.Count - radiusSegments;
        int previousOffset = twisted ? radiusSegments / 2 : 0;

        int startVertex = vertices.Count;
        vertices.Add(endCurveCenter + endCurveRight * radius);

        for (int i = 0; i < radiusSegments - 1; i++) {
            if (Mathf.Approximately(endT, 1f) && spline.loop) { startVertex = 0; }

            float endTheta = radiansPerSegment * (i + 1);

            float endX = radius * Mathf.Cos(endTheta);
            float endY = radius * Mathf.Sin(endTheta);

            vertices.Add(endCurveCenter + endCurveRight * endX + endCurveUp * endY);

            triangles.Add(previousStartVertex + (previousOffset + i + 0) % radiusSegments);
            triangles.Add(startVertex + i + 0);
            triangles.Add(previousStartVertex + (previousOffset + i + 1) % radiusSegments);

            triangles.Add(previousStartVertex + (previousOffset + i + 1) % radiusSegments);
            triangles.Add(startVertex + i + 0);
            triangles.Add(startVertex + i + 1);
        }
        // Connect end back around to start.
        triangles.Add(previousStartVertex + (previousOffset + (radiusSegments - 1)) % radiusSegments);
        triangles.Add(startVertex + radiusSegments - 1);
        triangles.Add(previousStartVertex + previousOffset);

        triangles.Add(previousStartVertex + previousOffset);
        triangles.Add(startVertex + radiusSegments - 1);
        triangles.Add(startVertex);
    }

    public void OnDrawGizmosSelected() {
        if (!spline) { return; }

        int lengthSegments = lengthSegmentsPerCurve * spline.CurveCount;

        for (int i = 0; i < lengthSegments; i++) {
            float t = i / (float)lengthSegments;
            Vector3 curveCenter = spline.GetPoint(t);
            Vector3 curveForward = spline.GetDirection(t);
            //curveForward = GetAveragedForward(t);
            Vector3 curveUp, curveRight;
            CurveUtils.GetUpAndRight(curveForward, out curveUp, out curveRight);
            // Highlight problem points. This shows how GetAveragedForward is useful.
            if (curveForward.magnitude <= 0.0001) {
                Gizmos.color = Color.magenta;
                Gizmos.DrawCube(curveCenter, new Vector3(0.2f, 0.2f, 0.2f));
            }
            if (curveUp.magnitude <= 0.0001) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(curveCenter, new Vector3(0.1f, 0.1f, 0.1f));
            }

            Gizmos.color = Color.red;
            Gizmos.DrawLine(curveCenter, curveCenter + curveForward * gizmoScale);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(curveCenter, curveCenter + curveUp * gizmoScale);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(curveCenter, curveCenter + curveRight * gizmoScale);
        }
    }
}
