using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem.Controls;
using Cinemachine;
using UnityEngine.Events;

//later add unity event to trigger confetti particles

//use this as input provider for ThirdPersonController
public class Player : MonoBehaviour
{
    public static Player Instance;
    RagdollBehavior ragdollB;
    UIBehavior uiBehavior;
    [SerializeField]
    private ProtoTypeInput inputs;
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
    public CharacterController GetCharacterController { get => charContrl; }
    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    [SerializeField] float LaunchPower;

    public bool IsJumpPressed;
    public bool IsSprinting;
    [HideInInspector]
    public bool Alive = true;
    bool carrying = false;
    Carryable CarryTarget;
    public Transform SpawnPoint;
    Coroutine HoldRoutine;
    [SerializeField] StringEvent IndicateFireTime;
    float HoldTimer;
    Coroutine CurrentHoldRoutine;
    Action HoldAction;
    Action InteractAction;
    public UnityEvent<Transform> OnRevive;
    public UnityEvent<Transform> OnDie;
    public UnityEvent<Transform> OnRepairPowerUp;

    enum State
    {
        walking, jumping, dead
    }
    State playerState = State.walking;

    private void Awake()
    {
        print("awake called");
        Instance = this;
    }

    private void OnEnable()
    {
        print("onEnable called");
        inputs = new ProtoTypeInput();

        inputs.UI.Disable();
        inputs.Gameplay.Enable();
        inputs.Gameplay.Movement.performed += ReadMoveInput;
        inputs.Gameplay.Movement.canceled += ReadMoveInput;
        inputs.Gameplay.Jump.performed += Jump;
        inputs.Gameplay.Jump.canceled += Jump;
        inputs.UI.HoldInteract.started += InputHold;
        inputs.UI.HoldInteract.performed += InputHold;
        inputs.UI.HoldInteract.canceled += InputHold;
        inputs.Gameplay.Sprint.performed += Sprint;
        inputs.Gameplay.Look.performed += ReadLookInput;
        inputs.Gameplay.Look.canceled += ReadLookInput;
        inputs.Gameplay.Interact.performed += InteractInput;
    }


    private void OnDisable()
    {
        print("on disable called");
        Instance = null;
        if (inputs == null) return;
        inputs.Disable();
        inputs.Gameplay.Movement.performed -= ReadMoveInput;
        inputs.Gameplay.Movement.canceled -= ReadMoveInput;
        inputs.Gameplay.Jump.performed -= Jump;
        inputs.Gameplay.Jump.canceled -= Jump;
        inputs.UI.HoldInteract.started -= InputHold;
        inputs.UI.HoldInteract.performed -= InputHold;
        inputs.UI.HoldInteract.canceled -= InputHold;
        inputs.Gameplay.Sprint.performed -= Sprint;
        inputs.Gameplay.Look.performed -= ReadLookInput;
        inputs.Gameplay.Look.canceled -= ReadLookInput;
        inputs.Gameplay.Interact.performed -= InteractInput;
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;

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
            if (!uiBehavior) return;
            if(Time.timeScale <.1)
            {
                uiBehavior.ResumeGame();
            }
            else
            {
                uiBehavior.PauseGame();

            }
        }

