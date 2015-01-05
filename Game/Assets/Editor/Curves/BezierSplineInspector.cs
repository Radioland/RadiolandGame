// Reference: http://catlikecoding.com/unity/tutorials/curves-and-splines/

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierSpline))]
public class BezierSplineInspector : Editor
{
    private BezierSpline spline;
    private Transform handleTransform;
    private Quaternion handleRotation;
    private const int lineSteps = 10;
    private const int stepsPerCurve = 10;
    private const float directionScale = 0.5f;
    private const float lineWidth = 8.0f;
    private float handleDotCapSize = 0.06f;
    private float handleSphereCapSize = 0.14f;
    private float pickSizeRatio = 1.2f;

    private int selectedIndex = -1;

    private static Color[] modeColors = {
        Color.white,
        Color.yellow,
        Color.cyan
    };

    /// <summary>
    /// Draws the spline and functional controls in the scene view.
    /// </summary>
    private void OnSceneGUI() {
        spline = target as BezierSpline;
        handleTransform = spline.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ?
                         handleTransform.rotation : Quaternion.identity;

        Vector3 p0 = ShowPoint(0);
        for (int i = 1; i < spline.ControlPointCount; i += 3) {
            Vector3 p1 = ShowPoint(i);
            Vector3 p2 = ShowPoint(i + 1);
            Vector3 p3 = ShowPoint(i + 2);

            Handles.color = Color.gray;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);
            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, lineWidth);
            p0 = p3;
        }

        ShowDirections();
    }

    /// <summary>
    /// Shows direction vectors along the spline.
    /// </summary>
    private void ShowDirections() {
        Handles.color = Color.green;
        Vector3 point = spline.GetPoint(0f);
        Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);
        int steps = stepsPerCurve * spline.CurveCount;
        for (int i = 1; i <= steps; i++) {
            point = spline.GetPoint(i / (float)steps);
            Handles.DrawLine(point, point + spline.GetDirection(i / (float)steps) * directionScale);
        }
    }

    /// <summary>
    /// Shows a point in the editor view and returns its position in world space.
    /// </summary>
    /// <returns>The point's position in world space.</returns>
    /// <param name="index">The index into <see cref="spline.points"/>.</param>
    private Vector3 ShowPoint(int index) {
        Vector3 point = handleTransform.TransformPoint(spline.GetControlPoint(index));

        // Draw a dot button for this point. If selected (clicked), draw a position handle.
        float size = HandleUtility.GetHandleSize(point);
        if (index == 0) { size *= 2f; } // Mark the start of the spline with a larger dot.
        Handles.color = modeColors[(int)spline.GetControlPointMode(index)];
        Handles.DrawCapFunction drawFunction;
        float buttonCapSize;
        if (spline.IsAnchorPoint(index)) {
            drawFunction = (Handles.DrawCapFunction)Handles.DotCap;
            buttonCapSize = size * handleDotCapSize;
        } else {
            drawFunction = (Handles.DrawCapFunction)Handles.SphereCap;
            buttonCapSize = size * handleSphereCapSize;
        }
        if (Handles.Button(point, handleRotation, buttonCapSize,
                           buttonCapSize * pickSizeRatio, drawFunction)) {
            selectedIndex = index;
            Repaint();
        }

        if (selectedIndex == index) {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleRotation);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(spline, "Move Point");
                EditorUtility.SetDirty(spline);
                spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
            }
        }

        return point;
    }

    /// <summary>
    /// Draws editable information fields in the inspector for the spline.
    /// </summary>
    public override void OnInspectorGUI() {
        spline = target as BezierSpline;

        EditorGUI.BeginChangeCheck();
        bool loop = EditorGUILayout.Toggle("Loop", spline.loop);
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(spline, "Toggle Loop");
            EditorUtility.SetDirty(spline);
            spline.loop = loop;
        }

        if (selectedIndex >= 0 && selectedIndex < spline.ControlPointCount) {
            DrawSelectedPointInspector();
        } else {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Select Start Point")) {
                selectedIndex = 0;
                Repaint();
            }
            if (GUILayout.Button("Select End Point")) {
                selectedIndex = spline.ControlPointCount - 1;
                Repaint();
            }
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Curve")) {
            Undo.RecordObject(spline, "Add Curve");
            spline.AddCurve();
            EditorUtility.SetDirty(spline);
        }
    }

    /// <summary>
    /// Draws information about the selected point in the inspector.
    /// Selections are made in <see cref="ShowPoint"/>
    /// </summary>
    private void DrawSelectedPointInspector() {
        int selectedAnchorIndex = spline.GetAnchorPointIndex(selectedIndex);
        bool isAnchor = spline.IsAnchorPoint(selectedIndex);
        string label = "Selected Point (" + (isAnchor ? "anchor" : "control") + "): " +
            selectedIndex.ToString() + "/" + (spline.ControlPointCount - 1).ToString();
        GUILayout.Label(label);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Select Previous Point")) { SelectPreviousPoint(); }
        if (GUILayout.Button("Select Next Point")) { SelectNextPoint(); }
        GUILayout.EndHorizontal();

        // Draw local position and allow editing.
        EditorGUI.BeginChangeCheck();
        Vector3 point = EditorGUILayout.Vector3Field("Position (Local)",
            spline.GetControlPoint(selectedIndex));
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(spline, "Move Point");
            EditorUtility.SetDirty(spline);
            spline.SetControlPoint(selectedIndex, point);
        }

        // Draw offset from anchor if the selected point is a control point.
        if (!isAnchor) {
            EditorGUI.BeginChangeCheck();
            Vector3 anchorPoint = spline.GetControlPoint(selectedAnchorIndex);
            Vector3 offsetPoint = EditorGUILayout.Vector3Field("Position (Offset from Anchor)",
                spline.GetControlPoint(selectedIndex) - anchorPoint);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(spline, "Move Point");
                EditorUtility.SetDirty(spline);
                spline.SetControlPoint(selectedIndex, anchorPoint + offsetPoint);
            }
        }

        EditorGUI.BeginChangeCheck();
        BezierControlPointMode mode = (BezierControlPointMode)
            EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(spline, "Change Point Mode");
            spline.SetControlPointMode(selectedIndex, mode);
            EditorUtility.SetDirty(spline);
        }
    }

    /// <summary>
    /// Selects the previous point, looping around if the spline is set to loop.
    /// </summary>
    private void SelectPreviousPoint() {
        selectedIndex -= 1;
        if (selectedIndex < 0) {
            if (spline.loop) {
                selectedIndex = spline.ControlPointCount - 1;
            } else {
                selectedIndex = 0;
            }
        }
        Repaint();
    }

    /// <summary>
    /// Selects the next point, looping around if the spline is set to loop.
    /// </summary>
    private void SelectNextPoint() {
        selectedIndex += 1;
        if (selectedIndex >= spline.ControlPointCount) {
            if (spline.loop) {
                selectedIndex = 0;
            } else {
                selectedIndex = spline.ControlPointCount - 1;
            }
        }
        Repaint();
    }
}
