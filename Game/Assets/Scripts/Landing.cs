using UnityEngine;
using System.Collections;

public class Landing : MonoBehaviour
{
    [SerializeField] private EffectManager landingEffects;

    void Awake() {

    }
    
    void Start() {
        
    }
    
    void Update() {
        
    }
    
    // Called via SendMessage in CharacterMovement.
    private void Grounded(float verticalSpeed) {
        if (landingEffects) {
            landingEffects.StartEvent();
        }
    }
}
