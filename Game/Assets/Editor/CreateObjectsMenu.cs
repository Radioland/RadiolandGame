using UnityEngine;
using UnityEditor;
using System.Collections;

[ExecuteInEditMode] public static class CreateObjectsMenu
{
    private static void CreateObject(string objectName, string objectPath,
                                     bool placeAbove=false, bool orientToSurface=false,
                                     bool matchScale=false) {
        GameObject theObject = GameObject.Instantiate(
                AssetDatabase.LoadAssetAtPath(objectPath, typeof(GameObject))) as GameObject;
        if (!theObject) { return; }

        theObject.name = objectName;

        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject) {
            if (GameObject.Find(selectedObject.name)) {
                theObject.transform.SetParent(selectedObject.transform);
            }
        }

        theObject.transform.localPosition = Vector3.zero;
        if (placeAbove && selectedObject.collider) {
            float height = selectedObject.collider.bounds.size.y;

            RaycastHit[] hits;
            hits = Physics.RaycastAll(selectedObject.transform.position + Vector3.up * height,
                                      -Vector3.up, height * 1.2f);
            foreach (RaycastHit hit in hits) {
                if (hit.collider == selectedObject.collider) {
                    Vector3 localHit = selectedObject.transform.InverseTransformPoint(hit.point);
                    theObject.transform.localPosition = localHit;

                    if (orientToSurface) {
                        theObject.transform.localRotation = Quaternion.Euler(hit.normal);
                    }

                    break;
                }
            }
        }
        if (matchScale) { theObject.transform.localScale = new Vector3(1, 1, 1); }

        Selection.activeObject = theObject;
        Undo.RegisterCreatedObjectUndo(theObject, "Create " + objectName);
    }

    [MenuItem("GameObject/Create Radioland/Critters/Talking Resistor", false, 1)]
    [MenuItem("Radioland/Critters/Talking Resistor", false, 1)]
    private static void CreatePanel() {
        CreateObject("Talking Resistor", "Assets/_Radioland/Prefabs/Critters/Talking Resistor.prefab",
                     placeAbove:true, orientToSurface:true);
    }

    [MenuItem("GameObject/Create Radioland/Props/Leaves", false, 1)]
    [MenuItem("Radioland/Props/Leaves", false, 1)]
    private static void CreateLeaves() {
        CreateObject("Leaves", "Assets/_Radioland/Environment/Prefabs/Components/props/leaves.prefab",
                     placeAbove:true, orientToSurface:true);
    }

    [MenuItem("GameObject/Create Radioland/Environment/Wire Creator", false, 1)]
    [MenuItem("Radioland/Environment/Wire Creator", false, 1)]
    private static void CreateWireCreator() {
        CreateObject("Wire Creator", "Assets/_Radioland/Environment/Prefabs/Components/wires/Wire Creator.prefab",
                     placeAbove:true, orientToSurface:false);
    }

    [MenuItem("GameObject/Create Radioland/Environment/Radio Tower", false, 1)]
    [MenuItem("Radioland/Environment/Radio Tower", false, 1)]
    private static void CreateRadioTower() {
        CreateObject("Radio Tower", "Assets/_Radioland/Environment/Prefabs/radioTower.prefab",
                     placeAbove:true, orientToSurface:true);
    }

    [MenuItem("GameObject/Create Radioland/Environment/Transport", false, 1)]
    [MenuItem("Radioland/Environment/Transport", false, 1)]
    private static void CreateTransport() {
        CreateObject("Transport", "Assets/_Radioland/Environment/Prefabs/Transport.prefab",
                     placeAbove:true);
    }

    [MenuItem("GameObject/Create Radioland/Environment/Elevator", false, 1)]
    [MenuItem("Radioland/Environment/Elevator", false, 1)]
    private static void CreateElevator() {
        CreateObject("Elevator", "Assets/_Radioland/Environment/Prefabs/Elevator.prefab",
                     placeAbove:true);
    }

    [MenuItem("GameObject/Create Radioland/Props/Visualizer Grass", false, 1)]
    [MenuItem("Radioland/Props/Visualizer Grass", false, 1)]
    private static void CreateVisualizerGrass() {
        CreateObject("Visualizer Grass", "Assets/_Radioland/Environment/Prefabs/Components/props/Visualizer Grass.prefab",
                     placeAbove:true, orientToSurface:true);
    }

    [MenuItem("GameObject/Create Radioland/General/Checkpoint", false, 1)]
    [MenuItem("Radioland/General/Checkpoint", false, 1)]
    private static void CreateCheckpoint() {
        CreateObject("Checkpoint", "Assets/_Radioland/Prefabs/LevelGeneral/Checkpoint.prefab",
                     placeAbove:true, orientToSurface:false);
    }

    [MenuItem("GameObject/Create Radioland/General/Small Killzone", false, 1)]
    [MenuItem("Radioland/General/Small Killzone", false, 1)]
    private static void CreateSmallKillzone() {
        CreateObject("Small Killzone", "Assets/_Radioland/Prefabs/LevelGeneral/Small Killzone.prefab",
                     placeAbove:true, orientToSurface:false);
    }
}
