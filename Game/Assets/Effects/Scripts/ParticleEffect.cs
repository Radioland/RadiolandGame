using UnityEngine;
using System.Collections;

public class ParticleEffect : Effect
{
    // Variables to specify in the editor.
    [SerializeField] private ParticleSystem effectParticleSystem;
    [SerializeField] private bool repositionParticleSystem = true;
    [SerializeField] private GameObject positionObject;
    [SerializeField] private Vector3 offsetFromObject;

    protected override void Awake() {
        base.Awake();

        if (!effectParticleSystem) {
            effectParticleSystem = gameObject.GetComponent<ParticleSystem>();
        }

        effectParticleSystem.Stop();
    }
    
    protected override void Start() {
        base.Start();
    }
    
    protected override void Update() {
        base.Update();
    }
    
    public override void TriggerEffect() {
        base.TriggerEffect();
    }
    
    public override void StartEffect() {
        base.StartEffect();

        if (repositionParticleSystem) {
            Vector3 position;
            if (positionObject) {
                position = positionObject.transform.position + offsetFromObject;
            } else {
                position = transform.position + offsetFromObject;
            }
            effectParticleSystem.transform.position = position;
        }
        
        effectParticleSystem.Play();
        effectParticleSystem.enableEmission = true;
    }
    
    public override void EndEffect() {
        base.EndEffect();
        
        effectParticleSystem.enableEmission = false;
    }
}
