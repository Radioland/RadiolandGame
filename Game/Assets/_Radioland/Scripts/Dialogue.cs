using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Dialogue : MonoBehaviour
{
    private enum DialogueMode
    {
        Automatic,
        Manual
    }

    [SerializeField] [Tooltip("Typically a Canvas")] private GameObject dialogueRootObject;
    [SerializeField] private DialogueMode mode = DialogueMode.Automatic;
    [SerializeField] private bool startVisible = false;
    [Header("Automatic Setup")]
    [SerializeField] private Text uiText;
    [SerializeField] private List<TextAsset> messageFiles;
    [Header("Manual Setup")]
    [SerializeField] private List<GameObject> messageObjects;
    [Header("Dialogue Effects")]
    [SerializeField] private EffectManager allMessagesEffects;
    [SerializeField] private List<EffectManager> specificMessageEffects;

    private bool visible;
    private int currentMessage = 0;
    private const float nextMessageCooldown = 0.5f;
    private float lastNextMessageTime;
    private int messageCount;

    private void Awake() {
        if (startVisible) {
            visible = true;
            dialogueRootObject.SetActive(true);
        } else {
            ClearMessage();
        }

        lastNextMessageTime = -1000f;

        messageCount = mode == DialogueMode.Automatic ? messageFiles.Count : messageObjects.Count;
    }

    private void Start() {

    }

    private void Update() {

    }

    public void NextMessage() {
        if (messageCount == 0) { return; }

        lastNextMessageTime = Time.time;

        if (visible) {
            if (mode == DialogueMode.Manual) { messageObjects[currentMessage].SetActive(false); }

            if (allMessagesEffects) { allMessagesEffects.StopEvent(); }
            if (specificMessageEffects.Count > currentMessage &&
                specificMessageEffects[currentMessage]) {
                specificMessageEffects[currentMessage].StopEvent();
            }

            currentMessage++;
        } else {
            visible = true;
            dialogueRootObject.SetActive(true);
        }

        if (currentMessage < messageCount) {
            if (messageObjects.Count > 0) {
                messageObjects[currentMessage].SetActive(true);
            } else {
                uiText.text = messageFiles[currentMessage].text;
            }

            if (allMessagesEffects) { allMessagesEffects.StartEvent(); }
            if (specificMessageEffects.Count > currentMessage &&
                specificMessageEffects[currentMessage]) {
                specificMessageEffects[currentMessage].StartEvent();
            }
        } else {
            ClearMessage();
        }
    }

    public void ClearMessage() {
        if (mode == DialogueMode.Manual) {
            foreach (GameObject messageObject in messageObjects) {
                messageObject.SetActive(false);
            }
        }

        currentMessage = 0;

        if (mode == DialogueMode.Automatic) { uiText.text = ""; }
        visible = false;
        dialogueRootObject.SetActive(false);
    }

    public void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player") && Input.GetButtonDown("Dialogue") &&
            Time.time - lastNextMessageTime > nextMessageCooldown) {
            NextMessage();
        }
    }

    public void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            ClearMessage();
        }
    }
}
