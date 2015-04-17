using UnityEngine;
using System.Collections;

public class SimpleMenu : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;

    private bool alreadyLoading = false;

    private void Awake() {

    }

    private void Start() {

    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Exit();
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

    public void Exit() {
        Application.Quit();
    }
}
