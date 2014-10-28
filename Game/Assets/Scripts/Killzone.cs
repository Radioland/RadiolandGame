using UnityEngine;
using System.Collections;

public class Killzone : MonoBehaviour
{
    private GameController gameController;

    void Awake() {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        gameController = gameControllerObject.GetComponent<GameController>();
    }

    void Start() {

    }

    void Update() {

    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            gameController.KillPlayer();
        }
    }
}
