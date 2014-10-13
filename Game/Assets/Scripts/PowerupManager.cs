using UnityEngine;
using System.Collections;

public class PowerupManager : MonoBehaviour
{
    [HideInInspector] public Powerup[] powerups;

    void Awake() {
        powerups = gameObject.GetComponents<Powerup>();
    }

    void Start() {

    }

    void Update() {

    }
}
