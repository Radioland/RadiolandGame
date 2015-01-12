using UnityEngine;
using System.Collections;

public class TriggerEffects : MonoBehaviour
{
    // Variables to specify in the editor.
    [SerializeField] protected EffectManager effectManager;
    [Tooltip("Destroys this script after starting.")]
    [SerializeField] private bool onlyTriggerOnce = false;

    private void Reset() {
        effectManager = gameObject.GetComponent<EffectManager>();
    }

    protected virtual void Awake() {

    }

    private void Start() {

    }

    private void Update() {

    }

    protected void StartEvent() {
        if (!enabled) { return; }

        if (effectManager) {
            effectManager.StartEvent();
            if (onlyTriggerOnce) {
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
