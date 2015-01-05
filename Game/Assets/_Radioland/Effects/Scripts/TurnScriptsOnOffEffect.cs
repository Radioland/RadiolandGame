using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnScriptsOnOffEffect : Effect
{
    // Variables to specify in the editor.
    public List<MonoBehaviour> scripts;
    [SerializeField] private bool startEnabled = true;
    [SerializeField] private bool endEnabled = true;

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

        foreach (MonoBehaviour script in scripts) {
            script.enabled = startEnabled;
        }
    }

    public override void EndEffect() {
        base.EndEffect();

        foreach (MonoBehaviour script in scripts) {
            script.enabled = endEnabled;
        }
    }
}
