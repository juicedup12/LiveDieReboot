using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using TMPro;

[TrackColor(0f, 0.5f, 1f)]
[TrackClipType(typeof(TMProClip))]
[TrackBindingType(typeof(TextMeshProUGUI))]
public class TMProTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<TMProMixerBehaviour>.Create (graph, inputCount);
    }
}
