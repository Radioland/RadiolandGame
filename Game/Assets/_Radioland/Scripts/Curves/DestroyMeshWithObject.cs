using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DestroyMeshWithObject : MonoBehaviour
{
    private void Awake() {

    }

    private void Start() {

    }

    private void Update() {

    }

    public void OnDestroy() {
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter) {
            Debug.Log("Destroying Mesh on " + transform.GetPath() + " before deletion.");
            Mesh mesh = meshFilter.sharedMesh;
            DestroyImmediate(mesh);
        }
    }
}
