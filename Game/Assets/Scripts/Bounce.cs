using UnityEngine;
using System.Collections;

public class Bounce : MonoBehaviour
{
    private CharacterMovement characterMovement;

    void Awake() {
        characterMovement = gameObject.GetComponent<CharacterMovement>();
    }

    void Start() {

    }

    void Update() {

    }
    
    // Called via SendMessage in CharacterMovement.
    private void Grounded(float verticalSpeed) {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.2f)) {
            BouncePlatform bouncePlatform = hit.collider.gameObject.GetComponent<BouncePlatform>();
            if (bouncePlatform) {
                characterMovement.Bounce(-verticalSpeed * bouncePlatform.elasticity);
            }
        }
    }
}
