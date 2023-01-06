using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformChild : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void MoveWithPlatfrom(Vector3 Translation)
    {
        transform.position += Translation;
    }

}
