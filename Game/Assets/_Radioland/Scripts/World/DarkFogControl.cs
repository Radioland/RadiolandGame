using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DarkFogControl : MonoBehaviour
{
    public float fogLevel = 0f;

    [Header("Linked Objects")]
    [SerializeField] private GameObject groundFogParent;
    [SerializeField] private Slider displaySlider;

    [Header("Configuration")]
    [SerializeField] private float groundFogMinHeight = 0f;
    [SerializeField] private float groundFogMaxHeight = 50f;

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
        fogLevel = Mathf.Clamp01(fogLevel);

        if (displaySlider) { displaySlider.value = fogLevel; }

        if (groundFogParent) {
            float y = Mathf.Lerp(groundFogMinHeight, groundFogMaxHeight, fogLevel);
            groundFogParent.transform.position = new Vector3(0f, y, 0f);
        }
    }

    private void OnChangeFogLevel(float change) {
        fogLevel += change;
        fogLevelTarget = fogLevel;
    }

    private void OnChangeFogLevelSmooth(float change) {
        fogLevelTarget += change;
    }
}
