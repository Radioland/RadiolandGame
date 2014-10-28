using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RadioControl : MonoBehaviour
{
    public float frequencyFadeLimit = 0.1f;
    public float powerupMinSignalStrength = 0.5f;
    public float currentFrequency { get { return radioDialSlider.value; } }

    // Private variables set in the inspector.
    [SerializeField] private Slider radioDialSlider;
    [SerializeField] private RectTransform radioKnobTransform;
    [SerializeField] private float knobTurnRatio = 4.0f;

    private RadioStation[] stations;

    void Awake() {
        if (!radioKnobTransform) {
            Debug.LogWarning("Please set the Knob for RadioControl.");
        }
        if (!radioDialSlider) {
            Debug.LogWarning("Please set the Slider for RadioControl.");
            gameObject.SetActive(false);
        }

        stations = gameObject.GetComponentsInChildren<RadioStation>();
        foreach (RadioStation station in stations) {
            station.radioControl = this;
        }
    }

    void Start() {

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
        radioDialSlider.value += scrollValue;

        if (radioKnobTransform) {
            float rotationDegrees = radioDialSlider.value * 360.0f * knobTurnRatio;
            radioKnobTransform.localRotation = Quaternion.Euler(0, 0, -rotationDegrees);
        }

        if (Input.GetButtonDown("Fire1")) {
            foreach (RadioStation station in stations) {
                if (station.signalStrength > powerupMinSignalStrength) {
                    station.UsePowerup();
                }
            }
        }
    }
}
