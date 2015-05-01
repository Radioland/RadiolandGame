using UnityEngine;

public class ParticlesFollowCurve : MonoBehaviour
{
    [SerializeField] private ParticleSystem system;
    [SerializeField] private bool flickerParticles = true;
    [SerializeField] private ICurve curve;

    [SerializeField] [Tooltip("Higher values interpolate to target position faster.")]
    [Range(0f, 1f)] private float smoothing = 0.2f;

    [SerializeField] [Tooltip("Ignored if the spline does not loop.")]
    private float loopTime = 1f;

    private bool stopped;

    private void Reset() {
        system = gameObject.GetComponentInChildren<ParticleSystem>();
        curve = gameObject.GetComponentInChildren<ICurve>();

        if (system) { Debug.Log("Found " + system.GetPath() + " for " + this.GetPath()); }
        if (curve) { Debug.Log("Found " + curve.GetPath() + " for " + this.GetPath()); }

        stopped = false;
    }

    private void Awake() {
        system.simulationSpace = ParticleSystemSimulationSpace.World;

        if (curve.loop) {
            system.emissionRate = system.maxParticles / loopTime;

            // Work within a lifetime range of [loopTime, 2 * loopTime].
            // When lifetime falls below loopTime, add loopTime.

            // We could set startLifetime to be infinite then calculate elapsed
            // time and mod that againt loopTime, but that could introduce
            // numerical inaccuracies.
            system.startLifetime = 2 * loopTime;
        }
    }

    private void Start() {

    }

    public void StopSystem() {
        system.Stop();
        stopped = true;
    }

    public void StartSystem() {
        system.Play();
        stopped = false;
    }

    private void Update() {
        if (system.isPaused) { return; }

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[system.particleCount];
        int particleCount = system.GetParticles(particles);

        for (int i = 0; i < particleCount; i++) {
            ParticleSystem.Particle particle = particles[i];

            float t;
            if (curve.loop) {
                if (particle.lifetime < particle.startLifetime / 2f) {
                    if (stopped) {
                        particle.lifetime = 0f;
                    } else {
                        particle.lifetime += particle.startLifetime / 2f;
                    }
                }
                t = (1f - (particle.lifetime / particle.startLifetime)) * 2f;
            } else {
                t = 1f - (particle.lifetime / particle.startLifetime);
            }
            particle.position = Vector3.Lerp(particle.position, curve.GetPoint(t), smoothing);

            if (flickerParticles) {
                particle.size = Mathf.Lerp(1,0.5f, Random.value);
            }

            particles[i] = particle;
        }

        system.SetParticles(particles, particleCount);
    }
}
