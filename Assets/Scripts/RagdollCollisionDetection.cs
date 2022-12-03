using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollCollisionDetection : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //print("powerup detection hit " + other.gameObject);
       if( other.TryGetComponent(out PowerUp power))
        {
            print($"{gameObject } detected { other} power up");
            Player.Instance.CheckPowerUpType(power);
        }

       if(other.CompareTag("Finish"))
        {
            Player.Instance.ClearLevel();
        }
    }
}
