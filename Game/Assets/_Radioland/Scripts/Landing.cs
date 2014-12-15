using UnityEngine;
using System.Collections;

public class Landing : MonoBehaviour
{
    [SerializeField] private EffectManager landingEffects;
    [SerializeField] private ParticleSystem landingParticles;
    [Tooltip("Minimum landing speed to trigger effects, affects interpolation.")]
    [SerializeField] private float minEffectSpeed = 12.0f;
    [Tooltip("Maximum landing speed for interpolation purposes, effects still trigger past this.")]
    [SerializeField] private float maxEffectSpeed = 30.0f;
    [SerializeField] private float minStartSize = 0.05f;
    [SerializeField] private float maxStartSize = 2.0f;
    [SerializeField] private float minStartSpeed = 0.2f;
    [SerializeField] private float maxStartSpeed = 3.0f;

    void Awake() {

    }
    
    void Start() {
        
    }
    
    void Update() {
        
    }
    
    // Called via SendMessage in CharacterMovement.
    void Grounded(float verticalSpeed) {
        if (landingEffects && -verticalSpeed > minEffectSpeed) {
            float t = Mathf.InverseLerp(minEffectSpeed, maxEffectSpeed, -verticalSpeed);

            landingParticles.startSize = Mathf.Lerp(minStartSize, maxStartSize, t);
            landingParticles.startSpeed = Mathf.Lerp(minStartSpeed, maxStartSpeed, t);

            landingEffects.StartEvent();
        }
    }
}
