using UnityEngine;
using System.Collections;

public class PushedByWind : MonoBehaviour
{
    private CharacterController controller;
    
    void Awake() {
        controller = gameObject.GetComponent<CharacterController>();
        
        if (!controller) {
            Debug.LogWarning(transform.GetPath() + " does not have a CharacterController.");
            this.enabled = false;
        }
    }

    void Start() {

    }

    void Update() {

    }
    
    void OnTriggerStay(Collider other) {
        WindPushArea windPushArea = other.gameObject.GetComponent<WindPushArea>();
        if (!windPushArea && other.gameObject.transform.parent) {
            windPushArea = other.gameObject.transform.parent.GetComponent<WindPushArea>();
        }
        if (windPushArea) {
            controller.Move(windPushArea.windVelocity * Time.deltaTime);
        }
    }
}
