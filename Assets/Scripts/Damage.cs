using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public enum DamageType
    {
        Kill, Fire, Cut, launch, bullet
    }
    public DamageType DmgType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(DmgType == DamageType.bullet)
        {
            if (transform.position.magnitude > 100)
                Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
    }
}
