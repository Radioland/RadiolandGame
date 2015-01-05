using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

public class RadioStation : MonoBehaviour
{
    [SerializeField] [Range(0.0f, 1.0f)] private float maxVolume = 1.0f;
    public float frequency;
    public float signalStrength {
        get {
            if (!unlocked) { return 0.0f; }
            // Lerp signalStrength from 0 to 1 within frequencyFadeLimit.
            // When outside of frequencyFadeLimit from the current frequency, clamp to 0.
            float frequencyDifference = Mathf.Abs(radioControl.currentFrequency - frequency);
            float strength = 1.0f - frequencyDifference / radioControl.frequencyFadeLimit;
            strength = Mathf.Clamp(strength, 0.0f, 1.0f);
            return strength;
        }
    }
    public Powerup powerup;
    [SerializeField] private bool m_unlocked = false;
    public bool unlocked { get { return m_unlocked; } }
    public int id;

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

        if (powerup) {
            powerup.radioStation = this;
        }
    }

    private void Update() {
        audioSource.volume = signalStrength * maxVolume;

        // End if the station signal is no longer strong enough.
        if (powerup && powerup.primed && !powerup.inUse &&
            (signalStrength < radioControl.powerupMinSignalStrength)) {
            powerup.EndPowerup();
        }
    }

    public bool StrongSignal() {
        return signalStrength > radioControl.powerupMinSignalStrength;
    }

    public void UsePowerup() {
        if (powerup && unlocked) {
            powerup.TryToUsePowerup();
        } else {
            Debug.LogWarning("Found no powerup for " + this.GetPath());
        }
    }

    public void Unlock() {
        m_unlocked = true;
    }
}
