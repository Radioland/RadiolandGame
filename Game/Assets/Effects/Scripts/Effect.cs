using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EffectManager))]
public class Effect : MonoBehaviour
{
    public float startDelay = 0.0f;
    public float duration = 1.0f;

    [HideInInspector] public bool isPlaying;
    [HideInInspector] public bool hasStarted;
    
    // Protected can be modified by subclasses, private can't
    protected float lastTimeTriggered;
    protected float lastTimeStarted;
    protected float percentDurationElapsed;

    protected virtual void Awake() {
        isPlaying = false;
        hasStarted = false;
        lastTimeTriggered = -1000;
    }

    protected virtual void Start() {

    }

    protected virtual void Update() {
        percentDurationElapsed = (Time.time - lastTimeStarted) / duration;

        if (isPlaying && !hasStarted && (Time.time - lastTimeTriggered > startDelay)) {
            StartEffect();
        }

        if (isPlaying && hasStarted && (Time.time - lastTimeTriggered > duration + startDelay)) {
            EndEffect();
        }
    }

    // Triggers that an event has occured, starts counting to the startDelay
    public virtual void TriggerEffect() {
        if (!enabled) { return; }

        lastTimeTriggered = Time.time;
        isPlaying = true;
        hasStarted = false;

        // If there is no delay, start immediately - without waiting for Update
        if (startDelay < 0.001) {
            StartEffect();
        }
    }

    // Actually start the effect, after any startDelay time
    public virtual void StartEffect() {
        lastTimeStarted = Time.time;
        hasStarted = true;
    }

    public virtual void EndEffect() {
        isPlaying = false;
        hasStarted = false;
    }
}
