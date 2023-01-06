using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//place this on the parent transform with a rigidbody of whatever needs to be carried
public class Carryable : MonoBehaviour, IInteractable
{

    public Transform CarryTransform;
    public Rigidbody CarryBody;
    protected float CarryZdist;
    public UnityEvent OnCarry;
    public UnityEvent OnRelease;

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
            //Vector3 dir =  CarryBody.transform.position - CarryTransform.position;
            CarryBody.MovePosition(CarryTransform.position + CarryTransform.forward * CarryZdist + CarryTransform.up);
            CarryBody.rotation = CarryTransform.rotation;
            //CarryBody.transform.LookAt(CarryBody.position + dir);
            //transform.parent = CarryTransform;
            //transform.position = CarryTransform.position + CarryTransform.forward + CarryTransform.up;
        }
    }

    //called by player
    public virtual void BeCarriedBy(Transform CarryTransform, float zdistance)
    {
        OnCarry?.Invoke();
        CarryZdist = zdistance; 
        this.CarryTransform =  CarryTransform;
        print("carryable being picked up by " + CarryTransform.name);
        CarryBody.isKinematic = true;
    }
    
    public virtual void Release(Vector3 velocity)
    {
        print("releasing ragdoll  " + gameObject.name);
        CarryTransform = null;
        CarryBody.isKinematic = false;
        OnRelease?.Invoke();
    }

    public void interact()
    {
        //need to add condition to check if carryable is being picked up
        if (Player.Instance)
        {
            print(" player instance is " + Player.Instance.transform);
            print("carryable interact of" + transform.root + "triggered ");
            //Transform t = Player.Instance.transform;
            //CarryComponent.BeCarriedBy(t);
            Player.Instance.CarryBody(this);

        }
        else
            print("no player instance");
    }
}
