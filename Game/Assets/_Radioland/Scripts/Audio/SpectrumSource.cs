using UnityEngine;
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

    public float[] spectrum;
    public int spectrumSamples = 1024;

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
            }
        }
    }

    private void Start() {

    }

    private void Update() {
        if (stationChoice == StationChoice.StrongestSignal) {
            float strongestSignal = allStations.Max(x => x.signalStrength);
            RadioStation station = allStations.First(x => Mathf.Approximately(x.signalStrength, strongestSignal));

            source = station.audioSource;
            stream = station.stream;
        }

        if (stream) {
            spectrum = stream.spectrum;
            spectrumSamples = spectrum.Length;
        } else if (source) {
            source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        } else if (stationChoice == StationChoice.None) {
            AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        }
    }
}
