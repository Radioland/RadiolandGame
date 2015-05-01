using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class LevelSelectController : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Image largePreviewImage;

    private LevelSelectEntry[] levelSelectEntries;

    private GameObject lastSelectedGameObject;

    private void Awake() {
        levelSelectEntries = gameObject.GetComponentsInChildren<LevelSelectEntry>();
    }

    private void Start() {
        lastSelectedGameObject = eventSystem.currentSelectedGameObject;

        UpdateDisplay();
    }

    private void Update() {
        if (lastSelectedGameObject != eventSystem.currentSelectedGameObject) {
            lastSelectedGameObject = eventSystem.currentSelectedGameObject;

            UpdateDisplay();
        }
    }

    private void UpdateDisplay() {
        if (!lastSelectedGameObject) { return; }
        LevelSelectEntry entry = lastSelectedGameObject.GetComponent<LevelSelectEntry>();

        largePreviewImage.sprite = entry.largePreview;
        titleText.text = entry.levelName;
        descriptionText.text = entry.levelDescription;
    }
}
