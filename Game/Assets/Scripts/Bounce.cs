using UnityEngine;
using System.Collections;

public class Bounce : MonoBehaviour
{
    [SerializeField] private float minimumSpeed = 10.0f;

    private CharacterMovement characterMovement;
    private float firstBounceSpeed;
    private bool bouncing;

    void Awake() {
        characterMovement = gameObject.GetComponent<CharacterMovement>();

        bouncing = false;
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
                ApplyBounce(verticalSpeed, bouncePlatform);
            } else {
                bouncing = false;
            }
        }
    }

    private void ApplyBounce(float verticalSpeed, BouncePlatform bouncePlatform) {
        if (!bouncing) {
            firstBounceSpeed = verticalSpeed;
        }

        if (-firstBounceSpeed > minimumSpeed) {
            characterMovement.Bounce(-firstBounceSpeed * bouncePlatform.elasticity);
            bouncing = true;
        } else {
            bouncing = false;
        }
    }
}
