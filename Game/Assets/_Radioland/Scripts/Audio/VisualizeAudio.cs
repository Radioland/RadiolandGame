using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class VisualizeAudio : MonoBehaviour
{
    private enum StationChoice {
        None,
        StrongestSignal,
        Station_1,
        Station_2,
        Station_3
    }

    [Header("Automatic Source Setup")]
    [SerializeField] private StationChoice stationChoice = StationChoice.Station_1;
    [Header("Manual Source Setup")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioStream stream;
    [Header("Automatic Objects Setup")]
    [SerializeField] private GameObject spectrumObjectPrefab;
    [SerializeField] private int spectumObjectCount = 5;
    [SerializeField] [Tooltip("This will be applied in the forward direction.")]
    private float spectrumObjectOffset = 0.5f;
    [Header("Manual Objects Setup")]
    [SerializeField] private List<GameObject> spectrumObjects;
    [Header("Configuration")]
    [SerializeField] private float minScale = 0.2f;
    [SerializeField] private float maxScale = 2.0f;
    [SerializeField] private int spectrumSamples = 1024;
    [SerializeField] private int upperFrequency = 1024;

    private RadioStation[] allStations;
    private float[] spectrum;

    private Vector3 originalScale;

    private static Color gizmoBoxColor = Color.blue;

    private void Awake() {
        originalScale = spectrumObjectPrefab.transform.localScale;

        spectrum = new float[spectrumSamples];

        if (spectrumObjects.Count == 0) {
            spectrumObjects = new List<GameObject>();
            for (int i = 0; i < spectumObjectCount; i++) {
                GameObject spectrumObject = (GameObject)Instantiate(spectrumObjectPrefab);

                spectrumObject.transform.parent = transform;
                spectrumObject.transform.position = transform.position + i * transform.forward * spectrumObjectOffset;
                spectrumObject.transform.localRotation = Quaternion.identity;

                spectrumObjects.Add(spectrumObject);
            }
        }

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

        float frequencyPerElement = AudioSettings.outputSampleRate / 2f / spectrumSamples;
        int stopIndex = Mathf.Min(Mathf.FloorToInt(upperFrequency / frequencyPerElement), spectrumSamples);

        float maxAmplitude = 0;
        for (int i = 0; i < stopIndex; i++) {
            maxAmplitude = Mathf.Max(maxAmplitude, spectrum[i]);
        }

        if (maxAmplitude <= 0.001) { return; }

        int spectrumSamplesPerObject = stopIndex / spectumObjectCount;
        for (int i = 0; i < spectumObjectCount; i++) {
            float spectrumSum = 0f;
            for (int j = 0; j < spectrumSamplesPerObject; j++) {
                spectrumSum += spectrum[j + i * spectrumSamplesPerObject];
            }

            float relativeScaleFactor = spectrumSum / spectrumSamplesPerObject / maxAmplitude;
            if (stream) { relativeScaleFactor *= stream.volume; }
            else if (source) { relativeScaleFactor *= source.volume; }
            float scaleFactor = Mathf.Lerp(minScale, maxScale, relativeScaleFactor);
            spectrumObjects[i].transform.localScale = new Vector3(originalScale.x,
                                                                  originalScale.y * scaleFactor,
                                                                  originalScale.z);
        }
    }

    public void OnDrawGizmos() {
//        if (spectrumObjects.Count != 0) { return; }

        // Draw a cube around the space taken up by the cubes.

        float depth = spectrumObjectPrefab.renderer.bounds.size.z;
        float width = spectrumObjectPrefab.renderer.bounds.size.x;
        float height = (spectrumObjectPrefab.renderer.bounds.size.y *
                        spectrumObjectPrefab.transform.localScale.y * maxScale);

        Vector3 start = transform.position - (transform.forward * depth / 2.0f);
        Vector3 end = start + (transform.forward * spectrumObjectOffset * (spectumObjectCount));
        Vector3 center = (start + end) * 0.5f;
        float distance = Vector3.Distance(start, end);
//        Quaternion direction = Quaternion.LookRotation(end - start);
        Quaternion direction = transform.rotation;

        Matrix4x4 transformMatrix = Matrix4x4.TRS(center, direction, Vector3.one);
        Gizmos.matrix = transformMatrix;

        Gizmos.color = gizmoBoxColor;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(width, height, distance));
    }
}
