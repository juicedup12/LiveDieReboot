using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CustomCinemachineInputProvider : MonoBehaviour, AxisState.IInputAxisProvider
{
    [SerializeField] Player player;
    public float GetAxisValue(int axis)
    {
        if(player)
        switch (axis)
        {
            case 0: return player.look.x;
            case 1: return player.look.y;
        }
        return 0;
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
