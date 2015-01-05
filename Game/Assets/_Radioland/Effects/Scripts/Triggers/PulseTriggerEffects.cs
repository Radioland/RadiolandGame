using UnityEngine;
using System.Collections;

public class PulseTriggerEffects : TriggerEffects
{
    // Variables to specify in the editor.
    [SerializeField] private float minStartupTime = 0.0f;
    [SerializeField] private float maxStartupTime = 0.0f;
    [SerializeField] private float minPulseTime = 2.0f;
    [SerializeField] private float maxPulseTime = 2.0f;

    private float lastStartedTime;
    private float currentPulseTime;

    protected override void Awake() {
        base.Awake();

        lastStartedTime = Time.time + Random.Range(minStartupTime, maxStartupTime);
    }

    private void Start() {

    }

    private void SetPulseTime() {
        currentPulseTime = Random.Range(minPulseTime, maxPulseTime);
    }

    private void Update() {
        if (Time.time - lastStartedTime > currentPulseTime) {
            StartEvent();
            SetPulseTime();
            lastStartedTime = Time.time;
        }
    }
}
