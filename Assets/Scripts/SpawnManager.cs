using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using Cinemachine;


//need to be able to pass in spawn location
//need to account for player number for player switching
//player number is passed in and player switch index reference is updated to instantiated object
//need to re-enable player class when spawning is finished
//need to disable shadows when player is disolving in
//need to give spawned players a reference to this
//class when they are instantiated
public class SpawnManager : MonoBehaviour
{
    //renderer will change 
    Renderer SpawnObjRenderer;
    public GameObject SpawnObject;
    Material[] ObjectMaterials;
    [SerializeField] Material DissolveMaterial;
    [SerializeField] PlayableDirector SpawnTimeline;
    [SerializeField] Transform SpawnPoint;
    Player CurrentPlayer;
    [SerializeField] CinemachineVirtualCamera SpawnLookCam;
    //on player respawned used for updating current player in playerswitcher 
    public UnityEvent<Player> OnPlayerRespawned;
    //switcher will respond by add spawned player to array 
    public UnityEvent<Player> OnPlayerSpawned;
    GameObject ObjectToDestroy;
    

    private void OnEnable()
    {
        SpawnTimeline.stopped += AssignObjectMats;
        SpawnTimeline.stopped += EnablePlayer;
    }  
    
    private void OnDisable()
    {
        SpawnTimeline.stopped -= AssignObjectMats;
        SpawnTimeline.stopped -= EnablePlayer;
    }


    private void Start()
    {
        //StartSpawn();
    }

    public void StartSpawn(Transform SpawnPos, GameObject ObjToDestroy)
    {
        SpawnPoint = SpawnPos;
        SpawnTimeline.Play();
        SpawnLookCam.Follow = SpawnPoint;
        SpawnLookCam.LookAt = SpawnPoint;
        ObjectToDestroy = ObjToDestroy;
        GetObjRenderer(ObjToDestroy);
        AssignDissolveMat(SpawnObjRenderer);
    }
    public void StartSpawn(Transform SpawnPos)
    {
        SpawnPoint = SpawnPos;
        SpawnTimeline.Play();
        SpawnLookCam.Follow = SpawnPoint;
        SpawnLookCam.LookAt = SpawnPoint;
    }

    public void instantiateObject()
    {
        GameObject obj = Instantiate(SpawnObject, SpawnPoint.position, SpawnPoint.rotation);
        CurrentPlayer = obj.GetComponentInChildren<Player>();
        CurrentPlayer.SetSpawner = this;
        CurrentPlayer.SpawnPoint = SpawnPoint;
        CurrentPlayer.RegisterCamera();
        OnPlayerRespawned?.Invoke(CurrentPlayer);
        obj.SetActive(true);
        GetObjRenderer(obj);
        GetObjectMats();
        AssignDissolveMat(SpawnObjRenderer);
    }

    public void InstatiateNew()
    {
        GameObject obj = Instantiate(SpawnObject, SpawnPoint.position, SpawnPoint.rotation);
        CurrentPlayer = obj.GetComponentInChildren<Player>();
        CurrentPlayer.SetSpawner = this;
        CurrentPlayer.SpawnPoint = SpawnPoint;
        CurrentPlayer.RegisterCamera();
        OnPlayerSpawned?.Invoke(CurrentPlayer);
        obj.SetActive(true);
        GetObjRenderer(obj);
        GetObjectMats();
        AssignDissolveMat(SpawnObjRenderer);
    }

    public void DestroyObj()
    {
        if(ObjectToDestroy)
        {
            Destroy(ObjectToDestroy);
            ObjectToDestroy = null;
        }
        else
        {
            print("no obj to destroy in spawnManager");
        }
    }

    void AssignDissolveMat(Renderer renderer)
    {
        print("spawn obj materials is " + renderer.materials.Length);
        Material[] mats = renderer.materials;
        for (int i = 0; i < renderer.materials.Length; i++)
        {
            mats[i] = DissolveMaterial;
            print("assigined dissolve mat to obj");
        }
        SpawnObjRenderer.materials = mats;

    }

    public void AssignObjectMats(PlayableDirector dir)
    {
        if (SpawnObjRenderer == null || ObjectMaterials == null)
        {
            print("no renderer or object materials");
            return;
        }
        print("assigning object materials");
        SpawnObjRenderer.materials = ObjectMaterials;
    }

    void EnablePlayer(PlayableDirector p)
    {
        if(CurrentPlayer)
        CurrentPlayer.enabled = true;
    }

    void GetObjRenderer(GameObject obj)
    {
        SpawnObjRenderer = obj.GetComponent<Renderer>();
        print("spawn renderer from obj is " + SpawnObjRenderer);
        if (SpawnObjRenderer == null)
        {
            SpawnObjRenderer = obj.GetComponentInChildren<Renderer>();
            print("obj render from childre in " + SpawnObjRenderer);
            return;
        }
        if (SpawnObjRenderer == null)
        {
            print("cant spawn without renderer");
            return;
        }
    }

    public void GetObjectMats()
    {
        ObjectMaterials = SpawnObjRenderer.materials;
    }
}
