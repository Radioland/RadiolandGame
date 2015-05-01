using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class LevelSelectController : MonoBehaviour
{
    [Header("Scene Objects")]
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private RadioControl radioControl;
    [SerializeField] private GameObject levelSelectEntriesParent;
    [SerializeField] private SimpleMenu simpleMenu;
    [Header("Display Objects")]
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Image largePreviewImage;

    private LevelSelectEntry[] levelSelectEntries;
    private int lastStrongestSignalId;

    private void Awake() {
        levelSelectEntries = levelSelectEntriesParent.GetComponentsInChildren<LevelSelectEntry>();
    }

    private void Start() {
        lastStrongestSignalId = radioControl.strongestSignalStation.id;

        UpdateDisplay();
    }

    private void Update() {
        if (!radioControl.strongestSignalStation.StrongSignal()) {
            return;
        }

        if (lastStrongestSignalId != radioControl.strongestSignalStation.id) {
            lastStrongestSignalId = radioControl.strongestSignalStation.id;
            UpdateDisplay();
        }
    }

    private void CheckRadio() {
        lastStrongestSignalId = radioControl.strongestSignalStation.id;
    }

    private void UpdateDisplay() {
        LevelSelectEntry entry = levelSelectEntries[lastStrongestSignalId-1];

        eventSystem.SetSelectedGameObject(entry.gameObject);

        largePreviewImage.sprite = entry.largePreview;
        titleText.text = entry.levelName;
        descriptionText.text = entry.levelDescription;
    }
}
