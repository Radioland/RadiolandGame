using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private Text justText;
    [SerializeField] private Text withImageText;
    [SerializeField] private Image withImageImage;

    [Header("Fade in/out")]
    [SerializeField] private CanvasGroup activeDialogueCanvasGroup;
    [SerializeField] private CanvasGroup dialogueAvailableCanvasGroup;
    [SerializeField] private Interpolate.EaseType easeType = Interpolate.EaseType.EaseInOutQuad;
    [SerializeField] private float fadeInTime = 1f;
    [SerializeField] private float fadeOutTime = 1f;

    private Dialogue currentDialogue;
    private HashSet<int> availableIds;

    private Interpolate.Function easeFunction;

    private CharacterMovement characterMovement;
    private CameraControl cameraControl;
    private Transform cameraTarget;
    private Transform playerTransform;
    private bool looking;

    private void Awake() {
        availableIds = new HashSet<int>();

        easeFunction = Interpolate.Ease(easeType);

        activeDialogueCanvasGroup.alpha = 0f;
        dialogueAvailableCanvasGroup.alpha = 0f;

        DisableJustTextUI();
        DisableWithImageUI();

        dialogueUI.SetActive(true);

        GameObject playerObject = GameObject.FindWithTag("Player");
        playerTransform = playerObject.transform;
        cameraControl = playerObject.GetComponent<CameraControl>();
        characterMovement = playerObject.GetComponent<CharacterMovement>();

        cameraTarget = new GameObject("Dialogue Camera Target").transform;
        looking = false;
    }

    private void Start() {
        Messenger.AddListener("RespawnStarted", OnRespawnStarted);
    }

    private void Update() {
        if (currentDialogue) {
            characterMovement.SetControllable(false);

            if (looking) {
                // Camera look between the player and the source of the dialogue.
                cameraTarget.position = Vector3.Lerp(currentDialogue.transform.position,
                                                     playerTransform.position, 0.5f);
                cameraControl.OverrideTarget(cameraTarget.transform, 12f);
            }
        }
    }

    public bool CanStartDialogue() {
        return currentDialogue == null;
    }

    public void StartDialogue(Dialogue nextDialogue, bool look=true) {
        if (!CanStartDialogue()) {
            Debug.LogWarning("Tried to start dialogue when not able to.");
        }

        currentDialogue = nextDialogue;

        looking = look;

        if (look) {
            // Player look at the source of the dialogue.
            characterMovement.LookAt(nextDialogue.transform);
        }

        StartCoroutine("FadeInActiveDialogue");
    }

    public void EndDialogue() {
        if (!currentDialogue) { return; }

        currentDialogue = null;

        cameraControl.ResetTarget();

        characterMovement.SetControllable(true);

        StartCoroutine("FadeOutActiveDialogue");
    }

    public void SetMessage(string messageText) {
        DisableWithImageUI();

        // Enable the UI with just text.
        justText.text = messageText;
        justText.enabled = true;
    }

    public void SetMessage(string messageText, Sprite image) {
        DisableJustTextUI();

        // Enable the UI with text and image.
        withImageText.text = messageText;
        withImageImage.sprite = image;

        withImageText.enabled = true;
        withImageImage.enabled = true;
    }

    private IEnumerator FadeInActiveDialogue() {
        StopCoroutine("FadeOutActiveDialogue");

        float startTime = Time.time;
        while (Time.time - startTime < fadeOutTime) {
            activeDialogueCanvasGroup.alpha = easeFunction(0f, 1f, Time.time - startTime, fadeInTime);
            yield return null;
        }
        activeDialogueCanvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOutActiveDialogue() {
        StopCoroutine("FadeInActiveDialogue");

        float startTime = Time.time;
        while (Time.time - startTime < fadeOutTime) {
            activeDialogueCanvasGroup.alpha = easeFunction(1f, -1f, Time.time - startTime, fadeOutTime);
            yield return null;
        }
        activeDialogueCanvasGroup.alpha = 0f;

        DisableJustTextUI();
        DisableWithImageUI();
    }

    private IEnumerator FadeInDialogueAvailable() {
        StopCoroutine("FadeOutDialogueAvailable");

        float startTime = Time.time;
        while (Time.time - startTime < fadeOutTime) {
            dialogueAvailableCanvasGroup.alpha = easeFunction(0f, 1f, Time.time - startTime, fadeInTime);
            yield return null;
        }
        dialogueAvailableCanvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOutDialogueAvailable() {
        StopCoroutine("FadeInDialogueAvailable");

        float startTime = Time.time;
        while (Time.time - startTime < fadeOutTime) {
            dialogueAvailableCanvasGroup.alpha = easeFunction(1f, -1f, Time.time - startTime, fadeOutTime);
            yield return null;
        }
        dialogueAvailableCanvasGroup.alpha = 0f;
    }

    private void DisableJustTextUI() {
        justText.text = "";
        justText.enabled = false;
    }

    private void DisableWithImageUI() {
        withImageImage.sprite = null;
        withImageText.text = "";

        withImageText.enabled = false;
        withImageImage.enabled = false;
    }

    public void SignalDialogueAvailable(int id) {
        if (availableIds.Count == 0) {
            StartCoroutine("FadeInDialogueAvailable");
        }

        availableIds.Add(id);
    }

    public void SignalDialogueNoLongerAvailable(int id) {
        availableIds.Remove(id);

        if (availableIds.Count == 0) {
            StartCoroutine("FadeOutDialogueAvailable");
        }
    }

    private void OnRespawnStarted() {
        // Ensure that we cleanup if the player dies.
        EndDialogue();
    }
}
