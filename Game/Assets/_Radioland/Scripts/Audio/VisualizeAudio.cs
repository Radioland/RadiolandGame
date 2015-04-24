using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisualizeAudio : MonoBehaviour
{
    [Header("Spectrum Sources")]
    [SerializeField] private SpectrumSource primarySource;
    [SerializeField] private SpectrumSource secondarySource;
    [Header("Automatic Objects Setup")]
    [SerializeField] private GameObject spectrumObjectPrefab;
    [SerializeField] private int spectumObjectCount = 6;
    [SerializeField] [Tooltip("This will be applied in the forward direction.")]
    private float spectrumObjectOffset = 0.5f;
    [SerializeField] private Gradient colorGradient;
    [Header("Manual Objects Setup")]
    [SerializeField] private List<GameObject> spectrumObjects;
    [Header("Configuration")]
    [SerializeField] private bool scaleX = false;
    [SerializeField] private bool scaleY = true;
    [SerializeField] private bool scaleZ = false;
    [SerializeField] private float minScale = 0.2f;
    [SerializeField] private float maxScale = 4.0f;
    [SerializeField] private float lerpSpeed = 0.5f;
    [SerializeField] private int upperFrequency = 1024;

    private SpectrumSource spectrumSource;
    private int spectrumSamples;
    private float[] spectrum;

    private Vector3[] originalScales;

    private static Color gizmoBoxColor = Color.blue;

    private void Awake() {
        if (!primarySource) { primarySource = gameObject.GetComponent<SpectrumSource>(); }

        spectrumSource = primarySource;
        spectrumSamples = spectrumSource.spectrumSamples;
        spectrum = new float[spectrumSamples];

        if (spectrumObjects.Count == 0) {
            // Automatic setup.
            originalScales = new Vector3[spectumObjectCount];
            spectrumObjects = new List<GameObject>();
            for (int i = 0; i < spectumObjectCount; i++) {
                originalScales[i] = spectrumObjectPrefab.transform.localScale;
                GameObject spectrumObject = (GameObject) Instantiate(spectrumObjectPrefab);

                spectrumObject.transform.parent = transform;
                spectrumObject.transform.position = transform.position + i * transform.forward * spectrumObjectOffset;
                spectrumObject.transform.localRotation = Quaternion.identity;

                // Set tint color.
                Renderer spectrumObjectRenderer = spectrumObject.GetComponent<Renderer>();
                if (spectrumObjectRenderer) {
                    spectrumObjectRenderer.material.SetColor("_Color", colorGradient.Evaluate(i * 1.0f / spectumObjectCount));
                }

                spectrumObjects.Add(spectrumObject);
            }
        } else {
            // Manual setup.
            spectumObjectCount = spectrumObjects.Count;
            originalScales = new Vector3[spectumObjectCount];
            for (int i = 0; i < spectumObjectCount; i++) {
                originalScales[i] = spectrumObjects[i].transform.localScale;
            }
        }
    }

    private void Start() {

    }

    private void UpdateSource() {
        if (!secondarySource) { return; }

        spectrumSource = (primarySource.maxAmplitude * primarySource.volume >
                          secondarySource.maxAmplitude * secondarySource.volume) ?
                          primarySource : secondarySource;
    }

    private void Update() {
        UpdateSource();

        spectrum = spectrumSource.spectrum;

        int stopIndex = Mathf.Min(Mathf.FloorToInt(upperFrequency / spectrumSource.frequencyPerElement),
                                  spectrumSamples);

        float maxAmplitude = 0;
        for (int i = 0; i < stopIndex; i++) {
            maxAmplitude = Mathf.Max(maxAmplitude, spectrum[i]);
        }

        int spectrumSamplesPerObject = stopIndex / spectumObjectCount;
        for (int i = 0; i < spectumObjectCount; i++) {
            float spectrumSum = 0f;
            for (int j = 0; j < spectrumSamplesPerObject; j++) {
                spectrumSum += spectrum[j + i * spectrumSamplesPerObject];
            }

            // Scale the sum based on the samples and volumes.
            float relativeScaleFactor = 0;
            if (maxAmplitude > 0.001) {
                relativeScaleFactor = spectrumSum / spectrumSamplesPerObject / maxAmplitude;
            }
            relativeScaleFactor *= spectrumSource.volume;
            if (spectrumSource.radioStation) { relativeScaleFactor /= spectrumSource.radioStation.maxVolume; }

            float scaleFactor = Mathf.Lerp(minScale, maxScale, relativeScaleFactor);
            Vector3 originalScale = originalScales[i];
            Vector3 newScale = originalScale;
            if (scaleX) { newScale.x = originalScale.x * scaleFactor; }
            if (scaleY) { newScale.y = originalScale.y * scaleFactor; }
            if (scaleZ) { newScale.z = originalScale.z * scaleFactor; }
            spectrumObjects[i].transform.localScale = Vector3.Lerp(spectrumObjects[i].transform.localScale,
                                                                   newScale, lerpSpeed);
        }
    }

    public void OnDrawGizmos() {
        if (!spectrumObjectPrefab) { return; }
        // if (spectrumObjects.Count != 0) { return; }

        // Draw a box around the space taken up by the spectrum objects.

        Renderer spectrumObjectPrefabRenderer = spectrumObjectPrefab.GetComponent<Renderer>();
        float depth = spectrumObjectPrefabRenderer.bounds.size.z;
        float width = spectrumObjectPrefabRenderer.bounds.size.x;
        float height = (spectrumObjectPrefabRenderer.bounds.size.y * maxScale);

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
