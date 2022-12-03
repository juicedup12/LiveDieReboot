using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmitSingularParticle : MonoBehaviour
{

    ParticleSystem Particle;
    [SerializeField] int ParticleCount;

    // Start is called before the first frame update
    void Start()
    {
        Particle = GetComponent<ParticleSystem>();
    }


    public void EmitParticle(Transform t)
    {
        ParticleSystem.EmitParams @params = new ParticleSystem.EmitParams();
        @params.position = t.position;
       @params.applyShapeToPosition = true;
        Particle.Emit(@params, ParticleCount);
    }
}
