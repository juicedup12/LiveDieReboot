using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


[TrackBindingType(typeof(Material))]
[TrackClipType(typeof(WipeMatClip))]
public class MaterialTrack : TrackAsset
{
    //public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    //{
        //return ScriptPlayable<WipeMatMixer>.Create(graph, inputCount);

    //}
}
