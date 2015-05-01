using UnityEngine;
using System.Collections;

public class SimpleMenu : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;

    private bool alreadyLoading;
    private bool alreadyWaitingToLoad;

    private void Awake() {
        alreadyLoading = false;
        alreadyWaitingToLoad = false;
    }

    private void Start() {

    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Exit();
        }

        #if !UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Confined;
        #endif
        Cursor.visible = true;

        HandleCheats();
    }

    private void HandleCheats() {
        if (Input.GetKeyDown(KeyCode.Equals)) {
            Application.LoadLevel(Application.loadedLevel + 1);
        }
        if (Input.GetKeyDown(KeyCode.Minus)) {
            Application.LoadLevel(Application.loadedLevel - 1);
        }
    }

    public void LoadLevel(int level) {
        if (alreadyLoading) { return; }
        alreadyLoading = true;

        Time.timeScale = 1.0f;
        AudioListener.pause = false;

        GameObject loadingScreenObject = (GameObject) Instantiate(loadingScreen);
        Loading loading = loadingScreenObject.GetComponent<Loading>();
        if (loading) {
            loading.LoadLevel(level);
        } else {
            Application.LoadLevel(level);
        }
    }

    public void LoadNextLevel() {
        LoadLevel(Application.loadedLevel + 1);
    }

    public void LoadNextLevel(float delay) {
        if (alreadyWaitingToLoad) { return; }
        alreadyWaitingToLoad = true;
        Invoke("LoadNextLevel", delay);
    }

    public void Exit() {
        Application.Quit();
    }
}
