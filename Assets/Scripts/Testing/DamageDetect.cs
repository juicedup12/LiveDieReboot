using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this script is used for testing damage detection without using player class
public class DamageDetect : MonoBehaviour
{
    [SerializeField] RagdollBehavior ragdollB;
    bool launched = false;
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Damage dmg) && !launched)
        {
            if(dmg.DmgType == Damage.DamageType.launch)
            {
                ragdollB.Detatch(dmg.transform.forward);
                launched = true;
            }
        }
    }
}
