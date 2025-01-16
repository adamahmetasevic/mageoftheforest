using UnityEngine;
using UnityEngine.Rendering.Universal;  // For Light2D

public class SpotlightFollow : MonoBehaviour
{
    public Transform enemy; // The enemy transform that is emitting sparks
    public ParticleSystem sparkParticles; // The Particle System for sparks
    public Light2D spotlight; // The 2D spotlight light source

    private ParticleSystem.Particle[] sparks; // Array to hold particle data

    void Update()
    {
        if (sparkParticles != null && spotlight != null)
        {
            // Get all the current particles
            int particleCount = sparkParticles.GetParticles(sparks);
            
            if (particleCount > 0)
            {
                // Assuming the first particle is the one you're tracking
                Vector3 lastSparkPosition = sparks[particleCount - 1].position;
                
                // Set the spotlight's position to the last particle's position
                spotlight.transform.position = new Vector3(lastSparkPosition.x, lastSparkPosition.y, spotlight.transform.position.z);
            }
        }
    }
}
