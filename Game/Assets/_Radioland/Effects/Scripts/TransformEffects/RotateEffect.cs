using UnityEngine;
using System.Collections;

public class RotateEffect : Effect
{
    [SerializeField] private Vector3 finalRotationEulerAngles;

    private Quaternion originalRotation;
    private Quaternion finalRotation;

    private void Reset() {
        timing = new EffectTiming();
        timing.timed = true;
    }

    protected override void Awake() {
        base.Awake();
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();

        if (hasStarted && isPlaying) {
            transform.rotation = Quaternion.Lerp(originalRotation, finalRotation,
                                                 percentDurationElapsed);
        }
    }

    public override void TriggerEffect() {
        base.TriggerEffect();
    }

    public override void StartEffect() {
        base.StartEffect();

        originalRotation = transform.rotation;
        finalRotation = Quaternion.Euler(finalRotationEulerAngles);
    }

    public override void EndEffect() {
        base.EndEffect();
    }
}
