using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using TMPro;

public class TMProMixerBehaviour : PlayableBehaviour
{
    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        TextMeshProUGUI trackBinding = playerData as TextMeshProUGUI;

        if (!trackBinding)
            return;

        int inputCount = playable.GetInputCount ();
        Color blendedColor = Color.clear;
        float totalWeight = 0f;
        float greatestWeight = 0f;
        int currentInputs = 0;


        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<TMProBehaviour> inputPlayable = (ScriptPlayable<TMProBehaviour>)playable.GetInput(i);
            TMProBehaviour input = inputPlayable.GetBehaviour ();

            // Use the above variables to process each frame of this playable.
            blendedColor += input.Color * inputWeight;
            totalWeight += inputWeight;

            if (inputWeight > greatestWeight)
            {
                greatestWeight = inputWeight;
            }

            if (!Mathf.Approximately(inputWeight, 0f))
                currentInputs++;
        }
        //removed default color from screenfad mixer behavior
        trackBinding.color = blendedColor + Color.white * (1f - totalWeight);

    }
}
