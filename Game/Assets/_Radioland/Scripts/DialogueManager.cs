using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private Text justText;
    // TODO: Text + Image layout

    // TODO: adjust camera look target when in dialogue (y from 2.5 to 0.5-1?)

    private Dialogue currentDialogue;

    private void Awake() {

    }

    private void Start() {

    }

    private void Update() {

    }

    public bool CanStartDialogue() {
        return currentDialogue == null;
    }

    public void StartDialogue(Dialogue nextDialogue) {
        if (!CanStartDialogue()) {
            Debug.LogWarning("Tried to start dialogue when not able to.");
        }

        currentDialogue = nextDialogue;
    }

    public void SetMessage(string messageText) {
        dialogueUI.SetActive(true);

        justText.text = messageText;
        justText.enabled = true;
    }
}
