using UnityEngine;
using System.Collections;

public class Gameover : MonoBehaviour {

    [SerializeField] private GameObject newGameButton;
    [SerializeField] private GameObject exitButton;
    [SerializeField] private GameObject checkBox;

    private RectTransform checkBoxTransform;

    private float startTime;

    [SerializeField] private int mouseIndex;

    private void Start() {
        startTime = Time.time;
        mouseIndex = 0;
        checkBoxTransform = checkBox.GetComponent<RectTransform>();
    }

    private void Update() {
        if (Input.GetButtonDown(buttonName:"Submit")) {
            if (mouseIndex == 0) {
                GoNewGame();
            }
            else {
                GoExit();
            }
        }
        if (mouseIndex == 0) {
            if (Input.GetAxisRaw("Vertical") != 0 && Time.time - startTime > 0.5f) {
                mouseIndex = 1;
                startTime = Time.time;
            }
            checkBoxTransform.anchorMin = new Vector2(0.48f,0.47f);
            checkBoxTransform.anchorMax = new Vector2(0.52f, 0.58f);
        }
        else {
            if (Input.GetAxisRaw("Vertical") != 0 && Time.time - startTime > 0.5f) {
                mouseIndex = 0;
                startTime = Time.time;
            }
            checkBoxTransform.anchorMin = new Vector2(0.48f,0.27f);
            checkBoxTransform.anchorMax = new Vector2(0.52f, 0.38f);
        }
    }

    public void GoNewGame() {
        Application.LoadLevel(1);
    }

    public void GoExit() {
        Application.Quit();
    }
}
