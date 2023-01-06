using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//deprecated
//can make carryable class as interactable instead
public class CarryInteract : MonoBehaviour, IInteractable
{
    public void interact()
    {
        //need to add condition to check if carryable is being picked up
        if (Player.Instance)
        {

            print("carry interact triggered");
            Carryable CarryComponent = GetComponent<Carryable>();
            //Transform t = Player.Instance.transform;
            //CarryComponent.BeCarriedBy(t);
            Player.Instance.CarryBody(CarryComponent);

        }
        else
            print("no player instance");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
