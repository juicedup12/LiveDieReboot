using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RagdollBehavior : Carryable
{

    Rigidbody[] rigidbodies;
    public Rigidbody hip;
    public bool HasBeenHit, IsBeingCarried;
    Collider hipCol;
    public float FlyMultiplier;
    [SerializeField] bool StartAsRagdoll = false;
    [SerializeField] float BulletReactionScale;
    [SerializeField] Transform[] TransformsToUnparent;
    int UnparentIndex;
    bool HasUnparentedTransform = false;
    [SerializeField] UnityEvent<Rigidbody[]> OnDetatch;
    [SerializeField] RagdollTrajectory ragdollTrajectory;
    public RagdollTrajectory SetTrajectory { set => ragdollTrajectory = value; }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        hipCol = GetComponent<Collider>();
        AttatchRigidBodies();
        enabled = false;
    }



    public void AttatchRigidBodies()
    {

        print(rigidbodies.Length + "amount of rigidbodies");
        if(!StartAsRagdoll)
        foreach (Rigidbody ragdollBone in rigidbodies)
        {
            ragdollBone.isKinematic = true;
            GetComponent<BoxCollider>().enabled = false;
        }
    }

    /// <summary>
    /// Detatch ragdoll from player and send it flying with velocity
    /// </summary>
    /// <param name="velocity"></param>
    public void Detatch(Vector3 velocity)
    {
        
        foreach (Rigidbody ragdollBone in rigidbodies)
        {
            if(ragdollBone.CompareTag("Player"))
            {
                print("found a player tag, continuing");
                continue;
            }
            //print("detatching rigidbody " + ragdollBone + " with " + velocity + " velocity");
            ragdollBone.isKinematic = false;
            //ragdollBone.AddForce(Velocity * FlyMultiplier, ForceMode.Impulse);
            //ragdollBone.velocity = Velocity * FlyMultiplier;
        }
        //hip.velocity = Velocity * FlyMultiplier;
        //hip.AddForce(Velocity * FlyMultiplier, ForceMode.Impulse);
        if (velocity.magnitude > 10)
        {
            print("launching ragdoll velocity with " + velocity);
            ragdollTrajectory.LaunchRagdoll(velocity);
        }
        else
        {
            print("launching hips with " + velocity);
            hip.AddForce(velocity * 10, ForceMode.VelocityChange);
        }
        GetComponent<SphereCollider>().enabled = true;
        this.enabled = true;

        //incase player dies inside of turret view, turret won't keep shooting at body
        HasBeenHit = false;
        GetComponent<BoxCollider>().enabled = true;
        //print("rigid body velocity is " + hip.velocity);
        OnDetatch?.Invoke(rigidbodies);
    }

    public override void BeCarriedBy(Transform t, float zdist)
    {
        
        base.BeCarriedBy(t, zdist);
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.velocity = Vector3.zero;
        }
        //hip is the root
        //CarryTransform = t;
        print("ragdoll being picked up by " +  t.name);
        //hip.isKinematic = true;
        IsBeingCarried = true;
    }

    //may need to make this virtual in order to use this on other carryables
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
        Detatch(velocity);
        base.Release(velocity);
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
        //print("ragdoll carry hit : " + collision.gameObject.name);

        //if (collision.gameObject.CompareTag("Turret"))
        //{
        //    print("ragdoll carry hit turret");
        //    //collision.gameObject.GetComponent<CharacterShootPredict>().KnockOver(transform.forward);
        //}
        
    }

    public void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("bullet"))
        {
            
            print("bullet hit ragdoll" + other.gameObject.name);
            if (IsBeingCarried && CarryTransform.CompareTag("Player")) 
            {
                if (UnparentIndex >= TransformsToUnparent.Length)
                {
                    Player.Instance.ReleaseCarryBody();
                    Destroy(other.gameObject);
                    Destroy(gameObject);
                    return;
                }
                //add code to remove rigidbodies
                //rigid body still referenced in array list
                foreach (Rigidbody item in rigidbodies)
                {
                    if (!HasUnparentedTransform && item.transform == TransformsToUnparent[UnparentIndex])
                    {
                        //after detatching 
                        print("unparenting limb");
                        item.transform.parent = null;
                        Destroy(item.GetComponent<CharacterJoint>());
                            UnparentIndex++;
                        HasUnparentedTransform = true;
                    }
                    print("moving ragdoll from bullet hit");
                    //item.AddForceAtPosition(Random.insideUnitSphere * BulletReactionScale, item.position);
                    item.AddForce(UnityEngine.Random.insideUnitSphere * BulletReactionScale, ForceMode.VelocityChange);
                    //item.AddExplosionForce(BulletReactionScale, other.transform.position, 1);
                    
                }
                HasUnparentedTransform = false;
                Destroy(other.gameObject);
            }
            else
            {
                Destroy(other.gameObject);
                print("bullet hit ragdoll");
                //Time.timeScale = .3f;
                //Fly(other.GetComponent<Rigidbody>().velocity);
                Detatch(other.GetComponent<Rigidbody>().velocity * 50);
                HasBeenHit = true;
            }
        }

        //if(other.gameObject.CompareTag("life"))
        //{
        //    Player plyr = GetComponentInParent<Player>();
        //    if (plyr)
        //    {
        //        plyr.CheckPowerUpType(other.GetComponent<PowerUp>());
        //        print("ragdoll touched life");
        //    }
        //    else
        //    {
        //        print("no player retrieved");
        //    }
        //}
        //if(other.CompareTag("Finish"))
        //{
        //    GetComponentInParent<Player>().ClearLevel();
        //}


        //if ragdoll hits life spawn a player and destroy ragdoll

    }

    public  void OnTriggerExit(Collider other)
    {
        //not sure why this is here
        if (other.CompareTag("Turret"))
        {
            HasBeenHit = false;
        }
    }

}
