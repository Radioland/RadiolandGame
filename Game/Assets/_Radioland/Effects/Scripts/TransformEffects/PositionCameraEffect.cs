using UnityEngine;
using System.Collections;

public class PositionCameraEffect : Effect
{
    // Variables to specify in the editor.
    public Transform cameraTransform;
    public Transform lookAtTarget;
    public Vector3 offsetPosition;

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

        if (cameraTransform) {
            cameraTransform.position = lookAtTarget.transform.position + offsetPosition;
            cameraTransform.LookAt(lookAtTarget);
        }
    }

    public override void EndEffect() {
        base.EndEffect();
    }
}
