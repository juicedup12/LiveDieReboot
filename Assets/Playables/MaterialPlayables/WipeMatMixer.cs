using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class WipeMatMixer : PlayableBehaviour
{
    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        Material MaterialBinding = playerData as Material;
        float direction = 0f;
        float step = 0f;

        if (!MaterialBinding)
            return;

        int inputCount = playable.GetInputCount(); //get the number of all clips on this track

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<WipeMatBehavior> inputPlayable = (ScriptPlayable<WipeMatBehavior>)playable.GetInput(i);
            WipeMatBehavior input = inputPlayable.GetBehaviour();

            // Use the above variables to process each frame of this playable.
            direction += input.direction * inputWeight;
            step += input.stepAmount * inputWeight;
        }

        //assign the result to the bound object

        //need to round values
        MaterialBinding.SetFloat("_StepAmount", step);
        MaterialBinding.SetFloat("_Direction", direction);
    }
}
