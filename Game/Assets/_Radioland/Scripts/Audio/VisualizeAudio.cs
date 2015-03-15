using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpectrumSource))]
public class VisualizeAudio : MonoBehaviour
{
    [Header("Automatic Objects Setup")]
    [SerializeField] private GameObject spectrumObjectPrefab;
    [SerializeField] private int spectumObjectCount = 6;
    [SerializeField] [Tooltip("This will be applied in the forward direction.")]
    private float spectrumObjectOffset = 0.5f;
    [Header("Manual Objects Setup")]
    [SerializeField] private List<GameObject> spectrumObjects;
    [Header("Configuration")]
    [SerializeField] private float minScale = 0.2f;
    [SerializeField] private float maxScale = 4.0f;
    [SerializeField] private int upperFrequency = 1024;

    private SpectrumSource spectrumSource;
    private int spectrumSamples;
    private float[] spectrum;

    private Vector3 originalScale;

    private static Color gizmoBoxColor = Color.blue;

    private void Awake() {
        originalScale = spectrumObjectPrefab.transform.localScale;

        spectrumSource = gameObject.GetComponent<SpectrumSource>();
        spectrumSamples = spectrumSource.spectrumSamples;
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
    }

    private void Start() {

    }

    private void Update() {
        spectrum = spectrumSource.spectrum;

        int stopIndex = Mathf.Min(Mathf.FloorToInt(upperFrequency / spectrumSource.frequencyPerElement),
                                  spectrumSamples);

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

            // Scale the sum based on the samples and volumes.
            float relativeScaleFactor = spectrumSum / spectrumSamplesPerObject / maxAmplitude;
            if (spectrumSource.stream) { relativeScaleFactor *= spectrumSource.stream.volume; }
            else if (spectrumSource.source) { relativeScaleFactor *= spectrumSource.source.volume; }
            if (spectrumSource.radioStation) { relativeScaleFactor /= spectrumSource.radioStation.maxVolume; }

            float scaleFactor = Mathf.Lerp(minScale, maxScale, relativeScaleFactor);
            spectrumObjects[i].transform.localScale = new Vector3(originalScale.x,
                                                                  originalScale.y * scaleFactor,
                                                                  originalScale.z);
        }
    }

    public void OnDrawGizmos() {
        if (!spectrumObjectPrefab) { return; }
        // if (spectrumObjects.Count != 0) { return; }

        // Draw a box around the space taken up by the spectrum objects.

        Renderer spectrumObjectPrefabRenderer = spectrumObjectPrefab.GetComponent<Renderer>();
        float depth = spectrumObjectPrefabRenderer.bounds.size.z;
        float width = spectrumObjectPrefabRenderer.bounds.size.x;
        float height = (spectrumObjectPrefabRenderer.bounds.size.y *
                        spectrumObjectPrefab.transform.localScale.y * maxScale);

        Vector3 start = transform.position - (transform.forward * depth / 2.0f);
        Vector3 end = start + (transform.forward * spectrumObjectOffset * (spectumObjectCount));
        Vector3 center = (start + end) * 0.5f;
        float distance = Vector3.Distance(start, end);
        Quaternion direction = transform.rotation;

        Matrix4x4 transformMatrix = Matrix4x4.TRS(center, direction, Vector3.one);
        Gizmos.matrix = transformMatrix;

        Gizmos.color = gizmoBoxColor;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(width, height, distance));
    }
}
