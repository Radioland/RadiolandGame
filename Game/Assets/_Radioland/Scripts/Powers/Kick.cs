using UnityEngine;
using System.Collections;

public class Kick : MonoBehaviour
{
    [SerializeField] private Vector3 kickDisplacement = new Vector3(0f, 0.2f, 1f);
    [SerializeField] private float kickDuration = 1f;
    [SerializeField] private AnimationCurve displacementX;
    [SerializeField] private AnimationCurve displacementY;
    [SerializeField] private AnimationCurve displacementZ;
    [SerializeField] private GameObject kickObject;

    private CharacterMovement characterMovement;
    private CharacterController characterController;
    private Animator animator;
    private int kickHash;
    private int kickingStateHash;

    private bool kicking;
    private float kickTime;

    private void Awake() {
        characterMovement = gameObject.GetComponent<CharacterMovement>();
        characterController = gameObject.GetComponent<CharacterController>();

        // Fetch animator properties.
        animator = gameObject.GetComponentInChildren<Animator>();
        if (!animator) {
            Debug.LogWarning("No animator found on " + transform.GetPath());
        }
        kickHash = Animator.StringToHash("Kick");
        kickingStateHash = Animator.StringToHash("Base Layer.Kicking");

        kicking = false;
    }

    private void Start() {
        Messenger.AddListener("RespawnStarted", OnRespawnStarted);
    }

    private void Update() {
        if (Input.GetButtonDown("Kick")) {
            Messenger.Broadcast("InputReceived");
            Messenger.Broadcast("KickTriggered");

            if (characterMovement.controllable) {
                animator.SetTrigger(kickHash);
            }
        }

        bool inKickState = animator.GetCurrentAnimatorStateInfo(0).fullPathHash == kickingStateHash;
        if (inKickState && !kicking) {
            StartKicking();
        } else if (!inKickState && kicking) {
            StopKicking();
        }

        if (kicking) {
            characterMovement.DisableGravity();
            characterMovement.SetControllable(false);

            kickTime += Time.deltaTime / kickDuration;

            Vector3 displacement = transform.right   * kickDisplacement.x * displacementX.Evaluate(kickTime) +
                                   transform.up      * kickDisplacement.y * displacementY.Evaluate(kickTime) +
                                   transform.forward * kickDisplacement.z * displacementZ.Evaluate(kickTime);

            characterController.Move(displacement * Time.deltaTime / kickDuration);
        }
    }

    private void StartKicking() {
        kicking = true;
        kickTime = 0f;
        kickObject.SetActive(true);
        characterMovement.Stop();
    }

    private void StopKicking() {
        kickObject.SetActive(false);
        kicking = false;
        characterMovement.SetControllable(true);
        characterMovement.EnableGravity();
    }

    private void OnRespawnStarted() {
        StopKicking();
    }
}