        if(Keyboard.current.fKey.wasPressedThisFrame)
        {
            InitialiseHoldAction(null, "TestHoldDesc");
        }

    }


    void Sprint(InputAction.CallbackContext ctx)
    {
        //var button = (ButtonControl)ctx.control;

        //if (button.wasPressedThisFrame)
        //{
        //    //Debug.Log($"Button {ctx.control} was pressed");
        //    sprint = true;
        //}
        //else if (button.wasReleasedThisFrame)
        //{
        //    //Debug.Log($"Button {ctx.control} was released");
        //    sprint = false;
        //}
        IsSprinting = ctx.ReadValueAsButton();
    }

    void Jump(InputAction.CallbackContext ctx)
    {
        //if(ctx.canceled)
        //{
        //    IsJumpPressed = false;
        //}
        //else
        //{
        //    IsJumpPressed = ctx.ReadValueAsButton();
        //}
        IsJumpPressed = ctx.ReadValueAsButton();

    }

    void ReadLookInput(InputAction.CallbackContext ctx)
    {
        look = ctx.ReadValue<Vector2>();
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
        //print(val.ReadValue<Vector2>());
        move = val.ReadValue<Vector2>();
    }

    void InitialiseHoldAction(Action OnHold, string HoldDescription)
    {

        inputs.UI.Enable();
        uiBehavior.HideUI();
        uiBehavior.ShowUIHoldButton(HoldDescription, "E");
        HoldAction = OnHold;
    }

    private void Die()
    {
        if (carrying)
        {
            ReleaseCarryBody();
        }
        ReleaseCarryBody();
        OnDie?.Invoke(ragdollB.transform);
        StopAllCoroutines();
        Alive = false;
        playerState = State.dead;

        inputs.Gameplay.Disable();
        uiBehavior.ShowCenterText("you died");
        Time.timeScale = .5f;
        //resets time scale after button is held
        //change this

        print("player died");
        InitialiseHoldAction(() => respawn(SpawnPoint.position), "reboot");
        GetComponent<StarterAssets.ThirdPersonController>().enabled = false;
        charContrl.enabled = false;
        GetComponent<Animator>().enabled = false;
        deathcam.Priority = 12;
        charContrl.enabled = false;

        //rb.isKinematic = false;
        //rb.AddForce(transform.forward + transform.position * 20 + transform.up * 10, ForceMode.Impulse);

    }

    //new logic for spawning player prefab
    void respawn(Vector3 SpawnPos)
    {
        uiBehavior.HideUI();
        deathcam.Priority = 0;
        StopAllCoroutines();
        Destroy(GetComponent<StarterAssets.ThirdPersonController>());

        Destroy(charContrl);
        Destroy(GetComponent<BasicRigidBodyPush>());
        Destroy(this);
        Player newPlayer = Instantiate(PlayerPrefab, SpawnPos, Quaternion.identity).GetComponentInChildren<Player>();
        newPlayer.transform.rotation = SpawnPoint.rotation;
        newPlayer.SpawnPoint = this.SpawnPoint;
        newPlayer.uiBehavior = this.uiBehavior;
        newPlayer.PlayerPrefab = this.PlayerPrefab;
        newPlayer.deathcam = this.deathcam;
        newPlayer.OnDie = this.OnDie;
        newPlayer.IndicateFireTime = IndicateFireTime;
        cam.Follow = newPlayer.transform;
        Time.timeScale = 1;
    }

    public void ClearLevel()
    {
        inputs.Gameplay.Disable();
        Alive = false;
        print("level clear");
        //need to change this to show hold button 
        //and add next level load logic to hold action
        InitialiseHoldAction(() => uiBehavior.GoToNextLevel(), "Hold to continue");
        uiBehavior.ShowLevelClear();
        StopAllCoroutines();
    }

    void Reboot()
    {

        print("respawning");

        StopAllCoroutines();
        OnDisable();
        

        deathcam.Priority = 8;
        Destroy(GetComponent<StarterAssets.ThirdPersonController>());

        Destroy(charContrl);
        Destroy(GetComponent<BasicRigidBodyPush>());
        Destroy(this);
        Player newPlayer = Instantiate(PlayerPrefab, SpawnPoint.position, Quaternion.identity).GetComponentInChildren<Player>();
        newPlayer.SpawnPoint = this.SpawnPoint;
        newPlayer.uiBehavior = this.uiBehavior;
        newPlayer.PlayerPrefab = this.PlayerPrefab;
        newPlayer.deathcam = this.deathcam;
        newPlayer.IndicateFireTime = IndicateFireTime;
        cam.Follow = newPlayer.transform;
        //Destroy(transform.parent.gameObject);
    }

    void InputHold(InputAction.CallbackContext ctx)
    {
        //print("input hold started");
        if (ctx.started)
        {
            if (uiBehavior)
            {
                //uiBehavior.HoldExecute(hold.duration);
                HoldInteraction hold = ctx.interaction as HoldInteraction;
                CurrentHoldRoutine = StartCoroutine(ButtonHoldRoutine(hold.duration));
            }
        }
        //need to add an action to invoke instead of just reboot
        if(ctx.performed)
        {
            print("completed hold UI");
            uiBehavior.HideUI();
            //Reboot();
            if (HoldAction == null) print("no hold action to execute");
            HoldAction?.Invoke();
            HoldAction = null;
        }
        if(ctx.canceled)
        {
            print("stopping hold routine");
            uiBehavior.SetRadialFillPercent(0);
            StopCoroutine(CurrentHoldRoutine);
        }
    }

    public IEnumerator ButtonHoldRoutine(float duration)
    {
        //if(!Radial.isActiveAndEnabled)
        //{
        //    print("no hold action");
        //    yield break;
        //}
        print("hold routine started with a duration of : " + duration);
        HoldTimer = 0;
        while (HoldTimer < duration)
        {
            //print("timer is " + timer);
            HoldTimer += Time.unscaledDeltaTime;
            uiBehavior.SetRadialFillPercent(HoldTimer / duration);
            yield return null;

        }
        print("UI timer complete");

    }

    void InteractInput(InputAction.CallbackContext ctx)
    {
        print("interact input called");

        if (carrying)
        {
            ReleaseCarryBody();
        }
        if (InteractAction != null)
        {
            print("interact action accessed by player");
            //change this to a delegate action
            InteractAction.Invoke();
            print("setting interact ation to null");
            InteractAction = null;
        }
        else print("interact action is null");

    }

    void CheckDmgType(Damage dmg)
    {
        //if (!Alive) return;
        switch (dmg.DmgType)
        {
            case Damage.DamageType.Kill:
                Vector3 velocity = charContrl.velocity;
                charContrl.enabled = false;
                //if(playerState == State.walking && walkVector.magnitude > .01)
                //{
                    velocity *= .5f;
                //}
                print("sending detatch velocity" + velocity.magnitude);
                ragdollB.Detatch(velocity);
                deathcam.Follow = ragdollB.transform;
                deathcam.LookAt = ragdollB.transform;
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
                //deathcam.Priority = 12;
                ragdollB.Detatch(dmg.transform.forward * LaunchPower);
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
                //deathcam.Follow = ragdollB.transform;
                deathcam.LookAt = ragdollB.transform;
                deathcam.Priority = 12;
                break;

            default:
                break;
        }
    }

    public void CarryBody(Carryable carrybody)
    {
        carrying = true;
        carrybody.BeCarriedBy(transform);
        CarryTarget = carrybody;
        print("showing carry UI");
        uiBehavior.ShowUIPressButton("throw", "e");
        print("setting interact action ");

        //add code to show throw body prompt
    }

    public void ReleaseCarryBody()
    {
        print("releasing carry body");
        if (!CarryTarget) return;
        CarryTarget.Release(transform.forward);
        carrying = false;
        uiBehavior.HideUI();
    }

    //when player is near carryable, it's data will be retrieved
    //and prompt user to pick up
    public void TargetPickUp(Carryable PickUpRB)
    {
        if (!carrying && Alive)
        {
            //activate pick up UI
            if (uiBehavior && PickUpRB)
            {
                inputs.UI.Enable();
                print("carryable obj is " + PickUpRB.gameObject.name);
                //PickUpUI.ShowPickUpUI(() => { PickUpRB.CarryTransform = transform; print($"assigning {gameObject.name} as the carrier of {PickUpRB.gameObject.name}"); PickUpRB.PickUp(); });
                uiBehavior.ShowUIPressButton("press to pick up", "e");
            }
            if (!PickUpRB)
            {
                print("away from carryable");
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
                    OnRepairPowerUp?.Invoke(transform);
                    print("revived");
                    respawn(Pwr.transform.position);
                    Destroy(gameObject);
                    return;


                    //Alive = true;
                    //playerState = State.walking;
                    uiBehavior.HideUI();
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

    //use fire event
    IEnumerator FireTimer()
    {
        IndicateFireTime?.Invoke(fireKillTime.ToString());
        float timer = fireKillTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            if(timer % 1 < .1)
            {
                print("invoking fire event");
                IndicateFireTime?.Invoke(Mathf.Round(timer).ToString()) ;
            }
            if(timer < 1)
            {
                float f = Mathf.Round(timer * 10.0f) * 0.1f;
                IndicateFireTime?.Invoke(f.ToString());
            }
            yield return null;
        }
        ragdollB.Detatch(charContrl.velocity * .5f);
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
        

        if(other.TryGetComponent(out IInteractable Target))
        {
            if (!Alive || carrying) return;
            InteractAction = Target.interact;
            print("interacting with : " + Target);
            uiBehavior.ShowUIPressButton("carry", "E");
        }
    }



    private void OnTriggerExit(Collider other)
    {
        if (!Alive || carrying) return;
        if(other.TryGetComponent(out IInteractable Target) && !carrying)
        {
            print("away from interactable");
            InteractAction = null;
            print("setting interact action to null");
            uiBehavior.HideUI();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("player enter collision " + collision.gameObject.name);
        if (playerState == State.jumping)
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                playerState = State.walking;
            }

        if(collision.gameObject.CompareTag("Turret"))
        {
            print("player entered collision turret");
            collision.gameObject.GetComponent<CharacterShootPredict>().KnockOver();
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


    [System.Serializable]
    public class StringEvent : UnityEvent<string>
    {

    }
}
