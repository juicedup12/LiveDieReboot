using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTextCameraRotate : MonoBehaviour
{
    Transform campos;
    // Start is called before the first frame update
    void Start()
    {
        campos = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 camDir = campos.position - transform.position;
        transform.LookAt(transform.position - camDir);
    }
}
