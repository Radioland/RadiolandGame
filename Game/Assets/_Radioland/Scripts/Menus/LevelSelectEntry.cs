using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelSelectEntry : MonoBehaviour
{
    public Sprite largePreview;
    public string levelName;
    public string levelDescription;

    private void Reset() {
        Image image = gameObject.GetComponent<Image>();
        if (image) {
            largePreview = image.sprite;
        }
    }

    private void Awake() {
        Text childText = gameObject.GetComponentInChildren<Text>();
        if (childText) {
            childText.text = levelName;
        }
    }

    private void Start() {

    }

    private void Update() {

    }
}
