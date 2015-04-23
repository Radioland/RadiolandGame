using UnityEngine;

public class TransformLerp : MonoBehaviour
{
    private enum RotateMode
    {
        Quaternion, Euler
    }

    [SerializeField] private RotateMode rotateMode = RotateMode.Quaternion;
    [SerializeField] private Vector3 finalTranslationDelta;
    [SerializeField] private Vector3 finalLocalRotation;
    [SerializeField] private float duration = 3f;
    [SerializeField] private bool allowRevert = true;

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private Quaternion initialRotationQuat;
    private Quaternion targetRotationQuat;
    private Vector3 initialRotationEuler;
    private Vector3 targetRotationEuler;

    private float currentTime;
    private float timeDamp;
    private bool runningAuto = false;

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

        initialRotationQuat = transform.localRotation;
        targetRotationQuat = Quaternion.Euler(finalLocalRotation);

        initialRotationEuler = transform.localRotation.eulerAngles;
        targetRotationEuler = finalLocalRotation;
    }

    private void Update() {
        if (runningAuto) {
            SetTime(currentTime + Time.deltaTime / duration);
        }
    }

    public void StartAuto() {
        runningAuto = true;
    }

    public void SetTime(float t) {
        currentTime = allowRevert ? t : Mathf.Max(currentTime, t);

        transform.position = Vector3.Lerp(initialPosition, targetPosition, currentTime);
        if (rotateMode == RotateMode.Quaternion) {
            transform.localRotation = Quaternion.Lerp(initialRotationQuat, targetRotationQuat, currentTime);
        } else {
            Vector3 rotation = Vector3.Lerp(initialRotationEuler, targetRotationEuler, currentTime);
            transform.localRotation = Quaternion.Euler(rotation);
        }
    }

    public void SetTimeSmooth(float t) {
        SetTime(Mathf.SmoothDamp(currentTime, t, ref timeDamp, duration));
    }

    private void DrawGizmos(bool selected=false) {
        if (!highlightDefaultMaterial) { highlightDefaultMaterial = Resources.Load<Material>("Materials/transform_target_default"); }
        if (!highlightSelectedMaterial) { highlightSelectedMaterial = Resources.Load<Material>("Materials/transform_target_selected"); }

        if (!Application.isPlaying) { SetupTransformations(); }

        Quaternion referenceRotation = transform.parent ?
                                           transform.parent.rotation * targetRotationQuat :
                                           targetRotationQuat;

        MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
        if (meshFilters.Length > 0) {
            foreach (MeshFilter filter in meshFilters) {
                Renderer meshObjectRenderer = filter.GetComponent<Renderer>();
                if (meshObjectRenderer) {
                    Vector3 rotationDifference = transform.eulerAngles - filter.transform.eulerAngles;
                    Quaternion rotation = referenceRotation * Quaternion.Euler(rotationDifference);
                    Vector3 positionDifference = transform.position - filter.transform.position;
                    Matrix4x4 matrix = Matrix4x4.TRS(targetPosition + positionDifference, rotation, filter.transform.lossyScale);

                    if (selected) { highlightSelectedMaterial.SetPass(0); }
                    else { highlightDefaultMaterial.SetPass(0); }
                    Graphics.DrawMeshNow(filter.sharedMesh, matrix);

                    meshObjectRenderer.sharedMaterial.SetPass(0);
                    GL.wireframe = true;
                    Graphics.DrawMeshNow(filter.sharedMesh, matrix);
                    GL.wireframe = false;
                }
            }
        } else {
            Matrix4x4 gizmoMatrix = Matrix4x4.TRS(targetPosition, referenceRotation, Vector3.one);
            Gizmos.matrix = gizmoMatrix;
            Gizmos.color = selected ? selectedColor : defaultColor;

            Bounds bounds = new Bounds(transform.position, Vector3.zero);

            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers) {
                if (!(renderer is ParticleSystemRenderer)) {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders) {
                bounds.Encapsulate(collider.bounds);
            }

            Gizmos.DrawWireCube(Vector3.zero, bounds.size);
        }
    }

    public void OnDrawGizmos() {
        DrawGizmos();
    }

    public void OnDrawGizmosSelected() {
        DrawGizmos(selected:true);
    }
}
