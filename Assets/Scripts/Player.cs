using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class Player : MonoBehaviour
{
    [SerializeField]
    UIBehavior uiBehavior;
    private PlayerInput inputs;
    private Rigidbody rb;
    [SerializeField]
    private float speed, jumpForce, fireKillTime;
    private Vector3 walkVector;

    bool Alive = true;
    enum State
    {
        walking, jumping
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
        if (!Alive) return;
        Vector2 inputVector =  val.ReadValue<Vector2>();
        walkVector = new Vector3(inputVector.x, 0, inputVector.y);
        walkVector *= speed;
        print(val.ReadValue<Vector2>());
    }

    void Die()
    {
        Alive = false;
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

    void CheckPowerUpType(PowerUp.PowerType PwrType)
    {
        switch (PwrType)
        {
            case PowerUp.PowerType.revive:
                print("revived");
                Alive = true;
                break;
            case PowerUp.PowerType.fly:
                break;
            default:
                break;
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
            CheckPowerUpType(other.gameObject.GetComponent<PowerUp>().Power);
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
