using UnityEngine;
using System.Collections;

// A region in space, defined either from a central point or a corner.
// Gizmos are used to draw the region in the Scene view.
// If a box collider is attached to this GameObject, its center and size will be
//     set automatically using the provided dimensions.

[ExecuteInEditMode]
public class Region : MonoBehaviour
{
    public Vector3 dimensions = new Vector3(35, 10, 10);
    [SerializeField] private bool isCentered = false;
    [Tooltip("Scale with global scale instead of local")]
    [SerializeField] private bool relativeDimensions = true;
    [SerializeField] private bool allowRotation = true;
    [SerializeField] private Color gizmoColor = Color.yellow;

    private Vector3 globalCenter;
    private Vector3 localCenter;
    private Vector3 colliderCenter;
    private Vector3 gizmoCenter;
    private BoxCollider boxCollider;

    private Vector3 scaledDimensions {
        get {
            if (relativeDimensions) {
                return new Vector3(dimensions.x * transform.lossyScale.x,
                                   dimensions.y * transform.lossyScale.y,
                                   dimensions.z * transform.lossyScale.z);
            } else {
                return dimensions;
            }
        }
    }

    private void Awake() {
        boxCollider = gameObject.GetComponent<BoxCollider>();

        UpdateProperties();
    }

    private void Start() {

    }

    private void Update() {
        UpdateProperties();
    }

    private void UpdateProperties() {
        if (isCentered) {
            globalCenter = transform.position;
            localCenter  = Vector3.zero;
        } else {
            globalCenter = new Vector3(transform.position.x + scaledDimensions.x / 2.0f,
                                       transform.position.y + scaledDimensions.y / 2.0f,
                                       transform.position.z + scaledDimensions.z / 2.0f);
            localCenter = new Vector3(dimensions.x / 2.0f,
                                      dimensions.y / 2.0f,
                                      dimensions.z / 2.0f);
        }

        if (boxCollider) {
            boxCollider.center = localCenter;
            boxCollider.size = dimensions;
        }
    }

    // Boundary value accessors. These do NOT account for rotation.
    public float GetMinX() { return globalCenter.x - scaledDimensions.x / 2; }
    public float GetMaxX() { return globalCenter.x + scaledDimensions.x / 2; }
    public float GetMinY() { return globalCenter.y - scaledDimensions.y / 2; }
    public float GetMaxY() { return globalCenter.y + scaledDimensions.y / 2; }
    public float GetMinZ() { return globalCenter.z - scaledDimensions.z / 2; }
    public float GetMaxZ() { return globalCenter.z + scaledDimensions.z / 2; }

    public Vector3 GetCenter() { return globalCenter; }

    public Vector3 GetRandomPosition() {
        if (allowRotation) {
            return globalCenter + (transform.right * Random.Range(-scaledDimensions.x / 2, scaledDimensions.x / 2) +
                                   transform.up * Random.Range(-scaledDimensions.y / 2, scaledDimensions.y / 2) +
                                   transform.forward * Random.Range(-scaledDimensions.z / 2, scaledDimensions.z / 2));
        } else {
            float randomX = Random.Range(globalCenter.x - scaledDimensions.x / 2,
                                            globalCenter.x + scaledDimensions.x / 2);
            float randomY = Random.Range(globalCenter.y - scaledDimensions.y / 2,
                                            globalCenter.y + scaledDimensions.y / 2);
            float randomZ = Random.Range(globalCenter.z - scaledDimensions.z / 2,
                                            globalCenter.z + scaledDimensions.z / 2);
            return new Vector3(randomX, randomY, randomZ);
        }

    }

    private void OnDrawGizmos() {
        Gizmos.color = gizmoColor;

        Vector3 scale = relativeDimensions ? transform.lossyScale : transform.localScale;
        Quaternion rotation = allowRotation ? transform.rotation : Quaternion.identity;
        Matrix4x4 transformMatrix = Matrix4x4.TRS(transform.position,
                                                  rotation,
                                                  scale);
        Gizmos.matrix = transformMatrix;

        Gizmos.DrawWireCube(localCenter, dimensions);
    }
}
