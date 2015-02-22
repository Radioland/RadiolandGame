using UnityEngine;

public class TransformLerp : MonoBehaviour
{
    [SerializeField] private Vector3 finalTranslationDelta;
    [SerializeField] private Vector3 finalLocalRotation;
    [SerializeField] private float duration = 3f;

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private Quaternion initialRotation;
    private Quaternion targetRotation;

    private float currentTime;
    private float timeDamp;

    private static readonly Color gizmoBoxColor = Color.grey;
    private static readonly Color selectedGizmoBoxColor = Color.blue;

    private void Awake() {
        SetupTransformations();

        currentTime = 0f;
        timeDamp = 0f;
    }

    private void Start() {

    }

    private void SetupTransformations() {
        initialPosition = transform.position;
        targetPosition = initialPosition + finalTranslationDelta;

        initialRotation = transform.localRotation;
        targetRotation = Quaternion.Euler(finalLocalRotation);
    }

    private void Update() {

    }

    public void SetTime(float t) {
        currentTime = t;

        transform.position = Vector3.Lerp(initialPosition, targetPosition, currentTime);
        transform.localRotation = Quaternion.Lerp(initialRotation, targetRotation, currentTime);
    }

    public void SetTimeSmooth(float t) {
        SetTime(Mathf.SmoothDamp(currentTime, t, ref timeDamp, duration));
    }

    private void DrawGizmos() {
        if (!Application.isPlaying) {
            SetupTransformations();
        }

        Quaternion referenceRotation = transform.parent ?
                                           transform.parent.rotation * targetRotation :
                                           targetRotation;

        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter) {
            GL.wireframe = true;
            if (renderer) { renderer.sharedMaterial.SetPass(0); }
            Graphics.DrawMeshNow(meshFilter.sharedMesh, Matrix4x4.TRS(targetPosition, referenceRotation, transform.lossyScale));
            GL.wireframe = false;
        } else {
            Matrix4x4 gizmoMatrix = Matrix4x4.TRS(targetPosition, referenceRotation, Vector3.one);
            Gizmos.matrix = gizmoMatrix;

            if (renderer) {
                Gizmos.DrawWireCube(Vector3.zero, renderer.bounds.extents);
            } else if (collider) {
                Gizmos.DrawWireCube(Vector3.zero, collider.bounds.extents);
            }
        }
    }

    public void OnDrawGizmos() {
        Gizmos.color = gizmoBoxColor;
        DrawGizmos();
    }

    public void OnDrawGizmosSelected() {
        Gizmos.color = selectedGizmoBoxColor;
        DrawGizmos();
    }
}
