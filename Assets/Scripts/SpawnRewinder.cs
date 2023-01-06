using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//will rewind timeline if spawn points have not been traversed fully
public class SpawnRewinder : TimelineRewnder
{

    SpawnIterate spawner;

    private void Awake()
    {
        spawner = GetComponent<SpawnIterate>();
    }

    public override bool RewindConditionMet()
    {
        //checks if spawn iterate's current point is equal to the amount of spawn points
        return !spawner.CompletedIteration;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
