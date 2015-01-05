using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CreateTube))]
public class CreateTubeInspector : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        CreateTube createTube = target as CreateTube;

        if (GUILayout.Button("Create New Tube")) {
            GameObject tube = createTube.CreateNew();
            if (tube) {
                Undo.RegisterCreatedObjectUndo(tube, "Create New Tube");
            }
        }
    }
}
