using UnityEngine;
using System.Collections;

public class Kick : MonoBehaviour
{
    [SerializeField] private Vector3 kickDisplacement = new Vector3(0f, 0.2f, 1f);
    [SerializeField] private float kickDuration = 1f;
    [SerializeField] private GameObject kickObject;

    private CharacterMovement characterMovement;
    private CharacterController characterController;
    private Animator animator;
    private int kickHash;
    private int kickingStateHash;

    private bool kicking;

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
        if (Input.GetButtonDown("Kick") && characterMovement.controllable) {
            animator.SetTrigger(kickHash);
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

            // TODO: curve or some delay for this?

            Vector3 displacement = transform.right * kickDisplacement.x +
                                   transform.up * kickDisplacement.y +
                                   transform.forward * kickDisplacement.z;
            characterController.Move(displacement * Time.deltaTime / kickDuration);
        }
    }

    private void StartKicking() {
        kicking = true;
        kickObject.SetActive(true);
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
