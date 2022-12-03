using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

public class TimedParticleEmitter : MonoBehaviour
{
    
    ParticleSystem Particles;
    Transform TargetTransform;
    [SerializeField] float ParticleDuration = 2;
    [SerializeField] int ParticlesToEmit;
    public void EmitParticles(Transform t)
    {
        print("emit particles called");
        ParticleSystem.EmitParams @params = new ParticleSystem.EmitParams();
        @params.position = t.position;
        @params.applyShapeToPosition = true;
        TargetTransform = t;
        ParticleSystem.ShapeModule shape = Particles.shape;
        shape.rotation = t.rotation.eulerAngles;
        print("emit transform pos is " + t.position);
        StartCoroutine(EmitRoutine(@params, shape));
    }

    // Start is called before the first frame update
    void Start()
    {

        Particles = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //need to update particle postion here instead of emitparticles()
    IEnumerator EmitRoutine(ParticleSystem.EmitParams @params, ParticleSystem.ShapeModule shape)
    {
        print("starting emit routine");
        float timer = 0;
        while (timer < ParticleDuration)
        {
            yield return new WaitForSeconds(.1f);
            shape.rotation = TargetTransform.rotation.eulerAngles;
            @params.position = TargetTransform.position;
            Particles.Emit(@params, ParticlesToEmit);
            timer += .1f;
            yield return null;
        }
        print("emit routine finished");
    }
}
