using UnityEngine;
using System.Collections;

public class ChangeMaterialEffect : Effect
{
    public Renderer rendererToChange;
    public Material newMaterial;
    public bool revertAtEnd = false;

    private Material originalMaterial;

    protected override void Awake() {
        base.Awake();

        originalMaterial = rendererToChange.material;
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

        rendererToChange.material = newMaterial;
    }

    public override void EndEffect() {
        base.EndEffect();

        if (revertAtEnd) {
            rendererToChange.material = originalMaterial;
        }
    }
}
