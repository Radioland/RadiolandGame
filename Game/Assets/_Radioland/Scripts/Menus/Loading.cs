﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Loading : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private GameObject loadingDisplay;
    [SerializeField] private Slider slider;
    [SerializeField] private Image overrideBackgroundImage;
    [SerializeField] private Text messageText;
    [SerializeField] private Text submitToContinueText;
    [SerializeField] private Text loadingText;
    [SerializeField] private Text finalizingLoadText;

    // List of background images and text strings
    [Header("Customize Per Level")]
    [SerializeField] private List<Sprite> backgroundImages;
    [SerializeField] private List<string> messages;
    [SerializeField] private List<bool> waitForSubmit;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        loadingDisplay.SetActive(false);
    }

    private void Start() {

    }

    private void Update() {

    }

    public void LoadLevel(int level) {
        Time.timeScale = 1.0f;
        AudioListener.pause = false;
        StartCoroutine(DisplayLoadingScreen(level));
    }

    private IEnumerator DisplayLoadingScreen(int level) {
        if (level < backgroundImages.Count && backgroundImages[level]) {
            overrideBackgroundImage.sprite = backgroundImages[level];
            overrideBackgroundImage.enabled = true;
        } else {
            overrideBackgroundImage.enabled = false;
        }

        if (level < messages.Count && messages[level].Length > 0) {
            messageText.text = messages[level];
            messageText.enabled = true;
        } else {
            messageText.enabled = false;
        }

        finalizingLoadText.enabled = false;

        loadingDisplay.SetActive(true);

        AsyncOperation async = Application.LoadLevelAsync(level);

        bool waitingForSubmit = (level < waitForSubmit.Count && waitForSubmit[level]);

        while(!async.isDone) {
            slider.value = Mathf.Clamp(async.progress, 0f, 0.8f);
            Time.timeScale = 0f;
            yield return null;
        }

        slider.value = 1f;

        if (waitingForSubmit) {
            submitToContinueText.enabled = true;
            loadingText.enabled = false;

            while (!Input.GetButtonDown("SubmitAny")) {
                yield return null;
            }
        }

        Time.timeScale = 1f;
        Destroy(gameObject);
    }
}
