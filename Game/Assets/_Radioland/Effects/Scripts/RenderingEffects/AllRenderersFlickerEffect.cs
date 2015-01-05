using UnityEngine;
using System.Collections;

public class AllRenderersFlickerEffect : Effect
{
    // Variables to specify in the editor.
    [SerializeField] private GameObject gameObjectToChange;
    [SerializeField] private float timeOn;
    [SerializeField] private float timeOff;

    private Renderer[] renderers;
    private bool currentlyVisible;
    private float lastTimeChanged;

    protected override void Awake() {
        base.Awake();

        currentlyVisible = true;
        renderers = gameObjectToChange.GetComponentsInChildren<Renderer>();
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();

        if (isPlaying && hasStarted) {
            if (currentlyVisible && Time.time - lastTimeChanged > timeOn) {
                ChangeVisibility(false);
            }
            if (!currentlyVisible && Time.time - lastTimeChanged > timeOff) {
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

    private void ChangeVisibility(bool newVisibility) {
        currentlyVisible = newVisibility;
        lastTimeChanged = Time.time;

        foreach (Renderer thisRenderer in renderers) {
            thisRenderer.enabled = currentlyVisible;
        }
    }
}
