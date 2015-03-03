using UnityEngine;
using System.Collections;

public class GDCStartMenu : MonoBehaviour
{
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
        Application.LoadLevel(level);
    }

    public void Exit() {
        Application.Quit();
    }
}
