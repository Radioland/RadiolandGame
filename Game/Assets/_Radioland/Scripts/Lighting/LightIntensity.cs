﻿using UnityEngine;
using System.Collections;

public class LightIntensity : MonoBehaviour
{
    [SerializeField] private float minIntensity = 0.25f;
    [SerializeField] private float maxIntensity = 0.6f;
    [SerializeField] private int minBrightness = 120;
    [SerializeField] private int maxBrightness = 240;
    [SerializeField] private float duration = 3f;

    private float currentTime;
    private float timeDamp;

    private void Awake() {
        currentTime = 0f;
        timeDamp = 0f;
    }

    private void Start() {

    }

    private void Update() {

    }

    public void SetIntensity(float t) {
        currentTime = t;

        float intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
        float brightness = Mathf.Lerp(minBrightness, maxBrightness, t) / 255.0f;

        Light[] lights = gameObject.GetComponentsInChildren<Light>();
        foreach (Light light in lights) {
            light.intensity = intensity;
            Color lightColor = light.color;
            HSBColor lightColorHSB = new HSBColor(lightColor);
            lightColorHSB.b = brightness;
            light.color = lightColorHSB.ToColor();
        }
    }

    public void SetIntensitySmooth(float t) {
        SetIntensity(Mathf.SmoothDamp(currentTime, t, ref timeDamp, duration));
    }
}