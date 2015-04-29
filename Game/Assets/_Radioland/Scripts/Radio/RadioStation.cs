using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class RadioStation : MonoBehaviour
{
    [Range(0.0f, 1.0f)] public float maxVolume = 1.0f;
    public float frequency;
    public float signalStrength;
    [SerializeField] private bool m_unlocked = false;
    public bool unlocked { get { return m_unlocked; } }
    public float maxSignalStrength = 1.0f;
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
            if (audioSource.clip.loadState == AudioDataLoadState.Unloaded) {
                audioSource.clip.LoadAudioData();
            }
        } else {
            audioSource.enabled = false;
            if (audioSource.clip.loadState == AudioDataLoadState.Loaded) {
                audioSource.clip.UnloadAudioData();
            }
        }

        audioSource.volume = signalStrength * maxVolume;
        if (stream) {
            stream.volume = signalStrength * maxVolume;
        }

        // Update signal strength.
        if (!unlocked) {
            signalStrength = 0f;
        } else {
            // Lerp signalStrength from 0 to 1 within frequencyFadeLimit.
            // When outside of frequencyFadeLimit from the current frequency, clamp to 0.
            float frequencyDifference = Mathf.Abs(radioControl.currentFrequency - frequency);
            float strength = radioControl.frequencyHighScale -
                                (radioControl.frequencyHighScale *
                                frequencyDifference / radioControl.frequencyFadeLimit);
            strength = Mathf.Lerp(0f, maxSignalStrength, strength);
            signalStrength = strength;
        }
    }

    public bool StrongSignal() {
        return signalStrength > radioControl.powerupMinSignalStrength;
    }

    public void Unlock() {
        m_unlocked = true;
        Messenger.Broadcast("UnlockStation", id);
    }
}
