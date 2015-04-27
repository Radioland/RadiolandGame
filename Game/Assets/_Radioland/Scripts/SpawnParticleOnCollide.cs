using UnityEngine;
using System.Collections;

public class SpawnParticleOnCollide : MonoBehaviour {
	public ParticleSystem thisParticleSystem;
    public ParticleCollisionEvent[] collisionEvents;

    public GameObject subParticleSystem;
    [Range(0,1)]
    public float spawnChance;
    
    void Start() {
        thisParticleSystem = GetComponent<ParticleSystem>();
        collisionEvents = new ParticleCollisionEvent[16];
    }

    void OnParticleCollision(GameObject other) {
        int safeLength = thisParticleSystem.GetSafeCollisionEventSize();
        if (collisionEvents.Length < safeLength)
            collisionEvents = new ParticleCollisionEvent[safeLength];
        
        int numCollisionEvents = thisParticleSystem.GetCollisionEvents(other, collisionEvents);
        int i = 0;
        while (i < numCollisionEvents) {
            if (Random.value <= spawnChance) {
                Vector3 pos = collisionEvents[i].intersection;
                GameObject subParticle = (GameObject)Instantiate(subParticleSystem, pos, subParticleSystem.transform.rotation);
            	subParticle.transform.SetParent(thisParticleSystem.transform);

            	// Want to destroy this specific particle that collided...
            }
            i++;
        }
    }
}