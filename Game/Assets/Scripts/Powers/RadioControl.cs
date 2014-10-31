using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RadioControl : MonoBehaviour
{
    public float frequencyFadeLimit = 0.2f;
    public float powerupMinSignalStrength = 0.25f;
    public float currentFrequency { get { return radioDialSlider.value; } }

    // Private variables set in the inspector.
    [SerializeField] private PowerupManager powerupManager;
    [SerializeField] private Slider radioDialSlider;
    [SerializeField] private Image energyGlowImage;
    [SerializeField] private RectTransform radioKnobTransform;
    [SerializeField] private float knobTurnRatio = 4.0f;
    [Tooltip("Plays when seeking between stations.")]
    [SerializeField] [Range(0.0f, 1.0f)] private float stationCutoff = 0.2f;
    [SerializeField] private AudioSource backgroundSource;
    [SerializeField] [Range(0.0f, 1.0f)] private float backgroundMaxVolume = 0.1f;
    [SerializeField] private AudioSource staticSource;
    [SerializeField] [Range(0.0f, 1.0f)] private float staticMaxVolume = 0.5f;
    [SerializeField] private float staticFadeTime = 3.0f;
    [SerializeField] private float staticLingerTime = 1.0f;

    private RadioStation[] stations;
    private bool inUse;
    private float lastStartedTime;
    private float lastActiveTime;
    private float lastDecreaseVolume;
    private float lastIncreaseVolume;

    void Awake() {
        if (!powerupManager) {
            Debug.LogWarning("[UI] PowerupManager needs to be set for RadioControl to show energy.");
        }
        if (!energyGlowImage) {
            Debug.LogWarning("[UI] Please set the energyGlow Image for RadioControl.");
        }
        if (!radioKnobTransform) {
            Debug.LogWarning("[UI] Please set the knob Transform for RadioControl.");
        }
        if (!radioDialSlider) {
            // Hope that there is only one slider on this object.
            // Warning: if there are multiple, this could be undesired behavior!
            if (transform.parent) {
                radioDialSlider = transform.parent.GetComponentInChildren<Slider>();
            }

            if (!radioDialSlider) {
                Debug.LogWarning("[UI] Slider needs to be set for RadioControl to function.");
                gameObject.SetActive(false);
            }
        }

        if (!staticSource) {
            Debug.LogWarning("[Audio] Please set the staticSource for RadioControl.");
        } else {
            staticSource.volume = 0.0f;
        }

        stations = gameObject.GetComponentsInChildren<RadioStation>();
        foreach (RadioStation station in stations) {
            station.radioControl = this;
        }

        ResetStatic();
    }

    void Start() {

    }

    void ResetStatic() {
        inUse = false;
        lastStartedTime = -1000.0f;
        lastActiveTime = -1000.0f;
        lastIncreaseVolume = 0.0f;
        lastDecreaseVolume = 0.0f;
    }

    void Update() {
        // Debug controls.
        if (Input.GetKey(KeyCode.Alpha1)) {
            radioDialSlider.value -= 0.01f;
        }
        if (Input.GetKey(KeyCode.Alpha2)) {
            radioDialSlider.value += 0.01f;
        }

        float scrollValue = Input.GetAxis("Mouse ScrollWheel");
        radioDialSlider.value -= scrollValue;

        // Track activity using raw input.
        float rawScroll = Input.GetAxisRaw("Mouse ScrollWheel");
        if (Mathf.Abs(rawScroll) > 0.001f) {
            if (!inUse) {
                inUse = true;
                if (Time.time - lastActiveTime > staticLingerTime) {
                    lastStartedTime = Time.time;
                }
            }
            lastActiveTime = Time.time;
        } else {
            inUse = false;
        }

        // Rotate the knob by a factor of the value.
        if (radioKnobTransform) {
            float rotationDegrees = radioDialSlider.value * 360.0f * knobTurnRatio;
            radioKnobTransform.localRotation = Quaternion.Euler(0, 0, rotationDegrees);
        }

        // Fade glow image based on energy percentage.
        if (powerupManager && energyGlowImage) {
            Color newColor = energyGlowImage.color;
            newColor.a = powerupManager.energy;
            energyGlowImage.color = newColor;
        }

        // Trigger powerups.
        if (Input.GetButtonDown("Fire1")) {
            foreach (RadioStation station in stations) {
                if (station.signalStrength > powerupMinSignalStrength) {
                    station.UsePowerup();
                }
            }
        }

        float maxSignal = 0.0f;
        foreach (RadioStation station in stations) {
            maxSignal = Mathf.Max(maxSignal, station.signalStrength);
        }

        if (backgroundSource) {
            backgroundSource.volume = (1.0f - maxSignal / stationCutoff) * backgroundMaxVolume;
        }

        // Adjust the volume of the static to fill in between stations.
        if (staticSource) {
            float staticStrength = (1.0f - maxSignal / stationCutoff) * staticMaxVolume;

            float volume;
            if (inUse || Time.time - lastActiveTime < staticLingerTime) {
                // Fade in.
                float tStart = (Time.time - lastStartedTime) / staticFadeTime;
                volume = Mathf.Lerp(lastDecreaseVolume, staticStrength, Mathf.Clamp01(tStart));
                lastIncreaseVolume = volume;
            } else {
                // Fade out.
                float tEnd = (Time.time - staticLingerTime - lastActiveTime) / staticFadeTime;
                volume = Mathf.Lerp(lastIncreaseVolume, 0.0f, Mathf.Clamp01(tEnd));
                lastDecreaseVolume = volume;
            }

            if (maxSignal > stationCutoff) {
                ResetStatic();
            }

            staticSource.volume = volume;
        }
    }
}
