using UnityEngine;
using System.Collections;

public class PushedByWind : MonoBehaviour
{
    private CharacterController controller;
    private CharacterMovement characterMovement;

    private void Awake() {
        controller = gameObject.GetComponent<CharacterController>();
        characterMovement = gameObject.GetComponent<CharacterMovement>();
    }

    private void Start() {

    }

    private void Update() {

    }

    private void OnTriggerStay(Collider other) {
        WindPushArea windPushArea = other.gameObject.GetComponent<WindPushArea>();
        if (!windPushArea && other.gameObject.transform.parent) {
            windPushArea = other.gameObject.transform.parent.GetComponent<WindPushArea>();
        }

        if (windPushArea) {
            if (characterMovement) {
                if (characterMovement.grounded) {
                    characterMovement.AddVelocity(windPushArea.windVelocity / characterMovement.mass);
                } else {
                    characterMovement.SetVelocity(windPushArea.windVelocity / characterMovement.mass);
                }
            } else if (controller) {
                controller.Move(windPushArea.windVelocity * Time.deltaTime);
            } else {
                transform.position = transform.position + windPushArea.windVelocity * Time.deltaTime;
            }
        }
    }
}
