using UnityEngine;
using UnityEditor;

public class RadiolandWindow : EditorWindow {
    private float curveGizmoBoxWidth = 0.9f;
    private float curveGizmoSphereRadius = 0.9f;
    private Color buttonColor = Color.red;

    public static RadiolandWindow window;

    // Add menu named "Radioland" to the Window menu
    [MenuItem ("Window/Radioland")]
    private static void Init() {
        // Get existing open window or if none, make a new one.
        window = (RadiolandWindow)EditorWindow.GetWindow(typeof(RadiolandWindow));
        window.title = "Radioland";
    }

    private void OnGUI() {
        GUILayout.Label ("Curve Settings", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Gizmo Box Width");
        curveGizmoBoxWidth = EditorGUILayout.Slider(curveGizmoBoxWidth, 0f, 2f);
        EditorGUILayout.EndHorizontal();

//        curveGizmoBoxWidth = EditorGUILayout.FloatField("Gizmo Box Width", curveGizmoBoxWidth);
        curveGizmoSphereRadius = EditorGUILayout.FloatField("Gizmo Sphere Radius", curveGizmoSphereRadius);
        PlayerPrefs.SetFloat("curveGizmoBoxWidth", curveGizmoBoxWidth);
        PlayerPrefs.SetFloat("curveGizmoSphereRadius", curveGizmoSphereRadius);

        buttonColor = EditorGUILayout.ColorField("Color", buttonColor);
        GUI.color = buttonColor;
        if (GUILayout.Button("testing" )) {

        }
    }
}
