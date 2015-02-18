using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private GameObject dialogueObject;
    [SerializeField] private Text uiText;
    [SerializeField] private List<TextAsset> messages;
    [SerializeField] private EffectManager allMessagesEffects;
    [SerializeField] private List<EffectManager> specificMessageEffects;

    private bool visible;
    private int currentMessage;
    private const float nextMessageCooldown = 0.5f;
    private float lastNextMessageTime;

    private void Awake() {
        ClearMessage();

        lastNextMessageTime = -1000f;
    }

    private void Start() {

    }

    private void Update() {

    }

    public void NextMessage() {
        if (messages.Count == 0) { return; }

        lastNextMessageTime = Time.time;

        if (visible) {
            if (allMessagesEffects) { allMessagesEffects.StopEvent(); }
            if (specificMessageEffects.Count > currentMessage &&
                specificMessageEffects[currentMessage]) {
                specificMessageEffects[currentMessage].StopEvent();
            }

            currentMessage++;
        } else {
            visible = true;
            dialogueObject.SetActive(true);
        }

        if (currentMessage < messages.Count) {
            uiText.text = messages[currentMessage].text;

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
        currentMessage = 0;
        uiText.text = "";
        visible = false;
        dialogueObject.SetActive(false);
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
