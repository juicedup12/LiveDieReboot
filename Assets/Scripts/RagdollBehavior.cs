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
    [SerializeField] bool StartAsRagdoll = false;
    [SerializeField] float BulletReactionScale;
    [SerializeField] Transform[] TransformsToUnparent;
    int UnparentIndex;
    bool HasUnparentedTransform = false;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        hipCol = GetComponent<Collider>();
        AttatchRigidBodies();
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
    /// <param name="Velocity"></param>
    public void Detatch(Vector3 Velocity)
    {
        
        foreach (Rigidbody ragdollBone in rigidbodies)
        {
            if(ragdollBone.CompareTag("Player"))
            {
                print("found a player tag, continuing");
                continue;
            }
            print("detatching rigidbody with " + Velocity + " velocity");
            ragdollBone.isKinematic = false;
            ragdollBone.AddForce(Velocity * FlyMultiplier, ForceMode.VelocityChange);
        }
        GetComponent<SphereCollider>().enabled = true;
        this.enabled = true;

        //incase player dies inside of turret view, turret won't keep shooting at body
        HasBeenHit = false;
        GetComponent<BoxCollider>().enabled = true;
    }

    public override void BeCarriedBy(Transform t)
    {
        base.BeCarriedBy(t);
        //hip is the root
        //CarryTransform = t;
        print("ragdoll being picked up by " +  CarryTransform.name);
        //hip.isKinematic = true;
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
            
            //print("bullet hit ragdoll" + other.gameObject.name);
            if (IsBeingCarried)
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
                    item.AddForce(Random.insideUnitSphere * BulletReactionScale, ForceMode.VelocityChange);
                    //item.AddExplosionForce(BulletReactionScale, other.transform.position, 1);
                    
                }
                HasUnparentedTransform = false;
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
