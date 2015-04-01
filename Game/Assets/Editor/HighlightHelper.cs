// Source: https://gist.github.com/toxicFork/e45ff9f0b061d7aeee23#file-highlighthelper-cs

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class HighlightHelper
{
    static HighlightHelper() {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGui;
        EditorApplication.update += OnEditorUpdate;

        SceneView.onSceneGUIDelegate += OnSceneGuiDelegate;
    }

    private static readonly Color hoverColor = new Color(1, 1, 1, 0.75f);
    private static readonly Color dragColor = new Color(1f, 0, 0, 0.75f);
    private static readonly Color faceColor = new Color(0.5f, 0.5f, 0.5f, 0.1f);
    private static readonly Color outlineColor = new Color(0f, 0f, 0.5f, 0.3f);
    private const float hoverPadding = 1.1f;

    private static void OnSceneGuiDelegate(SceneView sceneView) {
        bool highlightEnabled = EditorPrefs.GetBool("highlightEnabled", false);
        if (!highlightEnabled) { return; }

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

            Handles.color = dragColor;
            foreach (Object objectReference in DragAndDrop.objectReferences) {
                GameObject gameObject = objectReference as GameObject;

                if (gameObject && gameObject.activeInHierarchy) {
                    DrawObjectBounds(gameObject);

                    drawnInstanceIDs.Add(gameObject.GetInstanceID());
                }
            }

            Handles.color = hoverColor;
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
        bool highlightAllRecursive = EditorPrefs.GetBool("highlightAllRecursive", false);
        bool highlightEncapsulateChildren = EditorPrefs.GetBool("highlightEncapsulateChildren", true);

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
                if (!(renderer is ParticleSystemRenderer) && !(renderer is TrailRenderer)) {
                    Bounds rendererBounds = renderer.bounds;
                    rendererBounds.center = objectTransform.position;
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            float width = bounds.size.x / 2;
            float height = bounds.size.y / 2;
            float depth = bounds.size.z / 2;

            // Front
            Vector3[] verts = {
                bounds.center + new Vector3(width, height, depth),
                bounds.center + new Vector3(width, -height, depth),
                bounds.center + new Vector3(-width, -height, depth),
                bounds.center + new Vector3(-width, height, depth)
            };
            Handles.DrawSolidRectangleWithOutline(verts, faceColor, outlineColor);

            // Back
            verts = new[]{
                bounds.center + new Vector3(width, height, -depth),
                bounds.center + new Vector3(width, -height, -depth),
                bounds.center + new Vector3(-width, -height, -depth),
                bounds.center + new Vector3(-width, height, -depth)
            };
            Handles.DrawSolidRectangleWithOutline(verts, faceColor, outlineColor);

            // Right
            verts = new[]{
                bounds.center + new Vector3(width, height, -depth),
                bounds.center + new Vector3(width, -height, -depth),
                bounds.center + new Vector3(width, -height, depth),
                bounds.center + new Vector3(width, height, depth)
            };
            Handles.DrawSolidRectangleWithOutline(verts, faceColor, outlineColor);

            // Left
            verts = new[]{
                bounds.center + new Vector3(-width, height, -depth),
                bounds.center + new Vector3(-width, -height, -depth),
                bounds.center + new Vector3(-width, -height, depth),
                bounds.center + new Vector3(-width, height, depth)
            };
            Handles.DrawSolidRectangleWithOutline(verts, faceColor, outlineColor);

            // Top
            verts = new[]{
                bounds.center + new Vector3(-width, height, depth),
                bounds.center + new Vector3(-width, height, -depth),
                bounds.center + new Vector3(width, height, -depth),
                bounds.center + new Vector3(width, height, depth)
            };
            Handles.DrawSolidRectangleWithOutline(verts, faceColor, outlineColor);

            // Bottom
            verts = new[]{
                bounds.center + new Vector3(-width, -height, depth),
                bounds.center + new Vector3(-width, -height, -depth),
                bounds.center + new Vector3(width, -height, -depth),
                bounds.center + new Vector3(width, -height, depth)
            };
            Handles.DrawSolidRectangleWithOutline(verts, faceColor, outlineColor);
        }
    }

    private static int hoveredInstance = 0;
    private static double lastRepaint = -1000f;
    private const float repaintCooldown = 1 / 6f;

    private static void OnEditorUpdate() {
        bool highlightEnabled = EditorPrefs.GetBool("highlightEnabled", false);
        if (!highlightEnabled) { return; }

        EditorWindow mouseOverWindow = EditorWindow.mouseOverWindow;
        if (!mouseOverWindow || !mouseOverWindow.ToString().Contains("SceneHierarchyWindow")) {
            RepaintIfShowing(); return;
        }

        if (EditorApplication.timeSinceStartup - lastRepaint > repaintCooldown) { Repaint(); }
    }

    private static void Repaint() {
        EditorApplication.RepaintHierarchyWindow();
        lastRepaint = EditorApplication.timeSinceStartup;
    }

    private static void RepaintIfShowing() {
        if (hoveredInstance != 0) { Repaint(); }
    }

    private static void HierarchyWindowItemOnGui(int instanceId, Rect selectionRect) {
        bool highlightEnabled = EditorPrefs.GetBool("highlightEnabled", false);
        if (!highlightEnabled) { return; }

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
    }
}
