using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectManager : MonoBehaviour
{
    private Effect[] effects;

    void Awake() {
        effects = gameObject.GetComponents<Effect>();
    }

    void Start() {

    }

    void Update() {

    }

    // Triggers all effects on this object.
    public void StartEvent() {
        foreach (Effect effect in effects) {
            effect.TriggerEffect();
        }
    }

    // Prematurely end all effects (ahead of their own durations).
    // Useful for interrupting abilities, when an object dies, etc.
    public void StopEvent() {        
        foreach (Effect effect in effects) {
            effect.EndEffect();
        }
    }
}
