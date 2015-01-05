using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToggleLayerEffect : Effect
{
    // Variables to specify in the editor.
    [SerializeField] private GameObject objectToToggle;
    [SerializeField] private int newLayer;
    [SerializeField] private bool changeChildren = true;
    [SerializeField] private bool revertAtEnd = true;

    private List<GameObject> objectsToToggle;
    private List<int> originalLayers;

    protected override void Awake() {
        base.Awake();

        objectsToToggle = new List<GameObject>();
        originalLayers = new List<int>();

        objectsToToggle.Add(objectToToggle);
        originalLayers.Add(objectToToggle.layer);

        if (changeChildren) {
            Transform [] childTransforms = objectToToggle.GetComponentsInChildren<Transform>();
            foreach (Transform childTransform in childTransforms) {
                GameObject childObject = childTransform.gameObject;
                objectsToToggle.Add(childObject);
                originalLayers.Add(childObject.layer);
            }
        }
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    public override void TriggerEffect() {
        base.TriggerEffect();
    }

    public override void StartEffect() {
        base.StartEffect();

        foreach (GameObject toggleObject in objectsToToggle) {
            toggleObject.layer = newLayer;
        }
    }

    public override void EndEffect() {
        base.EndEffect();

        if (revertAtEnd) {
            for (int i = 0; i < objectsToToggle.Count; i++) {
                GameObject toggleObject = objectsToToggle[i];
                if (toggleObject) {
                    toggleObject.layer = originalLayers[i];
                }
            }
        }
    }
}
