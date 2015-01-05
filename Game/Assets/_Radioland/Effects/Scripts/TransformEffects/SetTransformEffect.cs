using UnityEngine;
using System.Collections;

public class SetTransformEffect : Effect
{
    // Variables to specify in the editor.
    public Transform objectTransform;
    public Vector3 position;
    public Vector3 rotationEulerAngles;
    public Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);

    protected override void Awake() {
        base.Awake();
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

        if (objectTransform) {
            objectTransform.position = position;
            objectTransform.rotation = Quaternion.Euler(rotationEulerAngles);
            objectTransform.localScale = scale;
        }
    }

    public override void EndEffect() {
        base.EndEffect();
    }
}
