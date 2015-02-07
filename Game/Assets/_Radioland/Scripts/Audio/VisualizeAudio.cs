using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class VisualizeAudio : MonoBehaviour
{
    [SerializeField] private GameObject spectrumObjectPrefab;
    [SerializeField] private int spectumObjectCount;
    [SerializeField] private float minScale = 0.2f;
    [SerializeField] private float maxScale = 2.0f;
    [SerializeField] private int spectrumSamples = 1024;
    [SerializeField] private int upperFrequency = 1024;

    private float[] spectrum;
    private List<GameObject> spectrumObjects;

    private void Awake() {
        spectrum = new float[spectrumSamples];

        spectrumObjects = new List<GameObject>();
        for (int i = 0; i < spectumObjectCount; i++) {
            GameObject spectrumObject = (GameObject)Instantiate(spectrumObjectPrefab);

            spectrumObject.transform.parent = transform;
            spectrumObject.transform.position = transform.position + new Vector3(i * 0.5f, 0, 0);

            spectrumObjects.Add(spectrumObject);
        }
    }

    private void Start() {

    }

    private void Update() {
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        float frequencyPerElement = AudioSettings.outputSampleRate / 2f / spectrumSamples;
        int stopIndex = Mathf.Min(Mathf.FloorToInt(upperFrequency / frequencyPerElement), spectrumSamples);

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

            float relativeScaleFactor = spectrumSum / spectrumSamplesPerObject / maxAmplitude;
            float scaleFactor = Mathf.Lerp(minScale, maxScale, relativeScaleFactor);
            spectrumObjects[i].transform.localScale = new Vector3(1f, scaleFactor, 1f);
        }
    }
}
