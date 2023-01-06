using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ParticleEmitOnMouse : MonoBehaviour
{
    [SerializeField]
    ParticleEmitWithvelocityLifetime emitSingular;
    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 coor = Mouse.current.position.ReadValue();
            RaycastHit hit;
            if (Physics.Raycast(cam.ScreenPointToRay(coor), out hit, 20, 1 << 3))
            {
                transform.position = hit.point;
                emitSingular.EmitParticle(transform);
            }
        }
    }
}
