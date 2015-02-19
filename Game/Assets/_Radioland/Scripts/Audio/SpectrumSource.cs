﻿using UnityEngine;
using System.Collections;
using System.Linq;

public class SpectrumSource : MonoBehaviour
{
    private enum StationChoice {
        None,
        StrongestSignal,
        Station_1,
        Station_2,
        Station_3
    }

    public int spectrumSamples = 1024;
    [HideInInspector] public float[] spectrum;
    [HideInInspector] public float sampleRate;
    [HideInInspector] public float frequencyPerElement;
    [HideInInspector] public RadioStation radioStation;

    [Header("Automatic Source Setup")]
    [SerializeField] private StationChoice stationChoice = StationChoice.Station_1;
    [Header("Manual Source Setup")]
    public AudioSource source;
    public AudioStream stream;
    [Header("Configuration")]
    private RadioStation[] allStations;

    private void Awake() {
        spectrum = new float[spectrumSamples];

        GameObject player = GameObject.FindWithTag("Player");
        allStations = player.GetComponentsInChildren<RadioStation>();

        // Find the radioStation matching stationChoice.
        foreach (RadioStation station in allStations) {
            if ((station.id == 1 && stationChoice == StationChoice.Station_1) ||
                (station.id == 2 && stationChoice == StationChoice.Station_2) ||
                (station.id == 3 && stationChoice == StationChoice.Station_3)) {
                source = station.audioSource;
                stream = station.stream;
                radioStation = station;
            }
        }
    }

    private void Start() {

    }

    private void Update() {
        if (stationChoice == StationChoice.StrongestSignal) {
            float strongestSignal = allStations.Max(x => x.signalStrength);
            radioStation = allStations.First(x => Mathf.Approximately(x.signalStrength, strongestSignal));

            source = radioStation.audioSource;
            stream = radioStation.stream;
        }

        if (stream) {
            spectrum = stream.spectrum;
            spectrumSamples = stream.spectrum.Length;
            sampleRate = stream.sampleRate;
        } else if (source) {
            source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
            sampleRate = source.clip.frequency;
        } else if (stationChoice == StationChoice.None) {
            AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
            sampleRate = AudioSettings.outputSampleRate;
        }

        frequencyPerElement = sampleRate / 2f / spectrumSamples;
    }
}