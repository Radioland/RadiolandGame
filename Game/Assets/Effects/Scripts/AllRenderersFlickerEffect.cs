using UnityEngine;
using System.Collections;

public class AllRenderersFlickerEffect : Effect
{
    public GameObject gameObjectToChange;
    public float timeOn;
    public float timeOff;

    private Renderer[] renderers;
    private bool isOn;
    private float lastTimeChanged;

    protected override void Awake() {
        base.Awake();

        renderers = gameObjectToChange.GetComponentsInChildren<Renderer>();
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();

        if (isPlaying && hasStarted) {
            if (isOn && Time.time - lastTimeChanged > timeOn) {
                ChangeVisibility(false);
            }
            if (!isOn && Time.time - lastTimeChanged > timeOff) {
                ChangeVisibility(true);
            }
        }
    }

    public override void TriggerEffect() {
        base.TriggerEffect();
    }

    public override void StartEffect() {
        base.StartEffect();

        ChangeVisibility(false);
    }

    public override void EndEffect() {
        base.EndEffect();

        ChangeVisibility(true);
    }

    void ChangeVisibility(bool newVisibility) {
        isOn = newVisibility;
        lastTimeChanged = Time.time;

        foreach (Renderer thisRenderer in renderers) {
            thisRenderer.enabled = isOn;
        }
    }
}
