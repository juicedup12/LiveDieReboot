using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallGenerator : MonoBehaviour
{
    public GameObject ThrowObject;
    public Vector3 ThroweVec;
    public float ThrowForce;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {

                 ThrowObject.transform.position = transform.position + Vector3.up;
                 ThrowObject.GetComponentInChildren<RagdollBehavior>().Fly(ThroweVec * ThrowForce);
              
        }
    }
}

