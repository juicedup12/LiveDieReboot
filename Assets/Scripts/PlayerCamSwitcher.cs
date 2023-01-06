using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using Cinemachine;


//a class that stops time and makes the screen black and white
//while the player is switching to another player
public class PlayerCamSwitcher : MonoBehaviour
{
    [SerializeField] CinemachineBrain brain;
    [SerializeField] Volume vol;
    ColorCurves ColCurves;

    private void Start()
    {
        if(vol.profile.TryGet(out ColorCurves colcurves))
        {
            ColCurves = colcurves;
        }
    }

    //if this doesn't work, use update method instead
    public void StartCamSwitch()
    {
        Time.timeScale = 0;
        //set screen black and white
        //
        ColCurves.active = true;
        StartCoroutine(SwitchRoutine());
    }

    IEnumerator SwitchRoutine()
    {
        //yield return new WaitForEndOfFrame();
        //yield return new WaitForEndOfFrame();
        yield return null;

        if (!brain.IsBlending)
        {
            print("error, brain isn't blending ");
            yield break;
        }
        print("brain is blending: " + brain.IsBlending);
        float dur = brain.ActiveBlend.Duration;

        print("switch routine going for " + dur + " seconds");
        yield return new WaitForSecondsRealtime(dur);
        Time.timeScale = 1;

        ColCurves.active = false;
    }
    
}
