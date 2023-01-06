using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

//a rigidbody object that the creates a predictable path for the ragdoll to follow
public class RagdollTrajectory : VisualizeTrajectory
{
    RagdollBehavior ragdoll;
    bool InTrajectory;
    UnityAction ReleaseAction;
    Collider col;


    public override void Start()
    {
        col = GetComponent<Collider>();
        base.Start();
        ragdoll = GetComponentInChildren<RagdollBehavior>();
        ragdoll.SetTrajectory = this;
        print("got ragdoll : " + ragdoll);
        ReleaseAction += ReleaseRagdoll;
        //ragdoll.Detatch(Vector3.forward * 12);
    }

    private void Update()
    {
        //if (Mouse.current.leftButton.wasPressedThisFrame)
        //{
        //    //transform.position = hip.transform.position;
        //    //if(hip.isActiveAndEnabled)
        //    //hip.BeCarriedBy(transform);
        //    ragdoll.Detatch(Vector3.forward * 12);
        //}
    }

    public void LaunchRagdoll(Vector3 dir)
    {
        InTrajectory = true;
        col.enabled = true;
        transform.position = ragdoll.transform.position;
        transform.rotation = ragdoll.transform.rotation;
        ragdoll.BeCarriedBy(transform, 0);
        ragdoll.OnCarry.AddListener(ReleaseAction);
        rb.isKinematic = false;
        ApplyForce(dir);

    }

    public void ReleaseRagdoll()
    {
        print(gameObject + " releasing ragdoll");
        col.enabled = false;
        ragdoll.Release(Vector3.zero);
        rb.isKinematic = true;
        ragdoll.OnCarry.RemoveListener(ReleaseAction);
        RemoveTrajectory();
        
    }

    void RemoveTrajectory()
    {
        lr.positionCount = 0;
        InTrajectory = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("ragdoll trajectory collided with " + collision.gameObject);
        if(InTrajectory)
        {
            ReleaseRagdoll();
        }
    }

}
