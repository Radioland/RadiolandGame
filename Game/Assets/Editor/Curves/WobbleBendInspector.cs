using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(WobbleBend))]
public class WobbleBendInspector : Editor
{
    private WobbleBend wobbleBend;
    private Transform handleTransform;
    private Quaternion handleRotation;

    private const float markerSize = 0.3f;

    private void OnSceneGUI() {
        wobbleBend = target as WobbleBend;
        handleTransform = wobbleBend.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ?
                         handleTransform.rotation : Quaternion.identity;

        Handles.color = Color.gray;
        Handles.DrawLine(wobbleBend.GetStartWorld(), wobbleBend.GetEndWorld());

        Handles.color = Color.green;
        Handles.SphereCap(0, wobbleBend.GetStartWorld(), handleRotation,
            HandleUtility.GetHandleSize(wobbleBend.GetStartWorld()) * markerSize);
        Handles.color = Color.red;
        Handles.SphereCap(0, wobbleBend.GetEndWorld(), handleRotation,
            HandleUtility.GetHandleSize(wobbleBend.GetEndWorld()) * markerSize);

        EditorGUI.BeginChangeCheck();
        Vector3 start = Handles.DoPositionHandle(wobbleBend.GetStartWorld(), handleRotation);
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(wobbleBend, "Move Start Point");
            EditorUtility.SetDirty(wobbleBend);
            wobbleBend.SetStartWorld(start);
        }

        EditorGUI.BeginChangeCheck();
        Vector3 end = Handles.DoPositionHandle(wobbleBend.GetEndWorld(), handleRotation);
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(wobbleBend, "Move End Point");
            EditorUtility.SetDirty(wobbleBend);
            wobbleBend.SetEndWorld(end);
        }
    }
}
