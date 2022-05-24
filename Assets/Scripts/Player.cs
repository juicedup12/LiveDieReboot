using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class Player : MonoBehaviour
{
    [SerializeField]
    UIBehavior uiBehavior;
    [SerializeField]
    PickUpObjUI pickUp;
    private PlayerInput inputs;
    private Rigidbody rb;
    [SerializeField]
    private float speed, jumpForce, fireKillTime;
    private Vector3 walkVector;
    GameObject TargetObject;
    GameObject HeldObj;


    bool Alive = true;
    enum State
    {
        walking, jumping, dead
    }
    State playerState;

    private void OnEnable()
    {

        inputs = new PlayerInput();
        inputs.Gameplay.Enable();
        inputs.Gameplay.Movement.performed += ReadMoveInput;
        inputs.Gameplay.Movement.canceled += (InputAction.CallbackContext x) => walkVector = Vector3.zero;
        inputs.Gameplay.Jump.performed += PerformJump;
        inputs.UI.Accept.started += InputHold;
        inputs.Gameplay.PickUp.performed += CarryObj;
        inputs.UI.Accept.performed += (cx) => print("input hold complete" );
        inputs.UI.Accept.canceled += (ctx) => {uiBehavior.CancelRadial();};
    }


    private void OnDisable()
    {
        inputs.Gameplay.Movement.performed -= ReadMoveInput;
        inputs.Gameplay.Movement.canceled -= (InputAction.CallbackContext x) => walkVector = Vector3.zero;
        inputs.Gameplay.Jump.performed += PerformJump;
        inputs.UI.Accept.started -= InputHold;
        inputs.UI.Accept.performed -= (cx) => print("input hold complete");
        inputs.UI.Accept.canceled -= (ctx) => { uiBehavior.CancelRadial();};
        inputs.Gameplay.Disable();
        inputs.UI.Disable();
        inputs.Gameplay.PickUp.performed -= CarryObj;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        switch (playerState)
        {
            case State.walking:
                rb.MovePosition(walkVector + transform.position);
                break;
            case State.jumping:
                break;
            case State.dead:
                //rb.AddRelativeForce(walkVector + transform.position);
                break;
            default:
                break;
        }
    }

    void PerformJump(InputAction.CallbackContext callbackContext)
    {
        playerState = State.jumping;
        //rb.AddForce(Vector3.up * jumpForce);
        rb.AddRelativeForce((Vector3.up + walkVector * 2 )* jumpForce , ForceMode.Impulse);
    }

    void ReadMoveInput(InputAction.CallbackContext val)
    {
        if (!Alive)
        {
            //walkVector = Vector2.zero;
            
            return;
        }
        Vector2 inputVector =  val.ReadValue<Vector2>();
        walkVector = new Vector3(inputVector.x, 0, inputVector.y);
        walkVector *= speed;
        print(val.ReadValue<Vector2>());
    }

    void Die()
    {
        Alive = false;
        playerState = State.dead;
        uiBehavior.ShowGameOver();
        print("player died");
        inputs.UI.Enable();
    }

    void ClearLevel()
    {
        Alive = false;
        uiBehavior.ShowLevelClear();
        print("level clear");
        inputs.UI.Enable();
    }


    void InputHold(InputAction.CallbackContext ctx)
    {
        print("input hold started");
        SlowTapInteraction hold = ctx.interaction as SlowTapInteraction;
        if(uiBehavior)
        uiBehavior.StartUIHold(hold.duration);
    }

    void CheckDmgType(Damage dmg)
    {
        switch (dmg.DmgType)
        {
            case Damage.DamageType.Kill:
                Die();
                break;
            case Damage.DamageType.Fire:
                StartCoroutine(FireTimer());
                print("fire");
                break;
            case Damage.DamageType.Cut:
                transform.localScale *= .5f;
                print("touched saw");
                break;
            case Damage.DamageType.launch:
                print("player launched");
                playerState = State.jumping;
                rb.AddRelativeForce((dmg.transform.right + Vector3.up * 2) * jumpForce, ForceMode.Impulse);

                break;

            default:
                break;
        }
    }

    void CheckPowerUpType(PowerUp Pwr)
    {
        switch (Pwr.Power)
        {
            case PowerUp.PowerType.revive:
                if (!Alive)
                {
                    print("revived");
                    Alive = true;
                    playerState = State.walking;
                    uiBehavior.CancelRadial();
                    Pwr.gameObject.SetActive(false);
                    uiBehavior.gameObject.SetActive(false);
                    
                    //delete power object
                }
                else
                {
                    if (!pickUp) return;
                    if (!HeldObj)
                    {
                        //activate pick up AI
                        pickUp.ShowPickUpUI();
                        TargetObject = Pwr.gameObject;
                    }
                    else
                    {
                        pickUp.ShowThrowUI();
                    }
                }
                break;
            case PowerUp.PowerType.fly:
                break;
            default:
                break;
        }
    }

    void CarryObj(InputAction.CallbackContext ctx)
    {
        if (!HeldObj && TargetObject)
        {
            HeldObj = TargetObject;
            TargetObject = null;
            HeldObj.GetComponent<Collider>().enabled = false;
            HeldObj.transform.parent = transform;
            HeldObj.transform.localPosition = Vector3.forward;
            pickUp.ShowThrowUI();
        }
        else if(HeldObj)
        {
            HeldObj.transform.parent = null;

            HeldObj.GetComponent<Collider>().enabled = true;
            HeldObj = null;
            pickUp.gameObject.SetActive(false);
        }

    }


    IEnumerator FireTimer()
    {
        float timer = 0;
        while(timer < fireKillTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        Die();
        print("timer finished");
    }

    private void OnTriggerEnter(Collider other)
    {
        print("collided with " + other.gameObject.name);
        int layer = 6;
        int layermask = 1 << layer;
        //print("bit mask is " + layermask);
        //print("layer bit is " + other.gameObject.layer.ToString());
        if (other.gameObject.layer == 6)
        {
            CheckDmgType(other.gameObject.GetComponentInParent<Damage>());
        }
        if(other.gameObject.layer == 7)
        {
            ClearLevel();
        }
        if(other.gameObject.layer == 8)
        {
            CheckPowerUpType(other.gameObject.GetComponent<PowerUp>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == 9)
        {
            pickUp.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(playerState == State.jumping)
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
                playerState = State.walking;
        }

    }

}
