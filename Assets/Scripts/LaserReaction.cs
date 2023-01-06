using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LaserReaction : MonoBehaviour
{

    public UnityEvent OnLaserEnter;
    public UnityEvent OnLaserExit;
    [SerializeField] Material InnerMat;
    [SerializeField] Material OuterMat;
    Material[] mats;
    Renderer r;

    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Renderer>();
        mats = r.materials;
        SetNewMaterials();
    }


    void SetNewMaterials()
    {
        InnerMat = new Material(InnerMat);
        OuterMat = new Material(OuterMat);
        mats[0] = OuterMat;
        mats[1] = InnerMat;
        r.materials = mats;
    }

    void InactiveMatChange()
    {
        OuterMat.color = Color.red;
        OuterMat.SetColor("_BaseColor", Color.black);
        InnerMat.DisableKeyword("_EMISSION");
    }

    void ActiveMatChange()
    {
        OuterMat.color = Color.green;
        InnerMat.EnableKeyword("_EMISSION");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Laser"))
        {
            print("laser hit react");
            OnLaserEnter?.Invoke();
            ActiveMatChange();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Laser"))
        {
            print("laser exit react");
            OnLaserExit?.Invoke();
            InactiveMatChange();
        }
    }
}
