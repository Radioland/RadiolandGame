// Thanks to http://answers.unity3d.com/questions/652362/rename-a-field-without-losing-inspector-values.html

using UnityEditor;
using UnityEngine;
using System.IO;

public class CopyFieldData : EditorWindow {
    private int sceneIndex; // scene to advance to next

    [MenuItem("Window/Copy Field Data")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(CopyFieldData));
    }

    private void OnGUI() {
        // If you only want to change scenes that are in your build, you can do this:
        //string[] paths = EditorBuildSettings.scenes.Select(scene => scene.path).ToArray();
        // In my case I have debugging scenes that are not in my build, so I do this instead:
        string[] paths = Directory.GetFiles("Assets/_Radioland/Scenes", "*.unity", SearchOption.AllDirectories);

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.margin = new RectOffset(10, 10, 10, 10);

        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.wordWrap = true;

        GUILayout.Label("Look at CopyFieldData.CopyDataInScene() to see/set what field is being copied on which component.", labelStyle);

        if (GUILayout.Button("Save and open " + paths[sceneIndex], buttonStyle)) {
            EditorApplication.SaveScene();
            EditorApplication.OpenScene(paths[sceneIndex]);
            sceneIndex++;
            if (sceneIndex >= paths.Length) {
                sceneIndex = 0;
            }
        }

        if (GUILayout.Button("Copy data in this scene", buttonStyle)) {
            CopyDataInScene();
        }

        if (GUILayout.Button("Copy data in ALL scenes", buttonStyle)) {
            if (EditorApplication.SaveCurrentSceneIfUserWantsTo()) {
                string originalScene = EditorApplication.currentScene;

                foreach (string scene in paths) {
                    EditorApplication.OpenScene(scene);
                    EditorUtility.FocusProjectWindow();
                    CopyDataInScene();
                    EditorApplication.SaveScene();
                }

                EditorApplication.OpenScene(originalScene);
            }
        }

        GUILayout.Label("Scenes Found:\n" + string.Join("\n", paths), labelStyle);
    }

    private void CopyDataInScene() {
        Debug.Log("Copying data in " + EditorApplication.currentScene);

        // Find and replace "Bus" to change the component being worked on
        Effect[] components = Resources.FindObjectsOfTypeAll<Effect>();
        foreach (Effect component in components) {
            // Edit this line to change which field is copied into which
            //component.Transfer();

            // The following line is necessary to get changes to instances of
            // prefabs to stick. If it gives you "unsupported type" errors, see:
            // http://answers.unity3d.com/questions/533541/unsupported-type-error-in-custom-editor-script.html
            PrefabUtility.RecordPrefabInstancePropertyModifications(component);
        }
    }
}
