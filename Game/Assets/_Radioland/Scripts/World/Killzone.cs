using UnityEngine;
using System.Collections;

public class Killzone : MonoBehaviour
{
    private GameController gameController;

    private void Awake() {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        gameController = gameControllerObject.GetComponent<GameController>();
    }

    private void Start() {

    }

    private void Update() {

    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            gameController.KillPlayer();
        }
    }
}
