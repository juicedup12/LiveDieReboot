using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this platform can be told to go to target or return to origin by other objects
public class TriggerableMovingPlatform : MovingPlatform
{
    enum PlatformState
    {
        idle, active, returning
    }

    PlatformState CurrentState;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {

        switch (CurrentState)
        {
            case PlatformState.idle:
                break;
            case PlatformState.active:
                ElapsedTime += Time.deltaTime;
                MoveToPos(target);
                break;
            case PlatformState.returning:
                ElapsedTime += Time.deltaTime;
                MoveToPos(origin);
                break;
            default:
                break;
        }
    }

    [ContextMenu("activate")]
    public void ActivatePlatform()
    {
        ElapsedTime = 0;
        CurrentState = PlatformState.active;
        OnReachedTarget = FinishedAction;
    }

    public void DeactivatePlatform()
    {
        ElapsedTime = 0;
        CurrentState = PlatformState.returning;
        OnReachedTarget = FinishedAction;
    }

    void FinishedAction()
    {
        CurrentState = PlatformState.idle;
        OnReachedTarget = null;
    }
}
