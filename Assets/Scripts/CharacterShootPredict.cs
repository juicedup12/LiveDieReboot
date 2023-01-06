using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterShootPredict  : MonoBehaviour
{
    private Animator anim;
    public GameObject BulletPrefab;
    private Component target;
    private float interceptionTime1 = 0.0f;
    private bool HaveCourse;
    [SerializeField]
    private float projectileSpeed;
    public Transform interceptionPoint;
    private float timer;
    public float CoolDown;
    private ParticleSystem particles;
    private Vector3 targetVelocity, targetPosition;
    RagdollBehavior RagdollTarget;
    CharacterController CharacterControllerTarget;
    [SerializeField]
    Material HostileMat, FriendlyMat;
    Renderer rndr;
    bool IsFriendly;
    Rigidbody rb;
    Quaternion defaultRot;
    [SerializeField]
    float MaxColVelocity;
    [SerializeField] Transform TurretBodyTransform;
    [SerializeField] LayerMask layercheck;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        timer = Time.time;
        particles = GetComponentInChildren<ParticleSystem>();
        rndr = GetComponent<Renderer>();
        HostileMat = rndr.material;
        defaultRot = TurretBodyTransform.rotation;

    }


    //need to replace character controller reference and use a rigidbody instead
    void Update()
    {
        if (!TurretBodyTransform)
        {
            Debug.Log("error no turret body transform");
        }

        if (IsFriendly)
        {
            return;
        }
        if (!target || IsFriendly)
        {
            TurretBodyTransform.rotation = defaultRot;
            if (!target)
            {
                //print("no target");
            }
            //print("friendly status: " + IsFriendly);
            return;
        }


        if (RagdollTarget) 
        {
            if (RagdollTarget.HasBeenHit) 
            { 
                target = null; 
                RagdollTarget = null; 
                print("target has been hit already");  
                return; 
            }
            targetPosition = RagdollTarget.transform.position;
            targetVelocity = RagdollTarget.hip.velocity;
            //print("target velocity is" + targetVelocity);
        }
        if(CharacterControllerTarget)
        {
            if(!CharacterControllerTarget.enabled)
            {
                CharacterControllerTarget = null;
                //print("character controller not enabled");
                target = null;
                return;
            }
        }

        //print("target is " + target.GetType());
        //bool HasRagdollTarget = RagdollTarget;

        //print("ragdoll target assigned: " + HasRagdollTarget);
        if (!RagdollTarget && target is RagdollBehavior)
        {
            print("assigning ragdoll as target");
            RagdollTarget = (RagdollBehavior)target;
        }
        if(!CharacterControllerTarget && target is CharacterController)
        {
            print("assigning character controller as target");
            CharacterControllerTarget = (CharacterController)target;
        }
        if (target is CharacterController)
        {
            
            targetPosition = CharacterControllerTarget.transform.position;
            targetVelocity = CharacterControllerTarget.velocity;
        }
        //print("target controller is " + target.gameObject.name + $"/n position is {targetPosition}" + " velocity is " + targetVelocity);

        Vector3 IC = CalculateInterceptCourse(targetPosition, targetVelocity, transform.position, projectileSpeed);

        //print("intercept course is " + IC);
        if (HaveCourse = IC != Vector3.zero)
        {
            IC.Normalize();
            interceptionTime1 = FindClosestPointOfApproach(targetPosition, targetVelocity, transform.position, IC * projectileSpeed);
            interceptionPoint.position = targetPosition  + targetVelocity * interceptionTime1;
        }
        
        TurretBodyTransform.LookAt(interceptionPoint);

        if (HaveCourse)
            if (Time.time > timer)
            {
                print("can shoot");
                timer = Time.time + CoolDown;
                Shoot();
            }

    }


    public static Vector3 CalculateInterceptCourse(Vector3 aTargetPos, Vector3 aTargetSpeed, Vector3 aInterceptorPos, float aInterceptorSpeed)
    {
        Vector3 targetDir = aTargetPos - aInterceptorPos;

        float iSpeed2 = aInterceptorSpeed * aInterceptorSpeed;
        float tSpeed2 = aTargetSpeed.sqrMagnitude;
        float fDot1 = Vector3.Dot(targetDir, aTargetSpeed);
        float targetDist2 = targetDir.sqrMagnitude;
        float d = (fDot1 * fDot1) - targetDist2 * (tSpeed2 - iSpeed2);
        if (d < 0.1f)  // negative == no possible course because the interceptor isn't fast enough
            return Vector3.zero;
        float sqrt = Mathf.Sqrt(d);
        float S1 = (-fDot1 - sqrt) / targetDist2;
        float S2 = (-fDot1 + sqrt) / targetDist2;

        if (S1 < 0.0001f)
        {
            if (S2 < 0.0001f)
                return Vector3.zero;
            else
                return (S2) * targetDir + aTargetSpeed;
        }
        else if (S2 < 0.0001f)
            return (S1) * targetDir + aTargetSpeed;
        else if (S1 < S2)
            return (S2) * targetDir + aTargetSpeed;
        else
            return (S1) * targetDir + aTargetSpeed;
    }

    public static float FindClosestPointOfApproach(Vector3 aPos1, Vector3 aSpeed1, Vector3 aPos2, Vector3 aSpeed2)
    {
        Vector3 PVec = aPos1 - aPos2;
        Vector3 SVec = aSpeed1 - aSpeed2;
        float d = SVec.sqrMagnitude;
        // if d is 0 then the distance between Pos1 and Pos2 is never changing
        // so there is no point of closest approach... return 0
        // 0 means the closest approach is now!
        if (d >= -0.0001f && d <= 0.0002f)
            return 0.0f;
        return (-Vector3.Dot(PVec, SVec) / d);
    }

    void Shoot()
    {
        
        Rigidbody B = Instantiate(BulletPrefab, TurretBodyTransform.position + transform.forward, transform.rotation).GetComponent<Rigidbody>(); ;
        B.velocity = TurretBodyTransform.forward * projectileSpeed;
        //particles.Play();
    }


    //private void OnTriggerEnter(Collider other)
    //{

    //    if (other.CompareTag("player"))
    //    {
    //        print("ball hit");
    //        anim.SetTrigger("Attack");
    //        
    //        print("ball velocity is " + target.velocity);
    //    }
    //}

    bool TurretRayCheck(Vector3 ObjPos, int lmask)
    {

        Vector3 dir = ObjPos - TurretBodyTransform.position;

        Ray ray = new(TurretBodyTransform.position, dir);

        Debug.DrawRay(TurretBodyTransform.position, dir, Color.green, 4);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 15, lmask))
        {
            print("raycast hit " + hit.transform.gameObject.name);
             Debug.DrawRay(TurretBodyTransform.position, hit.point - TurretBodyTransform.position, Color.red, 1);

            return true;
        }

        return false;
        
    }

    void ResetRotation()
    {
        //go back to default rotation after target leaves
    }

    public void Friendly()
    {
        print("turret is friendly");
        rndr.material = FriendlyMat;
        IsFriendly = true;
    }

    public void hostile()
    {
        print("turret is hostile");
        rndr.material = HostileMat;
        transform.parent = null;
        IsFriendly = false;
        //rb.isKinematic = true;
        rb.freezeRotation = true;
        enabled = true;
    }

    public void KnockOver()
    {
        print("turret knocked over");
        Friendly();
        enabled = false;
        rb.isKinematic = false;
        rb.freezeRotation = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (target|| IsFriendly) return;

        print(other.gameObject + "entered turret trigger");

        //if player enters, raycast in player layer and get charcontroller velocity
        if (other.TryGetComponent(out Player player))
        {
            print($"player in view of turret ({player.transform})");

            if (TurretRayCheck(other.transform.position + Vector3.up, 1 << 0))
            {
                print("raycast hit something");
            }
            else
            {
                print("nothing hit in raycast");
                target = player.GetCharacterController;
                RagdollTarget = null;
                print(target ? $"target is {target}" : "no target");
                return;
                //print("player hit");
            }

        }
        //if(other.CompareTag("limb"))
        //{
        //    print($"limb in view of turret ({other.transform})");
        //    if (TurretRayCheck(other.transform.position, 1 << 9))
        //    {
        //        target = other.GetComponent<Rigidbody>();
        //        print(target ? $"target is {other.gameObject.name}" : "no target");
        //        //print("player hit");
        //    }
        //}


        //if ragdoll enters raycast in ragdoll layer get rigidbody velocity
        if (other.CompareTag("Ragdoll"))
        {
            print($"ragdoll in view of turret ({other.transform})");

            RagdollBehavior ragdoll = other.GetComponent<RagdollBehavior>();
            if (ragdoll.IsBeingCarried && ragdoll.CarryTransform.CompareTag("Player") || ragdoll.HasBeenHit)
            {
                return;
            }

            if (TurretRayCheck(other.transform.position, 1 << 0))
            {
                print("raycast to ragdoll hit obstacle");
            }
            else
            {
                target = ragdoll;

                print(target ? $"target is {other.gameObject.name}" : "no target");
                print("player hit");
            }

        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        print("turret collided with " + collision.gameObject.name + "velocity : " + collision.relativeVelocity.magnitude);
        if (collision.gameObject.CompareTag("Ragdoll") && collision.relativeVelocity.magnitude > MaxColVelocity)
        {

            KnockOver();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            print("player exited");
            CharacterControllerTarget = null; target = null;
        }
    }

}
