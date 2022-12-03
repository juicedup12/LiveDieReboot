using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Carryable : MonoBehaviour
{

    public Transform CarryTransform;
    public Rigidbody CarryBody;
    [SerializeField] float CarryZdist;

    public virtual void Start()
    {
        if(!CarryBody)
        CarryBody = GetComponent<Rigidbody>();
    }

    public void FixedUpdate()
    {
        //print("carryable update");
        if (CarryTransform)
        {
            //print("carryable ");
            Vector3 dir =  CarryBody.transform.position - CarryTransform.position;
            CarryBody.MovePosition(CarryTransform.position + CarryTransform.forward * CarryZdist + CarryTransform.up);
            CarryBody.rotation = CarryTransform.rotation;
            //CarryBody.transform.LookAt(CarryBody.position + dir);
            //transform.parent = CarryTransform;
            //transform.position = CarryTransform.position + CarryTransform.forward + CarryTransform.up;
        }
    }

    public virtual void BeCarriedBy(Transform CarryTransform)
    {
        this.CarryTransform =  CarryTransform;
        print("carryable being picked up by " + CarryTransform.name);
        GetComponent<Rigidbody>().isKinematic = true;
    }
    
    public virtual void Release(Vector3 velocity)
    {
        print("releasing ragdoll  " + gameObject.name);
        CarryTransform = null;
        CarryBody.isKinematic = false;
    }


    

}
