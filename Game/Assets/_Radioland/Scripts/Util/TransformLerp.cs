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

    private static readonly Color defaultColor = new Color(0.6f, 0.6f, 0.6f, 0.5f);
    private static readonly Color selectedColor = new Color(0.3f, 0.5f, 0.9f, 0.5f);
    private static Material highlightDefaultMaterial;
    private static Material highlightSelectedMaterial;

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

    private void DrawGizmos(bool selected=false) {
        if (!highlightDefaultMaterial) { highlightDefaultMaterial = Resources.Load<Material>("Materials/transform_target_default"); }
        if (!highlightSelectedMaterial) { highlightSelectedMaterial = Resources.Load<Material>("Materials/transform_target_selected"); }

        if (!Application.isPlaying) {
            SetupTransformations();
        }

        Quaternion referenceRotation = transform.parent ?
                                           transform.parent.rotation * targetRotation :
                                           targetRotation;

        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        Renderer objectRenderer = gameObject.GetComponent<Renderer>();
        if (meshFilter && objectRenderer) {
            if (selected) { highlightSelectedMaterial.SetPass(0); }
            else          { highlightDefaultMaterial.SetPass(0);  }
            Graphics.DrawMeshNow(meshFilter.sharedMesh, Matrix4x4.TRS(targetPosition, referenceRotation, transform.lossyScale));

            objectRenderer.sharedMaterial.SetPass(0);
            GL.wireframe = true;
            Graphics.DrawMeshNow(meshFilter.sharedMesh, Matrix4x4.TRS(targetPosition, referenceRotation, transform.lossyScale));
            GL.wireframe = false;
        } else {
            Matrix4x4 gizmoMatrix = Matrix4x4.TRS(targetPosition, referenceRotation, Vector3.one);
            Gizmos.matrix = gizmoMatrix;

            Gizmos.color = selected ? selectedColor : defaultColor;
            if (objectRenderer) {
                Gizmos.DrawWireCube(Vector3.zero, objectRenderer.bounds.extents);
            } else if (GetComponent<Collider>()) {
                Gizmos.DrawWireCube(Vector3.zero, GetComponent<Collider>().bounds.size);
            }
        }
    }

    public void OnDrawGizmos() {
        DrawGizmos();
    }

    public void OnDrawGizmosSelected() {
        DrawGizmos(selected:true);
    }
}
