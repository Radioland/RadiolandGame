using UnityEngine;
using System.Collections;

public class IceSurface : MonoBehaviour
{
    [SerializeField] private float newGroundSmoothDampTime = 0.6f;

    private void Awake() {

    }

    private void Start() {

    }

    private void Update() {

    }

    public void OnTriggerEnter(Collider other) {
        CharacterMovement characterMovement = other.GetComponent<CharacterMovement>();
        if (characterMovement) {
            characterMovement.SetGroundSmoothDampTime(newGroundSmoothDampTime);
        }
    }

    public void OnTriggerStay(Collider other) {
        CharacterMovement characterMovement = other.GetComponent<CharacterMovement>();
        if (characterMovement) {
            characterMovement.SetGroundSmoothDampTime(newGroundSmoothDampTime);
        }
    }

    public void OnTriggerExit(Collider other) {
        CharacterMovement characterMovement = other.GetComponent<CharacterMovement>();
        if (characterMovement) {
            characterMovement.ResetGroundSmoothDampTime();
        }
    }
}
