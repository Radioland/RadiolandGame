using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EffectManager))]
public class Effect : MonoBehaviour
{
    // Variables to specify in the editor.
    [SerializeField] private float startDelay = 0.0f;
    [SerializeField] protected float duration = 1.0f;
    [SerializeField] private float cooldown = 0.0f;

    [HideInInspector] public bool isPlaying;
    [HideInInspector] public bool hasStarted;

    protected float lastTimeTriggered;
    protected float lastTimeStarted;
    protected float lastTimeEnded;

    protected float percentDurationElapsed {
        get { return (Time.time - lastTimeStarted) / duration; }
    }

    protected virtual void Awake() {
        isPlaying = false;
        hasStarted = false;
        lastTimeTriggered = -1000.0f;
        lastTimeStarted = -1000.0f;
        lastTimeEnded = -1000.0f;
    }

    protected virtual void Start() {

    }

    protected virtual void Update() {
        if (isPlaying && !hasStarted && (Time.time - lastTimeTriggered > startDelay)) {
            StartEffect();
        }

        if (isPlaying && hasStarted && (Time.time - lastTimeStarted > duration)) {
            EndEffect();
        }
    }

    // Triggers that an event has occured, starts counting to the startDelay.
    public virtual void TriggerEffect() {
        if (!enabled) { return; }

        lastTimeTriggered = Time.time;

        if (Time.time - lastTimeEnded < cooldown) { return; }

        isPlaying = true;
        hasStarted = false;

        // If there is no delay, start immediately - without waiting for Update.
        if (startDelay < 0.001) {
            StartEffect();
        }
    }

    // Actually start the effect, after any startDelay time.
    public virtual void StartEffect() {
        lastTimeStarted = Time.time;
        hasStarted = true;
    }

    public virtual void EndEffect() {
        isPlaying = false;
        hasStarted = false;
        lastTimeEnded = Time.time;
    }
}
