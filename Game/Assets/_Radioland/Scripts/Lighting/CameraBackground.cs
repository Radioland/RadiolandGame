using UnityEngine;
using System.Collections;

public class CameraBackground : MonoBehaviour
{
    [SerializeField] private int minSaturation = 37;
    [SerializeField] private int maxSaturation = 80;
    [SerializeField] private int minBrightness = 71;
    [SerializeField] private int maxBrightness = 240;
    [SerializeField] private float duration = 3f;

    private float currentTime;
    private float timeDamp;

    private Camera cameraToChange;

    private void Awake() {
        currentTime = 0f;
        timeDamp = 0f;

        cameraToChange = gameObject.GetComponent<Camera>();
    }

    private void Start() {

    }

    private void Update() {

    }

    public void SetIntensity(float t) {
        currentTime = t;

        float saturation = (int) Mathf.Lerp(minSaturation, maxSaturation, t) / 255.0f;
        float brightness = (int) Mathf.Lerp(minBrightness, maxBrightness, t) / 255.0f;

        HSBColor cameraColorHSB = new HSBColor(cameraToChange.backgroundColor);
        cameraColorHSB.b = brightness;
        cameraColorHSB.s = saturation;
        cameraToChange.backgroundColor = cameraColorHSB.ToColor();
    }

    public void SetIntensitySmooth(float t) {
        SetIntensity(Mathf.SmoothDamp(currentTime, t, ref timeDamp, duration));
    }
}
