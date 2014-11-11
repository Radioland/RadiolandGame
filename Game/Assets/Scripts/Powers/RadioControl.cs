using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RadioControl : MonoBehaviour
{
    public float frequencyFadeLimit = 0.2f;
    public float powerupMinSignalStrength = 0.25f;
    public float currentFrequency { get { return radioDialSlider.value; } }
    public float backgroundStrength { get { return (1.0f - maxSignal / stationCutoff); } }

    // Private variables set in the inspector.
    [SerializeField] private PowerupManager powerupManager;
    [SerializeField] private Slider radioDialSlider;
    [SerializeField] private Image energyGlowImage;
    [SerializeField] private RectTransform radioKnobTransform;
    [SerializeField] private float knobTurnRatio = 4.0f;
    [Tooltip("Plays when seeking between stations.")]
    [SerializeField] [Range(0.0f, 1.0f)] private float stationCutoff = 0.2f;
    [SerializeField] private AudioSource staticSource;
    [SerializeField] [Range(0.0f, 1.0f)] private float staticMaxVolume = 0.5f;
    [SerializeField] private float staticFadeTime = 3.0f;
    [SerializeField] private float staticLingerTime = 1.0f;

    // Secondary UI (screen space).
    [SerializeField] private GameObject screenUIPrefab;
    private GameObject screenUI;
    private Slider radioDialSlider2;
    private RectTransform radioKnobTransform2;
    private Image energyGlowImage2;

    private RadioStation[] stations;
    private bool inUse;
    private float lastStartedTime;
    private float lastActiveTime;
    private float lastDecreaseVolume;
    private float lastIncreaseVolume;
    private float maxSignal;

    private float GUILingerTime;

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

        maxSignal = 0.0f;

        GUILingerTime = -5;

        screenUI = (GameObject) GameObject.Instantiate(screenUIPrefab);
        RadioUI radioUI = screenUI.GetComponent<RadioUI>();
        radioDialSlider2 = radioUI.radioSlider;
        radioKnobTransform2 = radioUI.radioKnob;
        energyGlowImage2 = radioUI.radioEnergyGlowImage;

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
            radioDialSlider2.value -= 0.01f;
        }
        if (Input.GetKey(KeyCode.Alpha2)) {
            radioDialSlider.value += 0.01f;
            radioDialSlider2.value += 0.01f;
        }

        // TODO: replace with better controller/mouse input management.
        float scrollValue = Input.GetAxis("Mouse ScrollWheel") + Input.GetAxis("Tune");
        radioDialSlider.value -= scrollValue;
        radioDialSlider2.value += scrollValue;

        // Track activity using raw input.
        float rawScroll = Input.GetAxisRaw("Mouse ScrollWheel") + Input.GetAxisRaw("Tune");
        if (Mathf.Abs(rawScroll) > 0.001f) {
            GUILingerTime = Time.time;
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
            radioKnobTransform2.localRotation = Quaternion.Euler(0,0, -1 * rotationDegrees);
        }

        // Fade glow image based on energy percentage.
        if (powerupManager) {
            Color newColor;
            if (energyGlowImage) {
                newColor = energyGlowImage.color;
                newColor.a = powerupManager.energy;
                energyGlowImage.color = newColor;
            }
            if (energyGlowImage2) {
                newColor = energyGlowImage2.color;
                newColor.a = powerupManager.energy;
                energyGlowImage2.color = newColor;
            }
        }

        // Trigger powerups.
        if (Input.GetButtonDown("Fire1")) {
            foreach (RadioStation station in stations) {
                if (station.signalStrength > powerupMinSignalStrength) {
                    station.UsePowerup();
                }
            }
        }

        maxSignal = 0.0f;
        foreach (RadioStation station in stations) {
            maxSignal = Mathf.Max(maxSignal, station.signalStrength);
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

            if (inUse || Time.time - GUILingerTime < 1.5f) {
                screenUI.SetActive(true);
            }
            else {
                screenUI.SetActive(false);
            }

            if (maxSignal > stationCutoff) {
                ResetStatic();
            }

            staticSource.volume = volume;
        }
    }
}
