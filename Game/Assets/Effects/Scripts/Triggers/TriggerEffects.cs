using UnityEngine;
using System.Collections;

public class TriggerEffects : MonoBehaviour
{
    public EffectManager effectsManager;
    public bool destroyAfterTrigger = true;

    void Awake() {

    }

    void Start() {

    }

    void Update() {

    }

    protected void StartEvent() {
        effectsManager.StartEvent();
        if (destroyAfterTrigger) {
            Destroy(this);
        }
    }

    protected void StopEvent() {
        effectsManager.StopEvent();
    }
}
