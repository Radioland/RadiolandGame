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

    // This should be called by an attack starting, an attack hitting a target,
    // an environmental event, etc.
    public void StartEvent() {
        foreach (Effect effect in effects) {
            effect.TriggerEffect();
        }
    }

    // Prematurely end all events
    // Call when an ability is interrupted, etc.
    public void StopEvent() {        
        foreach (Effect effect in effects) {
            effect.EndEffect();
        }
    }
}
