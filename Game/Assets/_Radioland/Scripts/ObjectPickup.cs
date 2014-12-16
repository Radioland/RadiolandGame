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

    void Awake() {
        animator = gameObject.GetComponentInChildren<Animator>();
        pickupTriggerHash = Animator.StringToHash("Pickup");
        pickupStateHash = Animator.StringToHash("Base Layer.Pickup");
        pickingUp = false;

        characterMovement = gameObject.GetComponent<CharacterMovement>();
        //objectiveGUI = GameObject.Find("ObjectiveUI");
    }

    void Start() {

    }

    void OnTriggerEnter(Collider c) {
        if (c.gameObject.tag == "pickupable") {
            if (pickupEffects) {
                pickupEffects.StartEvent();
            }

            animator.SetTrigger(pickupTriggerHash);

            //Destroy(c.gameObject);
            //objectiveGUI.GetComponent<ObjectiveGUI>().IncrementTargets();
        }
    }
    
    void Update() {
        bool inPickupState = animator.GetCurrentAnimatorStateInfo(0).nameHash == pickupStateHash;

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
