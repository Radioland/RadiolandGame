using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RadioControl : MonoBehaviour
{
    public float frequencyFadeLimit = 0.2f;
    public float powerupMinSignalStrength = 0.25f;
    public float backgroundStrength { get { return (1.0f - maxSignal / stationCutoff); } }

    private float m_currentFrequency;
    public float currentFrequency { get { return m_currentFrequency; } }

    // Private variables set in the inspector.
    [SerializeField] private PowerupManager powerupManager;
    [SerializeField] private float knobTurnRatio = 4.0f;
    [Tooltip("Plays when seeking between stations.")]
    [SerializeField] [Range(0.0f, 1.0f)] private float stationCutoff = 0.2f;
    [SerializeField] private AudioSource staticSource;
    [SerializeField] [Range(0.0f, 1.0f)] private float staticMaxVolume = 0.5f;
    [SerializeField] private float staticFadeTime = 3.0f;
    [SerializeField] private float staticLingerTime = 1.0f;

    [SerializeField] private List<RadioUI> radioUIs;

    // Secondary UI (screen space).
    [SerializeField] private GameObject screenUIPrefab;
    private GameObject screenUIObject;
    private RadioUI screenUI;

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
            Debug.LogWarning("[UI] Please set PowerupManager for RadioControl.");
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
        m_currentFrequency = 0.5f;

        GUILingerTime = -5;

        screenUIObject = (GameObject) GameObject.Instantiate(screenUIPrefab);
        screenUI = screenUIObject.GetComponent<RadioUI>();

        radioUIs.Add(screenUI);

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

        // TODO: replace with better controller/mouse input management.
        float scrollValue = Input.GetAxis("Mouse ScrollWheel") + Input.GetAxis("Tune");

        // Debug controls.
        if (Input.GetKey(KeyCode.Alpha1)) {
            scrollValue += 0.01f;
        }
        if (Input.GetKey(KeyCode.Alpha2)) {
            scrollValue -= 0.01f;
        }

        m_currentFrequency -= scrollValue;
        m_currentFrequency = Mathf.Clamp01(m_currentFrequency);

        foreach (RadioUI radioUI in radioUIs) {
            radioUI.SetSliderValue(m_currentFrequency);
        }

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

        // Rotate the knob by a factor of the slider value.
        foreach (RadioUI radioUI in radioUIs) {
            float rotationDegrees = m_currentFrequency * 360.0f * knobTurnRatio;
            radioUI.SetKnobRotation(rotationDegrees);
        }

        // Fade glow image based on energy percentage.
        if (powerupManager) {
            foreach (RadioUI radioUI in radioUIs) {
                radioUI.SetGlowAlpha(powerupManager.energy);
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

            if (maxSignal > stationCutoff) {
                ResetStatic();
            }

            staticSource.volume = volume;
        }

        // Fade the secondary UI in and out.
        if (inUse || Time.time - GUILingerTime < 1.5f) {
            screenUIObject.SetActive(true);
        }
        else {
            screenUIObject.SetActive(false);
        }
    }
}
