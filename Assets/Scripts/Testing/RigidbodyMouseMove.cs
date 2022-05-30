using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RigidbodyMouseMove : MonoBehaviour
{
    [SerializeField]
    RagdollBehavior rd;
    Rigidbody hip;
    Camera cam;
    [SerializeField]
    float speed;
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
            if (!hip)
            {
                //hip = rd.PickUp();
                print("carrying activate");
            }    
            else
            {
                //rd.Release();
                hip = null;
            }
        }
        if (!hip) return;
        RaycastHit hit;
        Vector3 coor = Mouse.current.position.ReadValue();
        if (Physics.Raycast(cam.ScreenPointToRay(coor), out hit, 20, 1<<3))
        {

            hip.MovePosition(hit.point + Vector3.up * 3 );
            Debug.DrawRay(hit.point, transform.up * 3, Color.blue, .2f);
        }
        else
        {
            //print("no ground");
        }
    }
}
