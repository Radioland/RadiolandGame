using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectManager : MonoBehaviour
{
    [SerializeField] private bool onlyExecuteOnce = false;
    private Effect[] effects;

    private bool executed;

    private void Awake() {
        effects = gameObject.GetComponents<Effect>();

        executed = false;
    }

    private void Start() {

    }

    private void Update() {

    }

    // Triggers all effects on this object.
    public void StartEvent() {
        if (!enabled || (onlyExecuteOnce && executed)) { return; }

        foreach (Effect effect in effects) {
            effect.TriggerEffect();
        }

        executed = true;
    }

    // Prematurely end all effects (ahead of their own durations).
    // Useful for interrupting abilities, when an object dies, etc.
    public void StopEvent() {
        foreach (Effect effect in effects) {
            effect.EndEffect();
        }
    }
}
