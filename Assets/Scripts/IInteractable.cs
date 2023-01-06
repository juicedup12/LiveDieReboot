using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//objects that implement interactable need a trigger collider for player to detect
public interface IInteractable 
{
    void interact();
}
