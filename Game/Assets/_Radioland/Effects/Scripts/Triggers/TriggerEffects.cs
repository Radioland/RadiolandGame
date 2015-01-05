using UnityEngine;
using System.Collections;

public class TriggerEffects : MonoBehaviour
{
    // Variables to specify in the editor.
    [SerializeField] protected EffectManager effectManager;
    [Tooltip("Destroys this script, not its GameObject.")]
    [SerializeField] private bool destroyAfterTrigger = false;
    [Tooltip("Searches this GameObject for the EffectManager.")]
    [SerializeField] private bool defaultToSelfManager = true;

    protected virtual void Awake() {
        if (defaultToSelfManager && !effectManager) {
            effectManager = gameObject.GetComponent<EffectManager>();
            if (!effectManager) {
                Debug.LogWarning("Could not find EffectManager on" + this.GetPath());
            }
        }
    }

    private void Start() {

    }

    private void Update() {

    }

    protected void StartEvent() {
        if (!enabled) { return; }

        if (effectManager) {
            effectManager.StartEvent();
            if (destroyAfterTrigger) {
                Destroy(this);
            }
        }
    }

    protected void StopEvent() {
        if (effectManager) {
            effectManager.StopEvent();
        }
    }
}
