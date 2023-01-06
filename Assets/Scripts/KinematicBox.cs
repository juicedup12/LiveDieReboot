using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicBox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 3)
        {
            print("cube hit ground");
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
