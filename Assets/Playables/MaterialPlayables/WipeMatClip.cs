using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class WipeMatClip : PlayableAsset
{
    public WipeMatBehavior template;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<WipeMatBehavior>.Create(graph, template);
        return playable;
    }
}

[System.Serializable]
public class WipeMatBehavior : PlayableBehaviour
{
    public float direction = 0;
    public float stepAmount = 0;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        Material MaterialBinding = playerData as Material;

        if (!MaterialBinding)
            return;
        MaterialBinding.SetFloat("_StepAmount", stepAmount);
        MaterialBinding.SetFloat("_Direction", direction);
    }
}
