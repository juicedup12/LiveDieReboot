using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Carryable : MonoBehaviour
{

    public Transform CarryTransform;
    public Rigidbody CarryBody;
    public UnityEvent OnPickUp;
    public UnityEvent OnRelease;

    public virtual void Start()
    {
        if(!CarryBody)
        CarryBody = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        //print("carryable update");
        if (CarryTransform)
        {
            //print("carryable ");
            Vector3 dir =  CarryBody.transform.position - CarryTransform.position;
            CarryBody.MovePosition(CarryTransform.position + CarryTransform.forward + CarryTransform.up);
            CarryBody.rotation = CarryTransform.rotation;
            //CarryBody.transform.LookAt(CarryBody.position + dir);
            //transform.parent = CarryTransform;
            //transform.position = CarryTransform.position + CarryTransform.forward + CarryTransform.up;
        }
    }

    public virtual void PickUp(Transform CarryTransform)
    {
        this.CarryTransform =  CarryTransform;
        print("ragdoll being picked up by " + CarryTransform.name);
        GetComponent<Rigidbody>().isKinematic = true;
        OnPickUp?.Invoke();
    }
    
    public virtual void Release(Vector3 velocity)
    {
        CarryTransform = null;
        CarryBody.isKinematic = false;
        OnRelease?.Invoke();
    }


    public virtual void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            print("player near ragdoll");
            other.GetComponent<Player>().TargetPickUp(this);
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            print("player away from ragdoll");
            other.GetComponent<Player>().TargetPickUp(null);
        }
    }

}
