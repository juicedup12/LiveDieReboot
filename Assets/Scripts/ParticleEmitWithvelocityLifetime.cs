using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEmitWithvelocityLifetime : MonoBehaviour
{
    ParticleSystem Particle;
    [SerializeField] int ParticleCount;
    ParticleSystem.VelocityOverLifetimeModule VelocityLifetime;
    ParticleSystem.ShapeModule shapeModule;
    [SerializeField] ParticleSystem.MinMaxCurve BurstCurve;
    [SerializeField] ParticleSystem.MinMaxCurve JoinCurve;

    // Start is called before the first frame update
    void Start()
    {
        Particle = GetComponent<ParticleSystem>();
        VelocityLifetime = Particle.velocityOverLifetime;
        shapeModule = Particle.shape;
    }


    public void EmitParticle(Transform t)
    {
        ParticleSystem.EmitParams @params = new ParticleSystem.EmitParams();
        @params.position = t.position;
        VelocityLifetime.orbitalOffsetX = t.position.x;
        VelocityLifetime.orbitalOffsetY = t.position.y;
        VelocityLifetime.orbitalOffsetZ = t.position.z;
        @params.applyShapeToPosition = true;
        Particle.Emit(@params, ParticleCount);
    }


    public void BurstParticles(Transform t)
    {
        ParticleSystem.EmitParams @params = new ParticleSystem.EmitParams();
        @params.position = t.position;
        @params.applyShapeToPosition = true;
        VelocityLifetime.orbitalOffsetX = t.position.x;
        VelocityLifetime.orbitalOffsetY = t.position.y;
        VelocityLifetime.orbitalOffsetZ = t.position.z;
        //VelocityLifetime.orbitalX = t.position.x;
        //VelocityLifetime.orbitalY = t.position.y;
        //VelocityLifetime.orbitalZ = t.position.z;
        //VelocityLifetime.radial.curve = BurstAnimCurve;
        //VelocityLifetime.space = ParticleSystemSimulationSpace.Local;
        //VelocityLifetime.space = ParticleSystemSimulationSpace.World;
        VelocityLifetime.radial = BurstCurve;
        shapeModule.radius = .5f;
        Particle.Emit(@params, ParticleCount);
    }
    
    public void JoinParticles(Transform t)
    {
        ParticleSystem.EmitParams @params = new ParticleSystem.EmitParams();
        @params.position = t.position;

        VelocityLifetime.radial = JoinCurve;
        @params.applyShapeToPosition = true;
        VelocityLifetime.orbitalOffsetX = t.position.x;
        VelocityLifetime.orbitalOffsetY = t.position.y;
        VelocityLifetime.orbitalOffsetZ = t.position.z;
        //VelocityLifetime.radial.curve = 
        //VelocityLifetime.space = ParticleSystemSimulationSpace.Local;

        shapeModule.radius = 2;
        Particle.Emit(@params, ParticleCount);
    }
}
