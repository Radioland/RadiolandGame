using UnityEngine;
using UnityEditor;

public class RadiolandWindow : EditorWindow
{
    private Texture2D headerImage;
    private const float headerHeight = 100f;

    private const float defaultCurveGizmoSphereRadius = 0.8f;
    private const float defaultCurveGizmoBoxWidth= 0.9f;
    private float curveGizmoSphereRadius = EditorPrefs.GetFloat("curveGizmoSphereRadius", defaultCurveGizmoSphereRadius);
    private float curveGizmoBoxWidth = EditorPrefs.GetFloat("curveGizmoBoxWidth", 0.9f);

    private bool highlightEnabled = EditorPrefs.GetBool("highlightEnabled", false);
    private bool highlightAllRecursive = EditorPrefs.GetBool("highlightAllRecursive", false);
    private bool highlightEncapsulateChildren = EditorPrefs.GetBool("highlightEncapsulateChildren", true);

    public void OnEnable() {
        headerImage = Resources.Load("BirbBanner", typeof(Texture2D)) as Texture2D;
    }

    [MenuItem ("Window/Radioland")]
    private static void Init() {
        // Get existing open window or if none, make a new one.
        RadiolandWindow window = (RadiolandWindow)EditorWindow.GetWindow(typeof(RadiolandWindow));
        window.title = "Radioland";

        window.curveGizmoSphereRadius = EditorPrefs.GetFloat("curveGizmoSphereRadius", window.curveGizmoSphereRadius);
        window.curveGizmoBoxWidth = EditorPrefs.GetFloat("curveGizmoBoxWidth", window.curveGizmoBoxWidth);
    }

    private void OnGUI() {
        // Hack to get current window width (this allows for centering).
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        Rect scale = GUILayoutUtility.GetLastRect();

        GUILayout.Space(headerHeight);
        GUI.DrawTexture(new Rect(0, 0, scale.width, headerHeight), headerImage, ScaleMode.ScaleToFit);

        GUILayout.Label("Curve Settings", EditorStyles.boldLabel);

        // Curves: Gizmo Sphere Radius.
        EditorGUILayout.BeginHorizontal();
        curveGizmoSphereRadius = EditorGUILayout.Slider("Gizmo Sphere Radius", curveGizmoSphereRadius, 0f, 1f);
        if (Mathf.Approximately(curveGizmoSphereRadius, defaultCurveGizmoSphereRadius)) { GUI.enabled = false; }
        if (GUILayout.Button("Reset", GUILayout.ExpandWidth(false))) { curveGizmoSphereRadius = defaultCurveGizmoSphereRadius; }
        GUI.enabled = true;
        EditorPrefs.SetFloat("curveGizmoSphereRadius", curveGizmoSphereRadius);
        EditorGUILayout.EndHorizontal();

        // Curves: Gizmo Box Width.
        EditorGUILayout.BeginHorizontal();
        curveGizmoBoxWidth = EditorGUILayout.Slider("Gizmo Box Width", curveGizmoBoxWidth, 0f, 2f);
        if (Mathf.Approximately(curveGizmoBoxWidth, defaultCurveGizmoBoxWidth)) { GUI.enabled = false; }
        if (GUILayout.Button("Reset", GUILayout.ExpandWidth(false))) { curveGizmoBoxWidth = defaultCurveGizmoBoxWidth; }
        GUI.enabled = true;
        EditorPrefs.SetFloat("curveGizmoBoxWidth", curveGizmoBoxWidth);
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("Hierarchy Highlight Settings", EditorStyles.boldLabel);

        float labelWidthBackup = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 190;

        // Highlight: Enable.
        highlightEnabled = EditorGUILayout.Toggle("Enable Highlight (Slows Editor)", highlightEnabled);
        EditorPrefs.SetBool("highlightEnabled", highlightEnabled);

        // Highlight: All Recursive.
        highlightAllRecursive = EditorGUILayout.Toggle("Highlight All Recursive", highlightAllRecursive);
        EditorPrefs.SetBool("highlightAllRecursive", highlightAllRecursive);

        // Highlight: Encapsulate Children.
        highlightEncapsulateChildren = EditorGUILayout.Toggle("Highlight Encapsulate Children", highlightEncapsulateChildren);
        EditorPrefs.SetBool("highlightEncapsulateChildren", highlightEncapsulateChildren);
        EditorGUIUtility.labelWidth = labelWidthBackup;
    }
}
