using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CooldownManager : MonoBehaviour
{
    private PowerupManager powerupScript;
    private Text text;

    private void Awake() {
        powerupScript = GameObject.FindWithTag("Player").GetComponent<PowerupManager>();
        text = GetComponent<Text>();
    }

    private void Update() {
        float maxRemainingTime = 0.0f;
        foreach (Powerup powerup in powerupScript.powerups) {
            maxRemainingTime = Mathf.Max(maxRemainingTime, powerup.remainingTime);
        }
        text.text = maxRemainingTime.ToString();
    }
}
