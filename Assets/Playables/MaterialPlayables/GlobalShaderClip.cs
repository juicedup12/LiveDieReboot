using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GlobalShaderClip : PlayableAsset
{
    public GlobalShaderBehavior template;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<GlobalShaderBehavior>.Create(graph, template);
        return playable;
    }
}

[System.Serializable]
public class GlobalShaderBehavior : PlayableBehaviour
{

    //public ShaderKeyword keyword;
    public string name;
    public float value;
    private int ShaderID;
    public void setShaderID()
    {
        ShaderID = Shader.PropertyToID(name);
        Debug.Log("shader id from " + name + " is " + ShaderID);
    }

    public void assignShaderValue()
    {
        //Debug.Log("assigning " + value + " to " + name);
        Shader.SetGlobalFloat(ShaderID, value);
    }

    public override void OnGraphStart(Playable playable)
    {
        Debug.Log("shader keyword name is " + name + " with value " + value);
        setShaderID();
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        
        assignShaderValue();
        
    }
}

