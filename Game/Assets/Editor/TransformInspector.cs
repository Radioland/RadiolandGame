using UnityEngine;
using UnityEditor;

// Source: http://pastebin.com/E8ZeEb30

[CanEditMultipleObjects]
[CustomEditor(typeof(Transform), true)]
public class TransformInspector : Editor
{
    /// <summary>
    /// Draw the inspector widget.
    /// </summary>
    public override void OnInspectorGUI () {
        Transform t = (Transform)target;

        float oldLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 15;

        EditorGUILayout.BeginHorizontal();
        bool resetPos = GUILayout.Button("P", GUILayout.Width(20f));
        Vector3 position = EditorGUILayout.Vector3Field("", t.localPosition);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        bool resetRot = GUILayout.Button("R", GUILayout.Width(20f));
        Vector3 eulerAngles = EditorGUILayout.Vector3Field("", t.localEulerAngles);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        bool resetScale = GUILayout.Button("S", GUILayout.Width(20f));
        Vector3 scale = EditorGUILayout.Vector3Field("", t.localScale);
        EditorGUILayout.EndHorizontal();

        EditorGUIUtility.labelWidth = oldLabelWidth;

        if (resetPos) position = Vector3.zero;
        if (resetRot) eulerAngles = Vector3.zero;
        if (resetScale) scale = Vector3.one;

        if (GUI.changed) {
            Undo.RecordObject(t, "Transform Change");

            t.localPosition = FixIfNaN(position);
            t.localEulerAngles = FixIfNaN(eulerAngles);
            t.localScale = FixIfNaN(scale);
        }
    }

    private Vector3 FixIfNaN(Vector3 v) {
        if (float.IsNaN(v.x)) { v.x = 0; }
        if (float.IsNaN(v.y)) { v.y = 0; }
        if (float.IsNaN(v.z)) { v.z = 0; }
        return v;
    }}
