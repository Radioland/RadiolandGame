using UnityEngine;
using System.Collections;

public class PlatformMoveAndRotate : MonoBehaviour
{
    private float rotationAmount = 200;

    private float rotationSpeed = 200;

    [SerializeField] private float reverser = -1;

    private void Awake() {
        Messenger.AddListener("JumpStarted", OnJumpStarted);
    }

    private void Update() {
        if (rotationAmount < 180) {
            rotationAmount += Time.deltaTime * rotationSpeed;
            transform.Rotate(Vector3.forward * Time.deltaTime * rotationSpeed * reverser);
            //This is done to keep the rotations fixed at the 90 degree intervals because
            //Time.deltaTime * rotationSpeed may end at 92.2 degrees for example
            if (rotationAmount > 180) {
                if (transform.rotation.eulerAngles.z > 170 && transform.rotation.eulerAngles.z < 190) {
                    transform.localEulerAngles = new Vector3(0,0,180);
                }
                else if (transform.rotation.eulerAngles.z > 350 || transform.rotation.eulerAngles.z < 10) {
                    transform.localEulerAngles = new Vector3(0,0,0);
                }
            }
        }
    }

    private void OnJumpStarted() {
        rotationAmount = 0;
        reverser *= -1;
    }
}
