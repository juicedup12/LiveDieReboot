using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollBehavior : Carryable
{

    Rigidbody[] rigidbodies;
    public Rigidbody hip;
    public bool HasBeenHit, IsBeingCarried;
    Collider hipCol;
    public float FlyMultiplier;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        rigidbodies = GetComponents<Rigidbody>();
        hipCol = GetComponent<Collider>();
    }



    public void Attatch()
    {
        foreach (Rigidbody ragdollBone in rigidbodies)
        {
            print(rigidbodies.Length + "amount of rigidbodies");
            ragdollBone.isKinematic = true;
            GetComponent<BoxCollider>().enabled = false;
        }
    }

    /// <summary>
    /// Detatch ragdoll from player and send it flying with velocity
    /// </summary>
    /// <param name="Velocity"></param>
    public void Detatch(Vector3 Velocity)
    {
        foreach (Rigidbody ragdollBone in rigidbodies)
        {
            ragdollBone.isKinematic = false;
            ragdollBone.AddForce(Velocity * FlyMultiplier, ForceMode.VelocityChange);
        }
        GetComponent<SphereCollider>().enabled = true;
        this.enabled = true;

        //incase player dies inside of turret view, turret won't keep shooting at body
        HasBeenHit = true;
        GetComponent<BoxCollider>().enabled = true;
    }

    public override void PickUp(Transform t)
    {
        //hip is the root
        CarryTransform = t;
        print("ragdoll being picked up by " +  CarryTransform.name);
        hip.isKinematic = true;
        IsBeingCarried = true;
    }

    public void Fly(Vector3 velocity)
    {
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.AddForce(velocity * FlyMultiplier, ForceMode.VelocityChange);
        }
    }

    public override void Release(Vector3 velocity)
    {
        print("ragdoll released");
        Fly(velocity);
        hip.isKinematic = false;
        IsBeingCarried = false;
        CarryTransform = null;
        StartCoroutine(PickUpCoolDown());
    }

    IEnumerator PickUpCoolDown()
    {
        GetComponent<SphereCollider>().enabled = false;
        yield return new WaitForSeconds(.1f);
        GetComponent<SphereCollider>().enabled = true;

    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Turret"))
        {
            print("ragdoll carry hit turret");
            collision.gameObject.GetComponent<CharacterShootPredict>().KnockOver(transform.forward);
        }
        
    }

    new public void OnTriggerEnter(Collider other)
    {
        if(!IsBeingCarried) base.OnTriggerEnter(other);
 
        if (other.CompareTag("bullet"))
        {

            //print("bullet hit ragdoll" + other.gameObject.name);
            if (IsBeingCarried)
            {
                Destroy(other.gameObject);
            }
            else
            {
                print("bullet hit ragdoll");
                //Time.timeScale = .3f;
                Fly(other.GetComponent<Rigidbody>().velocity);
                HasBeenHit = true;
            }
        }

        if(other.gameObject.CompareTag("life"))
        {
            Player plyr = GetComponentInParent<Player>();
            if (plyr)
            {
                plyr.CheckPowerUpType(other.GetComponent<PowerUp>());
                print("ragdoll touched life");
            }
            else
            {
                print("no player retrieved");
            }
        }
        if(other.CompareTag("Finish"))
        {
            GetComponentInParent<Player>().ClearLevel();
        }


        //if ragdoll hits life spawn a player and destroy ragdoll

    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        if (other.CompareTag("Turret"))
        {
            HasBeenHit = false;
        }
    }

}
