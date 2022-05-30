using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem.Controls;
using Cinemachine;


//use this as input provider for ThirdPersonController
public class Player : MonoBehaviour
{
    RagdollBehavior ragdollB;
    UIBehavior uiBehavior;
    [SerializeField]
    private ProtoTypeInput inputs;
    private Rigidbody rb;
    [SerializeField]
    private float speed, jumpForce, fireKillTime;
    private Vector3 walkVector;
    bool HoldingObj;
    [SerializeField]
    GameObject PlayerPrefab;
    [SerializeField]
    CinemachineVirtualCamera cam;
    public CinemachineVirtualCamera deathcam;
    CharacterController charContrl;
    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;
    [HideInInspector]
    public bool Alive = true;
    bool carrying = false;
    public Transform SpawnPoint;
    enum State
    {
        walking, jumping, dead, carrying
    }
    State playerState = State.walking;

    private void OnEnable()
    {
        inputs = new ProtoTypeInput();

        inputs.UI.Disable();
        inputs.Gameplay.Enable();
        inputs.Gameplay.Movement.performed += ReadMoveInput;
        inputs.Gameplay.Movement.canceled += (InputAction.CallbackContext x) => move = Vector3.zero;
        inputs.Gameplay.Jump.performed += (ctx) => jump = true;
        inputs.Gameplay.Jump.canceled += (ctx) => jump = false;
        inputs.UI.HoldInteract.started += InputHold;
        inputs.UI.HoldInteract.performed += (ctx) =>
        {
            print("completed hold UI");
            uiBehavior.InvokeCommand();
            //add ui hold behavior later
        };
        inputs.UI.HoldInteract.canceled += (ctx) =>
        { uiBehavior.CancelRadial();  };
        inputs.Gameplay.Sprint.performed += (ctx) =>
        {
            var button = (ButtonControl)ctx.control;

            if (button.wasPressedThisFrame)
            {
                //Debug.Log($"Button {ctx.control} was pressed");
                sprint = true;
            }
            else if (button.wasReleasedThisFrame)
            {
                //Debug.Log($"Button {ctx.control} was released");
                sprint = false;
            }
        };
        inputs.Gameplay.Look.performed += (ctx) => look = ctx.ReadValue<Vector2>();
        inputs.Gameplay.Look.canceled += (ctx) => look = Vector2.zero;
        inputs.UI.PressInteract.performed += (ctx) => 
        { if (ctx.action.WasPerformedThisFrame()) uiBehavior.TapInvoke(); };
    }


