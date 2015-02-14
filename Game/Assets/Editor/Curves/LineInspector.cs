using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(Line))]
public class LineInspector : Editor
{
    private const float markerSize = 0.3f;

    private void OnSceneGUI() {
        // Fetch information about the Line.
        Line line = target as Line;
        Transform handleTransform = line.transform;
        Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local?
                                    handleTransform.rotation : Quaternion.identity;
        Vector3 p0 = handleTransform.TransformPoint(line.p0);
        Vector3 p1 = handleTransform.TransformPoint(line.p1);

        // Draw the line itself.
        Handles.color = Color.white;
        Handles.DrawLine(p0, p1);

        Handles.color = Color.green;
        Handles.SphereCap(0, p0, handleRotation, HandleUtility.GetHandleSize(p0) * markerSize);
        Handles.color = Color.red;
        Handles.SphereCap(0, p1, handleRotation, HandleUtility.GetHandleSize(p1) * markerSize);

        // Draw position handles and update point coordinates if values change.
        EditorGUI.BeginChangeCheck();
        p0 = Handles.DoPositionHandle(p0, handleRotation);
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(line, "Move Point");
            EditorUtility.SetDirty(line);
            line.p0 = handleTransform.InverseTransformPoint(p0);
        }
        EditorGUI.BeginChangeCheck();
        p1 = Handles.DoPositionHandle(p1, handleRotation);
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(line, "Move Point");
            EditorUtility.SetDirty(line);
            line.p1 = handleTransform.InverseTransformPoint(p1);
        }
    }
}
