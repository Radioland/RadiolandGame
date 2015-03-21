// Source: https://gist.github.com/toxicFork/e45ff9f0b061d7aeee23#file-highlighthelper-cs

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class HighlightHelper
{
    static HighlightHelper() {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGui;

        SceneView.onSceneGUIDelegate += OnSceneGuiDelegate;
    }

    private static readonly Color HoverColor = new Color(1, 1, 1, 0.75f);
    private static readonly Color DragColor = new Color(1f, 0, 0, 0.75f);
    private const float hoverPadding = 1.1f;

    private static void OnSceneGuiDelegate(SceneView sceneView) {
        switch (Event.current.type) {
            case EventType.DragUpdated:
            case EventType.DragPerform:
            case EventType.DragExited:
                sceneView.Repaint();
                break;
        }

        if (Event.current.type == EventType.repaint) {
            HashSet<int> drawnInstanceIDs = new HashSet<int>();

            Color handleColor = Handles.color;

            Handles.color = DragColor;
            foreach (Object objectReference in DragAndDrop.objectReferences) {
                GameObject gameObject = objectReference as GameObject;

                if (gameObject && gameObject.activeInHierarchy) {
                    DrawObjectBounds(gameObject);

                    drawnInstanceIDs.Add(gameObject.GetInstanceID());
                }
            }

            Handles.color = HoverColor;
            if (hoveredInstance != 0 && !drawnInstanceIDs.Contains(hoveredInstance)) {
                GameObject sceneGameObject = EditorUtility.InstanceIDToObject(hoveredInstance) as GameObject;

                if (sceneGameObject) {
                    DrawObjectBounds(sceneGameObject);
                }
            }

            Handles.color = handleColor;
        }
    }

    private static void DrawObjectBounds(GameObject sceneGameObject) {
        bool highlightAllRecursive = PlayerPrefs.GetInt("highlightAllRecursive", 0) == 1;
        bool highlightEncapsulateChildren = PlayerPrefs.GetInt("highlightEncapsulateChildren", 0) == 1;

        Transform[] objectTransforms = highlightAllRecursive ?
                sceneGameObject.GetComponentsInChildren<Transform>() :
                new[]{sceneGameObject.transform};

        foreach (Transform objectTransform in objectTransforms) {
            Bounds bounds = objectTransform == sceneGameObject ?
                new Bounds(objectTransform.position, Vector3.one) :
                new Bounds(objectTransform.position, Vector3.zero);

            Renderer[] renderers = highlightEncapsulateChildren ?
                objectTransform.GetComponentsInChildren<Renderer>() :
                objectTransform.GetComponents<Renderer>();

            foreach (Renderer renderer in renderers) {
                if (!(renderer is ParticleSystemRenderer)) {
                    Bounds rendererBounds = renderer.bounds;
                    rendererBounds.center = objectTransform.position;
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            float size = bounds.size.magnitude / 4f * hoverPadding;
            Handles.RectangleCap(0, bounds.center - new Vector3(size, 0, 0), Quaternion.Euler(0, 90, 0), size);
            Handles.RectangleCap(0, bounds.center + new Vector3(size, 0, 0), Quaternion.Euler(0, 90, 0), size);
            Handles.RectangleCap(0, bounds.center - new Vector3(0, size, 0), Quaternion.Euler(90, 0, 0), size);
            Handles.RectangleCap(0, bounds.center + new Vector3(0, size, 0), Quaternion.Euler(90, 0, 0), size);
            Handles.RectangleCap(0, bounds.center - new Vector3(0, 0, size), Quaternion.Euler(0, 0, 90), size);
            Handles.RectangleCap(0, bounds.center + new Vector3(0, 0, size), Quaternion.Euler(0, 0, 90), size);
        }
    }

    private static int hoveredInstance = 0;

    private static void HierarchyWindowItemOnGui(int instanceId, Rect selectionRect) {
        Event current = Event.current;

        switch (current.type) {
            case EventType.repaint:
                if (selectionRect.Contains(current.mousePosition)) {
                    if (hoveredInstance != instanceId) {
                        hoveredInstance = instanceId;
                        if (SceneView.lastActiveSceneView) {
                            SceneView.lastActiveSceneView.Repaint();
                        }
                    }
                } else {
                    if (hoveredInstance == instanceId) {
                        hoveredInstance = 0;
                        if (SceneView.lastActiveSceneView) {
                            SceneView.lastActiveSceneView.Repaint();
                        }
                    }
                }
                break;
            case EventType.MouseDrag:
            case EventType.DragUpdated:
            case EventType.DragPerform:
            case EventType.DragExited:
                if (SceneView.lastActiveSceneView) {
                    SceneView.lastActiveSceneView.Repaint();
                }
                break;
        }

        EditorApplication.RepaintHierarchyWindow();
    }
}
