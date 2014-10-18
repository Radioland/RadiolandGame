using UnityEngine;
using System.Collections;

public class StartTriggerEffects : TriggerEffects
{
    void Awake() {

    }

    void Start() {
        effectsManager.StartEvent();
    }

    void Update() {

    }
}
