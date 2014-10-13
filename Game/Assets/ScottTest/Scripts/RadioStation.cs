using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

public class RadioStation : MonoBehaviour
{
    public float frequency;

    [HideInInspector] public RadioControl radioControl;
    [HideInInspector] public AudioSource audioSource;

    void Awake() {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.volume = 0;
    }

    void Start() {
        if (!radioControl) {
            Debug.LogWarning("There is no RadioControl linked to this RadioStation!");
        }
    }

    void Update() {
        // Lerp signalStrength from 0 to 1 within frequencyFadeLimit.
        // When outside of frequencyFadeLimit from the current frequency, clamp to 0.
        float frequencyDifference = Mathf.Abs(radioControl.currentFrequency - frequency);
        float signalStrength = 1.0f - frequencyDifference / radioControl.frequencyFadeLimit;
        signalStrength = Mathf.Clamp(signalStrength, 0.0f, 1.0f);

        audioSource.volume = signalStrength;
    }
}
