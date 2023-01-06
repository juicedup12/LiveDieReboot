using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivesRewinder : TimelineRewnder
{
     LivesCounter lives;

    public override bool RewindConditionMet()
    {
        if (!lives.ReachedMaxLives())
        {
            return true;
        }
        print("timeline rewind condition not met");
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        lives = GetComponent<LivesCounter>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
