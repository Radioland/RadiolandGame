using UnityEngine;
using System.Collections;

public class ObjectPickup : MonoBehaviour {
    [SerializeField] private EffectManager pickupEffects;

    private Animator animator;
    private int pickupTriggerHash;
    private int pickupStateHash;
    private CharacterMovement characterMovement;
    private bool pickingUp;
    //private GameObject objectiveGUI;

    private void Awake() {
        animator = gameObject.GetComponentInChildren<Animator>();
        pickupTriggerHash = Animator.StringToHash("Pickup");
        pickupStateHash = Animator.StringToHash("Base Layer.Pickup");
        pickingUp = false;

        characterMovement = gameObject.GetComponent<CharacterMovement>();
    }

    private void Start() {

    }

    private void OnTriggerEnter(Collider c) {
        if (c.gameObject.tag == "pickupable") {
            if (pickupEffects) {
                pickupEffects.StartEvent();
            }

            animator.SetTrigger(pickupTriggerHash);
        }
    }

    private void Update() {
        bool inPickupState = animator.GetCurrentAnimatorStateInfo(0).fullPathHash == pickupStateHash;

        if (!pickingUp && inPickupState) {
            pickingUp = true;
        }

        if (pickingUp) {
            if (!inPickupState) {
                characterMovement.SetControllable(true);
                pickingUp = false;
                pickupEffects.StopEvent();
            } else {
                characterMovement.SetControllable(false);
            }
        }
    }

}
