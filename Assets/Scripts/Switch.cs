using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Switch : MonoBehaviour
{
    public UnityEvent OnSwitchPress;
    Material SwitchMat;

    // Start is called before the first frame update
    void Start()
    {
        SwitchMat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") 
        OnSwitchPress?.Invoke();
        print("switch pressed");
        SwitchMat.SetColor("_BaseColor", Color.green);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            OnSwitchPress?.Invoke();
        print("switch released");
        SwitchMat.SetColor("_BaseColor", Color.red);
    }
}
