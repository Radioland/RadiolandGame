using UnityEngine;
using System.Collections;

public class TriggerEffects : MonoBehaviour
{
    // Variables to specify in the editor.
    [SerializeField] protected EffectManager effectsManager;
    [Tooltip("Destroys this script, not its GameObject.")]
    [SerializeField] private bool destroyAfterTrigger = false;

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
