using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class RadioStation : MonoBehaviour
{
    [Range(0.0f, 1.0f)] public float maxVolume = 1.0f;
    public float frequency;
    public float signalStrength {
        get {
            if (!unlocked) { return 0.0f; }
            // Lerp signalStrength from 0 to 1 within frequencyFadeLimit.
            // When outside of frequencyFadeLimit from the current frequency, clamp to 0.
            float frequencyDifference = Mathf.Abs(radioControl.currentFrequency - frequency);
            float strength = radioControl.frequencyHighScale -
                             (radioControl.frequencyHighScale *
                              frequencyDifference / radioControl.frequencyFadeLimit);
            strength = Mathf.Clamp(strength, 0.0f, 1.0f);
            return strength;
        }
    }
    [SerializeField] private bool m_unlocked = false;
    public bool unlocked { get { return m_unlocked; } }
    public int id;

    public AudioStream stream;

    [HideInInspector] public RadioControl radioControl;
    [HideInInspector] public AudioSource audioSource;

    private void Awake() {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.volume = 0;
    }

    private void Start() {
        if (!radioControl) {
            Debug.LogWarning("There is no RadioControl linked to this RadioStation!");
        }

    }

    private void Update() {
        if ((!stream || (stream && !stream.streamInitialized)) && (audioSource)) {
            audioSource.enabled = true;
            stream.enabled = false;
        } else {
            stream.enabled = true;
            audioSource.enabled = false;
        }

        audioSource.volume = signalStrength * maxVolume;
        if (stream) {
            stream.volume = signalStrength * maxVolume;
        }
    }

    public bool StrongSignal() {
        return signalStrength > radioControl.powerupMinSignalStrength;
    }

    public void Unlock() {
        m_unlocked = true;
    }
}