    private void OnDisable()
    {
        inputs = new ProtoTypeInput();

        inputs.UI.Disable();
        inputs.Gameplay.Disable();
        inputs = new ProtoTypeInput();

        inputs.UI.Disable();
        inputs.Gameplay.Enable();
        inputs.Gameplay.Movement.performed -= ReadMoveInput;
        inputs.Gameplay.Movement.canceled -= (InputAction.CallbackContext x) => move = Vector3.zero;
        inputs.Gameplay.Jump.performed -= (ctx) => jump = true;
        inputs.Gameplay.Jump.canceled -= (ctx) => jump = false;
        inputs.UI.HoldInteract.started -= InputHold;
        inputs.UI.HoldInteract.performed -= (ctx) =>
        {
            print("completed hold UI");
            uiBehavior.InvokeCommand();
            //add ui hold behavior later
        };
        inputs.UI.HoldInteract.canceled -= (ctx) =>
        { uiBehavior.CancelRadial(); };
        inputs.Gameplay.Sprint.performed -= (ctx) =>
        {
            var button = (ButtonControl)ctx.control;

            if (button.wasPressedThisFrame)
            {
                //Debug.Log($"Button {ctx.control} was pressed");
                sprint = true;
            }
            else if (button.wasReleasedThisFrame)
            {
                //Debug.Log($"Button {ctx.control} was released");
                sprint = false;
            }
        };
        inputs.Gameplay.Look.performed -= (ctx) => look = ctx.ReadValue<Vector2>();
        inputs.Gameplay.Look.canceled -= (ctx) => look = Vector2.zero;
        inputs.UI.PressInteract.performed -= (ctx) =>
        { if (ctx.action.WasPerformedThisFrame()) uiBehavior.InvokeCommand(); };
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;

        rb = GetComponent<Rigidbody>();
        charContrl = GetComponent<CharacterController>();
        ragdollB = GetComponentInChildren<RagdollBehavior>();
        //if(ragdollB)
        //ragdollB.Attatch();
        UIBehavior ui = FindObjectOfType<UIBehavior>();

        if (ui)
        {
            print("ui behavior found: " + uiBehavior);
            uiBehavior = ui;
        }
        //GetComponent<StarterAssets.ThirdPersonController>().CinemachineCameraTarget = cam.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        { 
            if(Time.timeScale <.1)
            {
                uiBehavior.ResumeGame();
            }
            else
            {
                uiBehavior.PauseGame();

            }
        }

    }



    void ReadMoveInput(InputAction.CallbackContext val)
    {
        //if (!Alive)
        //{
        //    //walkVector = Vector2.zero;

        //    return;
        //}
        Vector2 inputVector = val.ReadValue<Vector2>();
        walkVector = new Vector3(inputVector.x, 0, inputVector.y);
        walkVector *= speed;
        print(val.ReadValue<Vector2>());
        move = val.ReadValue<Vector2>();
    }

    void Die()
    {
        Alive = false;
        playerState = State.dead;
        uiBehavior.ShowCenterText("you died");
        Time.timeScale = .5f;
        //resets time scale after button is held
        uiBehavior.ShowUIHoldButton("hold button", "E", Reboot);
        print("player died");
        inputs.UI.Enable();

        GetComponent<StarterAssets.ThirdPersonController>().enabled = false;
        charContrl.enabled = false;
        GetComponent<Animator>().enabled = false;
        deathcam.Priority = 12;
        charContrl.enabled = false;

        //rb.isKinematic = false;
        //rb.AddForce(transform.forward + transform.position * 20 + transform.up * 10, ForceMode.Impulse);

    }

    public void ClearLevel()
    {
        Alive = false;
        print("level clear");

        uiBehavior.ShowLevelClear();
        inputs.UI.Enable();
        StopAllCoroutines();
    }

    void Reboot()
    {
        inputs = null;

        print("respawning");

        Player newPlayer = Instantiate(PlayerPrefab, SpawnPoint.position, Quaternion.identity).GetComponentInChildren<Player>();
        newPlayer.SpawnPoint = this.SpawnPoint;
        newPlayer.uiBehavior = this.uiBehavior;
        newPlayer.PlayerPrefab = this.PlayerPrefab;
        newPlayer.deathcam = this.deathcam;
        cam.Follow = newPlayer.transform;
        deathcam.Priority = 8;
        uiBehavior.CommandAction -= Reboot;
        StopAllCoroutines();
        Destroy(charContrl);
        Destroy(GetComponent<StarterAssets.ThirdPersonController>());
        Destroy(GetComponent<BasicRigidBodyPush>());
        Destroy(this);
        //Destroy(transform.parent.gameObject);
    }

    void InputHold(InputAction.CallbackContext ctx)
    {
        //print("input hold started");
        SlowTapInteraction hold = ctx.interaction as SlowTapInteraction;
        if (uiBehavior)
        {
            print("starting radial routine");
            if (this)
                StartCoroutine(uiBehavior.RadialUpdateRoutine(hold.duration));

        }
    }

    void CheckDmgType(Damage dmg)
    {
        switch (dmg.DmgType)
        {
            case Damage.DamageType.Kill:
                charContrl.enabled = false;
                ragdollB.Detatch(transform.forward);
                deathcam.Follow = ragdollB.transform;
                deathcam.LookAt = ragdollB.transform;
                deathcam.Priority = 12;
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
                playerState = State.dead;
                Die();
                deathcam.LookAt = ragdollB.transform;
                deathcam.Priority = 12;
                ragdollB.Detatch(dmg.transform.forward * 10);
                //rb.AddRelativeForce((dmg.transform.right + Vector3.up * 2) * jumpForce, ForceMode.Impulse);
                //foreach (Rigidbody ragdollBone in RagDollBodies)
                //{
                //    ragdollBone.AddForce((new Vector3(dmg.transform.right.x,0)  * 5) + Vector3.up * jumpForce*2, ForceMode.VelocityChange);
                //}
                break;
            case Damage.DamageType.bullet:
                print("bullet hit player");
                playerState = State.dead;
                Die();
                charContrl.enabled = false;
                ragdollB.Detatch(dmg.GetComponent<Rigidbody>().velocity);
                deathcam.Follow = ragdollB.transform;
                deathcam.LookAt = ragdollB.transform;
                deathcam.Priority = 12;
                break;

            default:
                break;
        }
    }


    //when player is near carryable, it's data will be retrieved
    //and prompt user to pick up
    public void TargetPickUp(Carryable PickUpRB)
    {
        if (!carrying)
        {
            //activate pick up AI
            if (uiBehavior && PickUpRB)
            {
                inputs.UI.Enable();
                print("carryable obj is " + PickUpRB.gameObject.name);
                //PickUpUI.ShowPickUpUI(() => { PickUpRB.CarryTransform = transform; print($"assigning {gameObject.name} as the carrier of {PickUpRB.gameObject.name}"); PickUpRB.PickUp(); });
                uiBehavior.gameObject.SetActive(true);
                uiBehavior.ShowUIPressButton("press to pick up", "e", () =>
                {
                    carrying = true;
                    PickUpRB.PickUp(transform);
                    uiBehavior.HideUI();
                uiBehavior.ShowUIPressButton("hold to throw", "E", () => 
                    {
                    carrying = false;
                    PickUpRB.Release(transform.forward);
                    }
                    );
                });
            }
            if (!PickUpRB)
            {
                print("can't pick up");
                uiBehavior.HideUI();
                inputs.UI.Disable();
            }
        }
    }


    public void CheckPowerUpType(PowerUp Pwr)
    {
        switch (Pwr.Power)
        {
            case PowerUp.PowerType.revive:
                if (!Alive)
                {
                    
                    print("revived");
                    //Alive = true;
                    //playerState = State.walking;
                    uiBehavior.CancelRadial();
                    Pwr.gameObject.SetActive(false);
                    //ragdollB.Attatch();
                    //charContrl.enabled = true;
                    //GetComponent<StarterAssets.ThirdPersonController>().enabled = true;
                    //deathcam.Priority = 8;
                    //GetComponent<Animator>().enabled = true;
                    //charContrl.transform.position = ragdollB.transform.position;
                    Time.timeScale = 1;
                    Destroy(ragdollB.gameObject);
                    Player newPlayer = Instantiate(PlayerPrefab, Pwr.transform.position, Quaternion.identity).GetComponentInChildren<Player>();
                    newPlayer.SpawnPoint = this.SpawnPoint;
                    newPlayer.uiBehavior = this.uiBehavior;
                    newPlayer.PlayerPrefab = this.PlayerPrefab;
                    newPlayer.deathcam = this.deathcam;
                    cam.Follow = newPlayer.transform;
                    deathcam.Priority = 8;
                    uiBehavior.CommandAction -= Reboot;
                    StopAllCoroutines();
                    Destroy(charContrl);
                    Destroy(GetComponent<StarterAssets.ThirdPersonController>());
                    Destroy(GetComponent<BasicRigidBodyPush>());
                    Destroy(this);
                    //delete power object
                }
                else
                {
                    print("already alive");
                    if (!HoldingObj)
                    {
                        //activate pick up AI
                        //pickUp.ShowPickUpUI();
                        //TargetObject = Pwr.gameObject;
                    }
                    else
                    {
                    }
                }
                break;
            case PowerUp.PowerType.fly:
                break;
            default:
                break;
        }
    }

    //pressing pick up near carryable
    //void CarryObj(InputAction.CallbackContext ctx)
    //{
    //    if (!HoldingObj && PickUpUI)
    //    {
    //        uiBehavior.gameObject.SetActive(true);
    //        //uiBehavior.ShowUIHoldButton("hold to throw", "e");
    //        HoldingObj = true;


    //    }
    //    else if(HoldingObj)
    //    {
    //        //PickUpUI.ExecuteThrow();
    //        //PickUpUI.gameObject.SetActive(false);
    //    }

    //}


    IEnumerator FireTimer()
    {
        float timer = 0;
        while (timer < fireKillTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        Die();
        deathcam.Priority = 12;
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
        if (other.gameObject.layer == 7)
        {
            ClearLevel();
        }
        if (other.gameObject.layer == 8)
        {
            print("hit power up");
            CheckPowerUpType(other.gameObject.GetComponent<PowerUp>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            //PickUpUI.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (playerState == State.jumping)
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                playerState = State.walking;
            }

    }



    //private void ToggleRagdoll(bool bisAnimating)
    //{
    //    isRagdoll = !bisAnimating;
    //    charContrl.enabled = bisAnimating;
    //    foreach (Rigidbody ragdollBone in RagDollBodies)
    //    {
    //        ragdollBone.isKinematic = bisAnimating;
    //    }
    //    GetComponent<Animator>().enabled = bisAnimating;
    //    if (bisAnimating)
    //    {
    //        //RandomAnimation();

    //    }
    //}


    private void OnParticleCollision(GameObject other)
    {
        print("particle collided with " + other.gameObject);
        List<ParticleCollisionEvent> part = new List<ParticleCollisionEvent>();
        int numCollisionEvents = ParticlePhysicsExtensions.GetCollisionEvents(other.GetComponent<ParticleSystem>(), gameObject, part);
        if (part.Count > 0)
        {
            //launch player
            Die();
            //Hip.isKinematic = false;
            //Hip.AddForceAtPosition(part[0].velocity * 50, Hip.transform.position, ForceMode.Impulse);
        }

    }

}
