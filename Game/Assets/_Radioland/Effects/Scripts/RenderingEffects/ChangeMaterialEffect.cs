using UnityEngine;
using System.Collections;

public class ChangeMaterialEffect : Effect
{
    // Variables to specify in the editor.
    [SerializeField] private Renderer rendererToChange;
    [SerializeField] private int materialIndex = 0;
    [SerializeField] private Material newMaterial;
    [SerializeField] private bool revertAtEnd = false;

    private Material originalMaterial;

    protected override void Awake() {
        base.Awake();

        originalMaterial = rendererToChange.materials[materialIndex];
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

        Material[] materials = rendererToChange.materials;
        materials[materialIndex] = newMaterial;
        rendererToChange.materials = materials;
    }

    public override void EndEffect() {
        base.EndEffect();

        if (revertAtEnd) {
            Material[] materials = rendererToChange.materials;
            materials[materialIndex] = originalMaterial;
            rendererToChange.materials = materials;
        }
    }
}
