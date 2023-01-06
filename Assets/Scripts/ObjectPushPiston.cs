using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

//the piston that pushes objects upwards
//get references to an interface when objects are on platform
public class ObjectPushPiston : MonoBehaviour
{

    Rigidbody rb;
    [SerializeField] float PushStrength;
    public ThirdPersonController target;
    [SerializeField] MovingPlatform platform;
    bool PistonActive = false;
    public bool IsPistonActive { get => PistonActive; }
    bool Raising;
    float time;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            //PushObjects();
            InitialisePlatformRaise();
        }

    }

    private void FixedUpdate()
    {
        if(PistonActive)
        {
            time += Time.deltaTime;
            if(Raising)
            {
                platform.MoveToTarget(time);
            }
            else
            {
                platform.MoveToOrigin(time);
            }
        }
    }

    public void InitialisePlatformRaise()
    {
        if (PistonActive) return;
        print("initialising platform");
        PistonActive = true;
        Raising = true;
        time = 0;
        platform.TimeToReach = .5f;
        platform.OnReachedTarget = () => { PushObjects(); InitialisePlatfromLower(); };
    }

    void InitialisePlatfromLower()
    {
        Raising = false;
        time = 0;
        platform.TimeToReach = 1.5f;
        platform.OnReachedTarget = () => { PistonActive = false; platform.OnReachedTarget = null; };
    }

    void MovePlatform()
    {

    }


    //will eventually use interface instead of just tpc
    void PushObjects()
    {
        if (target)
            target.SetJumpVelocity(PushStrength);
    }

    private void OnTriggerEnter(Collider other)
    {
        print("piston collided with " + other.gameObject);
        if(other.CompareTag("Player"))
        {
            if(other.TryGetComponent(out ThirdPersonController tpc))
            {
                target = tpc;
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        print(other.gameObject + " exited piston");
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out ThirdPersonController tpc))
            {
                target = null;
            }

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            target = null;
        }
    }
}
