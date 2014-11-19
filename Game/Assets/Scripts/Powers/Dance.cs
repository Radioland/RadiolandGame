using UnityEngine;
using System.Collections;

public class Dance : MonoBehaviour
{
    public bool dancing;
    [SerializeField] private float minDanceTime = 0.5f;

    private CharacterMovement characterMovement;
    private Animator animator;
    private int danceBoolHash;
    private int danceStateHash;
    private bool stopping;
    private float lastDanceTime;

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
    }

    void Start() {

    }

    void Update() {
        if (characterMovement.grounded && Input.GetButtonDown("Dance")) {
            dancing = true;
            stopping = false;
            lastDanceTime = Time.time;

            animator.SetBool(danceBoolHash, true);
        }

        bool inDanceState = animator.GetCurrentAnimatorStateInfo(0).nameHash == danceStateHash;

        // Wait at least minDanceTime before stopping the animation and restoring control.
        if (Time.time - lastDanceTime > minDanceTime && stopping) {
            if (inDanceState) {
                animator.SetBool(danceBoolHash, false);
            }

            if (!inDanceState) {
                characterMovement.SetControllable(true);
                stopping = false;
                dancing = false;
            }
        }

        if (dancing) {
            characterMovement.SetControllable(false);
        }
    }
    
    // Called via SendMessage in CharacterMovement.
    void InputReceived() {
        if (dancing) {
            stopping = true;
        }
    }

    // Called via SendMessage in CharacterMovement.
    void JumpTriggered() {
        // Jump immediately cancels dancing.
        if (dancing) {
            stopping = false;
            dancing = false;
            animator.SetBool(danceBoolHash, false);
            characterMovement.SetControllable(true);
        }
    }
}
