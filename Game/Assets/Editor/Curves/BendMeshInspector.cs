using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(BendMesh))]
public class BendMeshInspector : Editor
{
    private BendMesh bendMesh;
    private Transform handleTransform;
    private Quaternion handleRotation;

    private const float markerSize = 0.3f;

    private void OnSceneGUI() {
        bendMesh = target as BendMesh;
        handleTransform = bendMesh.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ?
                         handleTransform.rotation : Quaternion.identity;

        Handles.color = Color.gray;
        Handles.DrawLine(bendMesh.GetStartWorld(), bendMesh.GetEndWorld());

        Handles.color = Color.green;
        Handles.SphereCap(0, bendMesh.GetStartWorld(), handleRotation,
            HandleUtility.GetHandleSize(bendMesh.GetStartWorld()) * markerSize);
        Handles.color = Color.red;
        Handles.SphereCap(0, bendMesh.GetEndWorld(), handleRotation,
            HandleUtility.GetHandleSize(bendMesh.GetEndWorld()) * markerSize);

        EditorGUI.BeginChangeCheck();
        Vector3 start = Handles.DoPositionHandle(bendMesh.GetStartWorld(), handleRotation);
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(bendMesh, "Move Start Point");
            EditorUtility.SetDirty(bendMesh);
            bendMesh.SetStartWorld(start);
        }

        EditorGUI.BeginChangeCheck();
        Vector3 end = Handles.DoPositionHandle(bendMesh.GetEndWorld(), handleRotation);
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(bendMesh, "Move End Point");
            EditorUtility.SetDirty(bendMesh);
            bendMesh.SetEndWorld(end);
        }
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (GUILayout.Button("Bend and Save Meshes")) {
            Undo.RecordObject(bendMesh, "Bend Mesh");
            bendMesh.PerformBend(true);
            EditorUtility.SetDirty(bendMesh);
        }
    }
}
