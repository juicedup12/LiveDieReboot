using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TMProClip : PlayableAsset, ITimelineClipAsset
{
    public TMProBehaviour template = new TMProBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.Extrapolation | ClipCaps.Blending; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<TMProBehaviour>.Create (graph, template);
        //was put here by boilerplate
        //TMProBehaviour clone = playable.GetBehaviour ();
        return playable;
    }
}
