using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(SpectrumSource))]
public class SpectrumFrequencyTrigger : MonoBehaviour
{
    [SerializeField] private float minTriggerFrequency = 0f;
    [SerializeField] private float maxTriggerFrequency = 512f;
    [SerializeField] private float minWatchFrequency = 0f;
    [SerializeField] private float maxWatchFrequency = 2048f;
    [SerializeField] private float thresholdPercent = 0.5f;
    [SerializeField] private float cooldown = 0f;
    [SerializeField] private UnityEvent triggerEvent;

    private SpectrumSource spectrumSource;
    private int spectrumSamples;
    private float[] spectrum;

    // Private class-scope instead of local-scope to view in debug mode within Unity.
    private int startTriggerIndex;
    private int stopTriggerIndex;
    private int startWatchIndex;
    private int stopWatchIndex;

    private float lastTriggeredTime;

    private void Awake() {
        spectrumSource = gameObject.GetComponent<SpectrumSource>();
        spectrumSamples = spectrumSource.spectrumSamples;
        spectrum = new float[spectrumSamples];
    }

    private void Start() {

    }

    private int GetSpectrumIndex(float frequency) {
        return Mathf.Min(Mathf.FloorToInt(frequency / spectrumSource.frequencyPerElement), spectrumSamples);
    }

    private void Update() {
        if (Time.time - lastTriggeredTime < cooldown) { return; }

        spectrum = spectrumSource.spectrum;

        startTriggerIndex = GetSpectrumIndex(minTriggerFrequency);
        stopTriggerIndex = GetSpectrumIndex(maxTriggerFrequency);
        startWatchIndex = GetSpectrumIndex(minWatchFrequency);
        stopWatchIndex = GetSpectrumIndex(maxWatchFrequency);

        float totalTriggerAmplitude = 0f;
        for (int i = startTriggerIndex; i < stopTriggerIndex; i++) {
            totalTriggerAmplitude += spectrum[i];
        }

        float totalWatchAmplitude = 0f;
        for (int i = startWatchIndex; i < stopWatchIndex; i++) {
            totalWatchAmplitude += spectrum[i];
        }

        float relativeTriggerAmplitude = totalTriggerAmplitude / totalWatchAmplitude;

        if (spectrumSource.stream) { relativeTriggerAmplitude *= spectrumSource.stream.volume; }
        else if (spectrumSource.source) { relativeTriggerAmplitude *= spectrumSource.source.volume; }
        if (spectrumSource.radioStation) { relativeTriggerAmplitude /= spectrumSource.radioStation.maxVolume; }

        if (relativeTriggerAmplitude > thresholdPercent) {
            lastTriggeredTime = Time.time;
            triggerEvent.Invoke();
        }
    }
}
