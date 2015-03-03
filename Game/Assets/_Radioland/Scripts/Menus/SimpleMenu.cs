using UnityEngine;
using System.Collections;

public class SimpleMenu : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;

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

    public void Exit() {
        Application.Quit();
    }
}
