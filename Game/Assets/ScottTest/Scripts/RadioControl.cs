using UnityEngine;
using UnityEngine.UI;
using System.Collections;
//using System.Collections.Generic;

public class RadioControl : MonoBehaviour
{
    public float frequencyFadeLimit = 0.1f;
    public float currentFrequency { get { return radioDialSlider.value; } }

    // Private variables set in the inspector.
    [SerializeField] private Slider radioDialSlider;

    private RadioStation[] stations;

    void Awake() {
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
        if (Input.GetKey(KeyCode.Alpha1)) {
            radioDialSlider.value -= 0.01f;
        }
        if (Input.GetKey(KeyCode.Alpha2)) {
            radioDialSlider.value += 0.01f;
        }

        float scrollValue = Input.GetAxis("Mouse ScrollWheel");
        radioDialSlider.value += scrollValue;
    }
}
