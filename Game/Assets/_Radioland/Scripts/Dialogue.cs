using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private List<TextAsset> messageFiles;
    [Header("Dialogue Effects")]
    [SerializeField] private EffectManager allMessagesEffects;
    [SerializeField] private List<EffectManager> specificMessageEffects;

    private bool visible;
    private int currentMessage = 0;
    private const float nextMessageCooldown = 0.5f;
    private float lastNextMessageTime;
    private int messageCount;

    private DialogueManager dialogueManager;

    private void Awake() {
        lastNextMessageTime = -1000f;

        messageCount = messageFiles.Count;

        GameObject gameController = GameObject.FindWithTag("GameController");
        dialogueManager = gameController.GetComponent<DialogueManager>();
    }

    private void Start() {

    }

    private void Update() {

    }

    public void NextMessage() {
        if (messageCount == 0) { return; }

        lastNextMessageTime = Time.time;

        if (!visible) {
            if (!dialogueManager.CanStartDialogue()) { return; }

            dialogueManager.StartDialogue(this);
            visible = true;
        } else {
            if (allMessagesEffects) { allMessagesEffects.StopEvent(); }
            if (specificMessageEffects.Count > currentMessage &&
                specificMessageEffects[currentMessage]) {
                specificMessageEffects[currentMessage].StopEvent();
            }

            currentMessage++;
        }

        if (currentMessage < messageCount) {
            dialogueManager.SetMessage(messageFiles[currentMessage].text);

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
        if (!visible) { return; }

        visible = false;

        if (allMessagesEffects) { allMessagesEffects.StopEvent(); }
        if (specificMessageEffects.Count > currentMessage &&
            specificMessageEffects[currentMessage]) {
            specificMessageEffects[currentMessage].StopEvent();
        }

        currentMessage = 0;

        dialogueManager.EndDialogue();
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
