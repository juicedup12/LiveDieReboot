using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

//timeline signal will notify rewinder that timeline has reached
//end of clip or desired location and if rewind condition is met
//then clip will return to clip start
public abstract class TimelineRewnder : MonoBehaviour
{
    [SerializeField]
    PlayableDirector Timeline;
    [SerializeField] protected float ClipStart;


    public void CheckRewindCondition()
    {
        if(RewindConditionMet())
        {
            print("changing clip time to " + ClipStart);
            Timeline.time = (double)ClipStart;
        }
        //no need to rewind
    }

    public abstract bool RewindConditionMet();

}
