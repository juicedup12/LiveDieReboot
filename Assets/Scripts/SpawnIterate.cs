using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//will update timeline 
//
public class SpawnIterate : MonoBehaviour
{

    [SerializeField] Transform[] SpawnPoints;
    int CurrentPoint;
    public bool CompletedIteration { get =>  CurrentPoint == SpawnPoints.Length; }

    //for testing 
    //dummyspawn point is a stand in for spawnmanager's spawn point
    [SerializeField] Transform SpawnPoint;


    public void SetSpawn()
    {
        SpawnPoint.position = SpawnPoints[CurrentPoint].position;
        CurrentPoint++;
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
