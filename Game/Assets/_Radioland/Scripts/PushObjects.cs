using UnityEngine;
using System.Collections;

public class PushObjects : MonoBehaviour
{
    [SerializeField] private LayerMask pushLayers;

    private CharacterMovement characterMovement;

    private void Awake() {
        characterMovement = gameObject.GetComponent<CharacterMovement>();
    }

    private void Start() {

    }

    private void Update() {

    }

    private void OnControllerColliderHit(ControllerColliderHit controllerHit) {
        if ((1<<controllerHit.collider.gameObject.layer & pushLayers) != 0) {
            Rigidbody hitRigidbody = controllerHit.rigidbody;

            Vector3 pushDir = new Vector3(controllerHit.moveDirection.x, 0, controllerHit.moveDirection.y).normalized;
            Vector3 characterVelocity = characterMovement.GetVelocity();
            float massRatio = characterMovement.mass / hitRigidbody.mass;

            Vector3 pushForce = Vector3.Scale(pushDir, characterVelocity) * massRatio;
            hitRigidbody.AddForce(pushForce);
        }
    }
}
