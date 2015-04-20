using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private CanvasGroup dialogueCanvasGroup;
    [SerializeField] private Text justText;
    [SerializeField] private Text withImageText;
    [SerializeField] private Image withImageImage;

    [Header("Fade in/out")]
    [SerializeField] private Interpolate.EaseType easeType = Interpolate.EaseType.EaseInOutQuad;
    [SerializeField] private float fadeInTime = 1f;
    [SerializeField] private float fadeOutTime = 1f;

    private Dialogue currentDialogue;
    private Interpolate.Function easeFunction;

    private CharacterMovement characterMovement;
    private CameraControl cameraControl;
    private Transform cameraTarget;
    private Transform playerTransform;

    private void Awake() {
        easeFunction = Interpolate.Ease(easeType);

        dialogueCanvasGroup.alpha = 0f;

        DisableJustTextUI();
        DisableWithImageUI();

        dialogueUI.SetActive(true);

        GameObject playerObject = GameObject.FindWithTag("Player");
        playerTransform = playerObject.transform;
        cameraControl = playerObject.GetComponent<CameraControl>();
        characterMovement = playerObject.GetComponent<CharacterMovement>();

        cameraTarget = new GameObject("Dialogue Camera Target").transform;
    }

    private void Start() {
        Messenger.AddListener("RespawnStarted", OnRespawnStarted);
    }

    private void Update() {
        if (currentDialogue) {
            characterMovement.SetControllable(false);
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

        if (look) {
            // Camera look between the player and the source of the dialogue.
            Vector3 cameraPosition = Vector3.Lerp(nextDialogue.transform.position,
                                                  playerTransform.position, 0.5f);
            cameraPosition.y -= 1f;
            cameraTarget.position = cameraPosition;
            cameraControl.OverrideTarget(cameraTarget.transform, 12f);

            // Player look at the source of the dialogue.
            characterMovement.LookAt(nextDialogue.transform);
        }

        StartCoroutine("FadeIn");
    }

    public void EndDialogue() {
        if (!currentDialogue) { return; }

        currentDialogue = null;

        cameraControl.ResetTarget();

        characterMovement.SetControllable(true);

        StartCoroutine("FadeOut");
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

        DisableJustTextUI();
        DisableWithImageUI();
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

    private void OnRespawnStarted() {
        // Ensure that we cleanup if the player dies!
        EndDialogue();
    }
}
