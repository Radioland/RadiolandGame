using UnityEngine;
using System.Collections;

public class PowerupManager : MonoBehaviour
{
    [SerializeField] private Renderer powerupReadyGlow;
    [SerializeField] private string glowColorName = "_TintColor";
    [SerializeField] private ParticleSystem usePowerupParticles;
    [SerializeField] private EffectManager usePowerupEffects;

    [HideInInspector] public Powerup[] powerups;

    [SerializeField] private float m_energy;
    public float energy {
        get { return m_energy; }
        set { m_energy = Mathf.Clamp(value, 0.0f, 1.0f); }
    }
    public bool IsFullEnergy() { return m_energy >= 0.998f; }

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

        // Show powerupReadyGlow if a powerup is ready.
        bool powerupReady = false;
        foreach (Powerup powerup in powerups) {
            if (powerup.SufficientResources()) {
                if (!powerupReadyGlow.enabled) {
                    powerupReadyGlow.enabled = true;
                    powerupReadyGlow.material.SetColor(glowColorName, powerup.color);
                }
                usePowerupParticles.startColor = powerup.color;
                powerupReady = true;
            }
        }
        if (!powerupReady) {
            powerupReadyGlow.enabled = false;
        }
    }

    public bool CanUsePowerup(Powerup testPowerup) {
        return (testPowerup.energyCost <= energy);
    }

    public void SetActivePowerup(Powerup newActivePowerup) {
        if (activePowerup) {
            activePowerup.EndPowerup();
        }
        activePowerup = newActivePowerup;

        if (usePowerupEffects) {
            usePowerupEffects.StartEvent();
        }
    }

    public void EndPowerup() {
        activePowerup = null;

        if (usePowerupEffects) {
            usePowerupEffects.StopEvent();
        }
    }
}
