using UnityEngine;
using System.Collections;

public class PlatformRotation : MonoBehaviour {
    private float rotationAmount = 100;

    //100 is a decent value for this one
    [SerializeField] private float rotationSpeed;
    //Set this to either 90, 180, 270
    [SerializeField] private float rotationDegrees;

    private void Awake() {
        Messenger.AddListener("JumpStarted", OnJumpStarted);
    }

    private void Update() {
        if (rotationAmount < rotationDegrees) {
            rotationAmount += Time.deltaTime * rotationSpeed;
            transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);
            //This is done to keep the rotations fixed at the 90 degree intervals because
            //Time.deltaTime * rotationSpeed may end at 92.2 degrees for example
            if (rotationAmount > rotationDegrees) {
                if (transform.rotation.eulerAngles.y > 80 && transform.rotation.eulerAngles.y < 100) {
                    transform.localEulerAngles = new Vector3(0,90,0);
                }
                else if (transform.rotation.eulerAngles.y > 170 && transform.rotation.eulerAngles.y < 190) {
                    transform.localEulerAngles = new Vector3(0,180,0);
                }
                else if (transform.rotation.eulerAngles.y > 260 && transform.rotation.eulerAngles.y < 280) {
                    transform.localEulerAngles = new Vector3(0,270,0);
                }
                else if (transform.rotation.eulerAngles.y > 350 || transform.rotation.eulerAngles.y < 10) {
                    transform.localEulerAngles = new Vector3(0,0,0);
                }
            }
        }
    }

    public void OnJumpStarted() {
        rotationAmount = 0;
    }

}
