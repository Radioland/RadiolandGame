using UnityEngine;
using System.Collections;

public class PowerupManager : MonoBehaviour
{
    [HideInInspector] public Powerup[] powerups;

    [SerializeField] private float m_energy;
    public float energy {
        get { return m_energy; }
        set { m_energy = Mathf.Clamp(value, 0.0f, 1.0f); }
    }

    private Powerup activePowerup; // Only allow one active powerup at a time.

    void Awake() {
        powerups = gameObject.GetComponents<Powerup>();

        foreach (Powerup powerup in powerups) {
            powerup.SetPowerupManager(this);
        }
    }

    void Start() {

    }

    void Update() {
        // Debug energy refill.
        if (Input.GetKeyDown(KeyCode.F)) {
            energy = 1.0f;
        }
    }

    public bool CanUsePowerup(Powerup testPowerup) {
        return (testPowerup.energyCost < energy);
    }

    public void SetActivePowerup(Powerup newActivePowerup) {
        if (activePowerup) {
            activePowerup.EndPowerup();
        }
        activePowerup = newActivePowerup;
    }

    public void EndPowerup() {
        activePowerup = null;
    }
}
