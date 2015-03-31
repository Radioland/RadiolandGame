using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DarkFogControl : MonoBehaviour
{
    public float fogLevel = 0f;

    [SerializeField] private Slider displaySlider;

    private float fogLevelTarget;
    private float fogSmoothVel = 0f;
    private const float fogSmoothTime = 0.5f;

    private void Awake() {
        if (displaySlider) { displaySlider.value = fogLevel; }

        Messenger.AddListener<float>("ChangeFogLevel", OnChangeFogLevel);
        Messenger.AddListener<float>("ChangeFogLevelSmooth", OnChangeFogLevelSmooth);
    }

    private void Start() {

    }

    private void Update() {
        fogLevel = Mathf.SmoothDamp(fogLevel, fogLevelTarget, ref fogSmoothVel, fogSmoothTime);

        if (displaySlider) { displaySlider.value = fogLevel; }
    }

    private void OnChangeFogLevel(float change) {
        fogLevel += change;
        fogLevelTarget = fogLevel;
    }

    private void OnChangeFogLevelSmooth(float change) {
        fogLevelTarget += change;
    }
}
