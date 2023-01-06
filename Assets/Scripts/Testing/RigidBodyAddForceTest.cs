using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RigidBodyAddForceTest : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float force = 20;
    [SerializeField] float speed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.rKey.isPressed)
        {
            //rb.AddForce(new Vector3(0, force));
            //Player.Instance.GetCharacterController.Move(Vector3.up * force);
            StartCoroutine(Move());
        }
    }

    IEnumerator Move()
    {
        for (float i = 0; i < 1; i+= Time.deltaTime)
        {
            transform.Translate(Vector3.up * Time.fixedDeltaTime * speed, Space.Self);
            yield return null;
        }
    }

}
