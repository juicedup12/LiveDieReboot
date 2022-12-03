using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RigidbodyMouseMove : MonoBehaviour
{
    [SerializeField]
    RagdollBehavior rd;
    Camera cam;
    [SerializeField]
    float speed;
    public GameObject Ragdoll;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (!rd)
            {
                rd = Instantiate(Ragdoll, transform.position, Quaternion.identity).GetComponentInChildren<RagdollBehavior>();
                rd.BeCarriedBy(transform);
                print("carrying activate");
            }    
            else
            {
                //rd.Release();
                rd.Release(transform.forward);
            }
        }
        if (!rd) return;
        RaycastHit hit;
        Vector3 coor = Mouse.current.position.ReadValue();
        if (Physics.Raycast(cam.ScreenPointToRay(coor), out hit, 20, 1<<3))
        {

            transform.position = hit.point + Vector3.up * 3 ;
            Debug.DrawRay(hit.point, transform.up * 3, Color.blue, .2f);
        }
        else
        {
            //print("no ground");
        }
    }
}
