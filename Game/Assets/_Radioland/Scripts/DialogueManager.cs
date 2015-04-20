using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private CanvasGroup dialogueCanvasGroup;
    [SerializeField] private Text justText;
    // TODO: Text + Image layout

    [Header("Fade in/out")]
    [SerializeField] private Interpolate.EaseType easeType = Interpolate.EaseType.EaseInOutQuad;
    [SerializeField] private float fadeInTime = 1f;
    [SerializeField] private float fadeOutTime = 1f;

    // TODO: adjust camera look target when in dialogue (y from 2.5 to 0.5-1?)

    private Dialogue currentDialogue;
    private Interpolate.Function easeFunction;

    private void Awake() {
        easeFunction = Interpolate.Ease(easeType);
        dialogueCanvasGroup.alpha = 0f;

        dialogueUI.SetActive(true);
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

        StartCoroutine("FadeIn");
    }

    public void EndDialogue() {
        if (!currentDialogue) { return; }

        currentDialogue = null;

        justText.text = "";
        justText.enabled = false;

        StartCoroutine("FadeOut");
    }

    public void SetMessage(string messageText) {
        justText.text = messageText;
        justText.enabled = true;
    }

    private IEnumerator FadeIn() {
        StopCoroutine("FadeOut");

        float startTime = Time.time;

        while (Time.time - startTime < fadeOutTime) {
            dialogueCanvasGroup.alpha = easeFunction(0f, 1f, Time.time - startTime, fadeInTime);
            yield return null;
        }
        dialogueCanvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut() {
        StopCoroutine("FadeIn");

        float startTime = Time.time;

        while (Time.time - startTime < fadeOutTime) {
            dialogueCanvasGroup.alpha = easeFunction(1f, -1f, Time.time - startTime, fadeOutTime);
            yield return null;
        }
        dialogueCanvasGroup.alpha = 0f;
    }
}
