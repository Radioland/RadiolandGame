using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EffectManager))]
public class Effect : MonoBehaviour
{
    [SerializeField] protected EffectTiming timing = new EffectTiming();

    [HideInInspector] public bool isPlaying;
    [HideInInspector] public bool hasStarted;

    protected float lastTimeTriggered;
    protected float lastTimeStarted;
    protected float lastTimeEnded;

    protected float percentDurationElapsed {
        get { return timing.timed ? ((Time.time - lastTimeStarted) / timing.duration) : 0f; }
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
        if (isPlaying && !hasStarted && (Time.time - lastTimeTriggered > timing.startDelay)) {
            StartEffect();
        }

        if (timing.timed && isPlaying && hasStarted &&
            (Time.time - lastTimeStarted > timing.duration)) {
            EndEffect();
        }
    }

    // Triggers that an event has occured, starts counting to the startDelay.
    public virtual void TriggerEffect() {
        if (!enabled) { return; }

        lastTimeTriggered = Time.time;

        if (Time.time - lastTimeStarted < timing.cooldown ||
            Time.time - lastTimeEnded < timing.cooldown) {
            return;
        }

        if (timing.random && Random.value > timing.playChance) { return; }

        isPlaying = true;
        hasStarted = false;

        // If there is no delay, start immediately - without waiting for Update.
        if (timing.startDelay < 0.001) {
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
