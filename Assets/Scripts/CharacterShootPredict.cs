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
    RagdollBehavior trg;
    CharacterController c;
    [SerializeField]
    Material HostileMat, FriendlyMat;
    Renderer rndr;
    bool IsFriendly;
    Rigidbody rb;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        timer = Time.time;
        particles = GetComponentInChildren<ParticleSystem>();
        rndr = GetComponent<Renderer>();
        HostileMat = rndr.material;

    }

    void Update()
    {
        if (!target || IsFriendly) return;

        if (trg) {
            if (trg.HasBeenHit) { target = null; trg = null; return; }
            targetPosition = trg.transform.position;
            targetVelocity = trg.hip.velocity;
        }
        if(c)
        {
            if(!c.enabled)
            {
                c = null;
                target = null;
                return;
            }
        }
        
        if (!trg && target is RagdollBehavior)
        {
            trg = (RagdollBehavior)target;
        }
        if(!c && target is CharacterController)
        {
            c = (CharacterController)target;
        }
        if (target is CharacterController)
        {
            
            targetPosition = c.transform.position;
            targetVelocity = c.velocity;
        }
        //print("target controller is " + target.gameObject.name + $"/n position is {targetPosition}" + " velocity is " + targetVelocity);

        Vector3 IC = CalculateInterceptCourse(targetPosition, targetVelocity, transform.position, projectileSpeed);

        //print("intercept course is " + IC);
        if (HaveCourse = IC != Vector3.zero)
        {
            IC.Normalize();
            interceptionTime1 = FindClosestPointOfApproach(targetPosition, targetVelocity, transform.position, IC * projectileSpeed);
            interceptionPoint.position = targetPosition + targetVelocity * interceptionTime1;
        }

        transform.LookAt(interceptionPoint);

        if (HaveCourse)
            if (Time.time > timer)
            {
                //print("can shoot");
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

        Rigidbody B = Instantiate(BulletPrefab, transform.position + transform.forward, transform.rotation).GetComponent<Rigidbody>(); ;
        B.velocity = transform.forward * projectileSpeed;
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

        Vector3 dir = ObjPos - transform.position;

        Ray ray = new(transform.position, dir);

        Debug.DrawRay(transform.position, dir, Color.green, 4);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 15, lmask))
        {
            print("raycast hit " + hit.transform.gameObject.name);
             Debug.DrawRay(transform.position, hit.point - transform.position, Color.red, 1);
            return true;
        }

        return false;
        
    }

    public void Friendly()
    {
        rndr.material = FriendlyMat;
        IsFriendly = true;
    }

    public void hostile()
    {
        rndr.material = HostileMat;
        transform.parent = null;
        IsFriendly = false;
        rb.isKinematic = true;

    }

    public void KnockOver(Vector3 forward)
    {

        rb.isKinematic = false;
        Friendly();
        rb.AddForce(forward);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (target|| IsFriendly) return;



        //if player enters, raycast in player layer and get charcontroller velocity
        if (other.CompareTag("Player"))
        {
            print($"player in view of turret ({other.transform})");
            if (TurretRayCheck(other.transform.position + Vector3.up, 1 << 10))
            {
                target = other.GetComponent<CharacterController>();
                print(target ? $"target is {other.gameObject.name}" : "no target");
                //print("player hit");
            }
            else print("nothing hit in raycast");

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


        //if ragdoll enters raycast in limb layer get rigidbody velocity
        if (other.CompareTag("Ragdoll"))
        {
            print($"ragdoll in view of turret ({other.transform})");

            if (!other.GetComponent<RagdollBehavior>().IsBeingCarried)
            {
                if (TurretRayCheck(other.transform.position, 1 << 11))
                {
                    target = other.GetComponent<RagdollBehavior>();

                    print(target ? $"target is {other.gameObject.name}" : "no target");
                    //print("player hit");
                }
            }
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            print("player exited");
            c = null; target = null;
        }
    }

}
