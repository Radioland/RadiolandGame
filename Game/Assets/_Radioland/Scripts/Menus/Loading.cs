using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Loading : MonoBehaviour
{
    [SerializeField] private GameObject loadingDisplay;
    [SerializeField] private Slider slider;

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
        loadingDisplay.SetActive(true);

        AsyncOperation async = Application.LoadLevelAsync(level);
        while(!async.isDone) {
            slider.value = async.progress;

            yield return null;
        }

        Destroy(gameObject);
    }
}
