using UnityEngine;
using UnityEditor;

public class RadiolandWindow : EditorWindow
{
    private Texture2D headerImage;
    private const float headerHeight = 100f;

    private const float defaultCurveGizmoSphereRadius = 0.8f;
    private const float defaultCurveGizmoBoxWidth= 0.9f;
    private float curveGizmoSphereRadius = PlayerPrefs.GetFloat("curveGizmoSphereRadius", defaultCurveGizmoSphereRadius);
    private float curveGizmoBoxWidth = PlayerPrefs.GetFloat("curveGizmoBoxWidth", 0.9f);
    private bool highlightAllRecursive = PlayerPrefs.GetInt("highlightAllRecursive", 0) == 1;
    private bool highlightEncapsulateChildren = PlayerPrefs.GetInt("highlightEncapsulateChildren", 0) == 1;

    public void OnEnable() {
        headerImage = Resources.Load("BirbBanner", typeof(Texture2D)) as Texture2D;
    }

    [MenuItem ("Window/Radioland")]
    private static void Init() {
        // Get existing open window or if none, make a new one.
        RadiolandWindow window = (RadiolandWindow)EditorWindow.GetWindow(typeof(RadiolandWindow));
        window.title = "Radioland";

        window.curveGizmoSphereRadius = PlayerPrefs.GetFloat("curveGizmoSphereRadius", window.curveGizmoSphereRadius);
        window.curveGizmoBoxWidth = PlayerPrefs.GetFloat("curveGizmoBoxWidth", window.curveGizmoBoxWidth);
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
        PlayerPrefs.SetFloat("curveGizmoSphereRadius", curveGizmoSphereRadius);
        EditorGUILayout.EndHorizontal();

        // Curves: Gizmo Box Width.
        EditorGUILayout.BeginHorizontal();
        curveGizmoBoxWidth = EditorGUILayout.Slider("Gizmo Box Width", curveGizmoBoxWidth, 0f, 2f);
        if (Mathf.Approximately(curveGizmoBoxWidth, defaultCurveGizmoBoxWidth)) { GUI.enabled = false; }
        if (GUILayout.Button("Reset", GUILayout.ExpandWidth(false))) { curveGizmoBoxWidth = defaultCurveGizmoBoxWidth; }
        GUI.enabled = true;
        PlayerPrefs.SetFloat("curveGizmoBoxWidth", curveGizmoBoxWidth);
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("Hierarchy Highlight Settings", EditorStyles.boldLabel);

        // Highlight: All Recursive.
        float labelWidthBackup = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 180;
        highlightAllRecursive = EditorGUILayout.Toggle("Highlight All Recursive", highlightAllRecursive, GUILayout.ExpandWidth(false));
        PlayerPrefs.SetInt("highlightAllRecursive", highlightAllRecursive ? 1 : 0);

        // Highlight: Encapsulate Children.
        highlightEncapsulateChildren = EditorGUILayout.Toggle("Highlight Encapsulate Children", highlightEncapsulateChildren);
        PlayerPrefs.SetInt("highlightEncapsulateChildren", highlightEncapsulateChildren ? 1 : 0);
        EditorGUIUtility.labelWidth = labelWidthBackup;
    }
}
