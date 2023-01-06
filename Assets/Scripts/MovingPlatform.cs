using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    protected Vector3 origin;
    protected Vector3 target;
    [SerializeField] Vector3 TravelVector;
    bool ReachedTarget = false;
    protected float ElapsedTime;
    public float TimeToReach;
    PlatformChild child;
    Rigidbody rb;
    public Action OnReachedTarget;

    // Start is called before the first frame update
    public virtual void Start()
    {
        origin = transform.position;
        target = origin + TravelVector;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //ElapsedTime += Time.deltaTime;
        //if (!ReachedTarget)
        //{
        //    //MoveRigidbody(origin, target);
        //    MovePositionLerp(origin, target);
        //}
        //else
        //{
        //    //MoveRigidbody(target, origin);
        //    MovePositionLerp(target, origin);
        //}
    }

    protected void MoveToPos(float time, Vector3 Position)
    {
        ElapsedTime = time;
        MovePositionLerp(transform.position, Position);
    }
    
    protected void MoveToPos( Vector3 Position)
    {
        MovePositionLerp(transform.position, Position);
    }

    public void MoveToTarget(float time)
    {
        ElapsedTime = time;
        MovePositionLerp(origin, target);
    }

    public void MoveToOrigin(float time)
    {
        ElapsedTime = time;
        MovePositionLerp(target, origin);
    }

    void MovePositionLerp(Vector3 posA, Vector3 posB )
    {
        float percentage = ElapsedTime / TimeToReach;
        float dist = Vector3.Distance(posA, posB);
        if (percentage >= 1 || dist < .1)
        {
            transform.position = posB;
            ElapsedTime = 0;
            ReachedTarget = !ReachedTarget;
            OnReachedTarget?.Invoke();
        }
        Vector3 newPos = Vector3.Lerp(posA, posB, percentage);
        Vector3 change =  newPos - transform.position;
        transform.position = newPos;
        

        if (child)
        {
            child.MoveWithPlatfrom(change); ;
        }
    }
    
    void MoveRigidbody(Vector3 posA, Vector3 posB)
    {
        float percentage = ElapsedTime / TimeToReach;
        Vector3 newPos = Vector3.Lerp(posA, posB, percentage);
        rb.MovePosition(newPos);
        if (percentage >= 1)
        {
            ElapsedTime = 0;
            ReachedTarget = !ReachedTarget;
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        print("platform trigger hit" + other.gameObject);
        if(other.TryGetComponent(out PlatformChild child))
        {
            print("got platform child");
            this.child = child;
        }
        else
        {
            print("no child to get");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out PlatformChild child))
        {
            if(this.child == child)
            {
                this.child = null;
            }
        }
    }
}
