using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RadioUI : MonoBehaviour
{
    [SerializeField] private Slider radioSlider;
    [SerializeField] private RectTransform radioKnob;
    [SerializeField] private Image radioEnergyGlowImage;

    [SerializeField] private bool invert = false;

    private Image[] images;
    private float currentAlpha;
    private float currentGlowAlpha;

    void Awake() {
        images = gameObject.GetComponentsInChildren<Image>();

        currentAlpha = 1.0f;
    }

    void Start() {

    }

    void Update() {

    }

    public void SetSliderValue(float newValue) {
        if (radioSlider) {
            radioSlider.value = (invert ? 1 - newValue : newValue);
        }
    }

    public void SetKnobRotation(float rotationDegrees) {
        if (radioKnob) {
            radioKnob.localRotation = Quaternion.Euler(0, 0, rotationDegrees * (invert ? -1 : 1));
        }
    }

    public void SetGlowAlpha(float newAlpha) {
        currentGlowAlpha = newAlpha;

        if (radioEnergyGlowImage) {
            Color color = radioEnergyGlowImage.color;
            color.a = newAlpha * currentAlpha;
            radioEnergyGlowImage.color = color;
        }
    }

    public void SetAlpha(float alpha) {
        currentAlpha = alpha;

        foreach (Image image in images) {
            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }

        SetGlowAlpha(currentGlowAlpha);
    }
}
