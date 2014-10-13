using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CooldownManager : MonoBehaviour
{
    PowerupManager powerupScript;
    Text text;

    void Awake() {
        powerupScript = GameObject.Find("TestCharacter").GetComponent<PowerupManager>();
        text = GetComponent<Text>();
    }

    void Update() {
        float maxRemainingTime = 0.0f;
        foreach (Powerup powerup in powerupScript.powerups) {
            maxRemainingTime = Mathf.Max(maxRemainingTime, powerup.remainingTime);
        }
        text.text = maxRemainingTime.ToString();
    }
}
