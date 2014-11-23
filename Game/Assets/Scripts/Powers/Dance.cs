using UnityEngine;
using System.Collections;

public class Dance : MonoBehaviour
{
    public bool dancing;
    [SerializeField] private float minDanceTime = 0.5f;
    [SerializeField] private float minInputStopTime = 0.5f;
    [SerializeField] private float inputPersistTime = 0.1f;
    [SerializeField] private EffectManager danceEffects;

    private CharacterMovement characterMovement;
    private Animator animator;
    private int danceBoolHash;
    private int danceStateHash;
    private bool stopping;
    private float lastDanceTime;
    private float lastInputTime;
    private float lastInputStartedTime;
    private bool stillReceivingInput;

    void Awake() {
        dancing = false;

        characterMovement = gameObject.GetComponent<CharacterMovement>();

        // Fetch animator properties.
        animator = gameObject.GetComponentInChildren<Animator>();
        if (!animator) {
            Debug.LogWarning("No animator found on " + transform.GetPath());
        }
        danceBoolHash = Animator.StringToHash("Dancing");
        danceStateHash = Animator.StringToHash("Base Layer.Dance");

        stopping = false;
        lastDanceTime = -1000.0f;
        lastInputTime = -1000.0f;
        lastInputStartedTime = -1000.0f;
    }

    void Start() {

    }

    void Update() {
        if (characterMovement.grounded && Input.GetButtonDown("Dance")) {
            dancing = true;
            stopping = false;
            lastDanceTime = Time.time;

            animator.SetBool(danceBoolHash, true);

            if (danceEffects) {
                danceEffects.StartEvent();
            }
        }

        bool inDanceState = animator.GetCurrentAnimatorStateInfo(0).nameHash == danceStateHash;

        // Wait at least minDanceTime before stopping the animation and restoring control.
        if (Time.time - lastDanceTime > minDanceTime && stopping) {
            if (inDanceState) {
                animator.SetBool(danceBoolHash, false);
            }

            if (!inDanceState) {
                StopDancing();
            }
        }

        if (dancing) {
            characterMovement.SetControllable(false);
        }
    }
    
    // Called via SendMessage in CharacterMovement.
    void InputReceived() {
        lastInputTime = Time.time;

        if (dancing && stillReceivingInput) {
            if (Time.time - lastInputStartedTime > minInputStopTime) {
                stopping = true;
            }
        } else {
            lastInputStartedTime = Time.time;
        }
        stillReceivingInput = true;
    }

    // Called via SendMessage in CharacterMovement.
    void NoMovementInput() {
        if (Time.time - lastInputTime > inputPersistTime) {
            stillReceivingInput = false;
        }
    }

    // Called via SendMessage in CharacterMovement.
    void JumpTriggered() {
        // Jump immediately cancels dancing.
        if (dancing) {
            StopDancing();
        }
    }

    void StopDancing() {
        animator.SetBool(danceBoolHash, false);
        characterMovement.SetControllable(true);
        stopping = false;
        dancing = false;

        if (danceEffects) {
            danceEffects.StopEvent();
        }
    }
}
