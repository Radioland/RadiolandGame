using UnityEngine;
using System.Collections;

public class ParticleEffect : Effect
{
    public ParticleSystem effectParticleSystem;
    public GameObject positionObject;
    public Vector3 offsetFromObject;

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

        Vector3 position;
        if (positionObject) {
            position = positionObject.transform.position;
            // Orient and offset in the x direction
            position.x += offsetFromObject.x * positionObject.transform.forward.x;
            // Simply offset in the y and z directions
            position.y += offsetFromObject.y;
            position.z += offsetFromObject.z;
        } else {
            position = transform.position + offsetFromObject;
        }
        effectParticleSystem.transform.position = position;
        
        effectParticleSystem.Play();
        effectParticleSystem.enableEmission = true;
    }
    
    public override void EndEffect() {
        base.EndEffect();
        
        effectParticleSystem.enableEmission = false;
    }
}
