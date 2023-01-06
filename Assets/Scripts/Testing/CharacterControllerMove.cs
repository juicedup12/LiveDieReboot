using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterControllerMove : MonoBehaviour
{
    CharacterController character;
    [SerializeField] float speed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.qKey.isPressed)
        {
            moveCharacter(Vector3.down);
        }
        if(Keyboard.current.wKey.isPressed)
        {
            moveCharacter(transform.forward);
        }
        if (Keyboard.current.sKey.isPressed)
        {
            moveCharacter(transform.transform.forward * -1);
        }
        if (Keyboard.current.aKey.isPressed)
        {
            moveCharacter(transform.right * -1);
        }
        if (Keyboard.current.dKey.isPressed)
        {
            moveCharacter(transform.right);
        }
    }

    void moveCharacter(Vector3 move)
    {
        character.Move(move * speed * Time.deltaTime);
    }
}
