using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Switch : MonoBehaviour
{
    public UnityEvent OnSwitchPress;
    public UnityEvent OnSwitchRelease;
    Material SwitchMat; 
    bool HasCollided;
    Collider col;

    // Start is called before the first frame update
    void Start()
    {
        Renderer r = GetComponent<Renderer>();
        if (!r) return;
        SwitchMat = new Material(r.material);
        r.material = SwitchMat;
    }

    // Update is called once per frame
    void Update()
    {
        if (HasCollided)
        {
            if (!col)
            {
                print("col has been removed");
                HasCollided = false;
                OnSwitchRelease?.Invoke();
                print("switch released");
                if (SwitchMat)
                    SwitchMat.SetColor("_BaseColor", Color.red);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            OnSwitchPress?.Invoke();
            print("switch pressed");
            if(SwitchMat)
            SwitchMat.SetColor("_BaseColor", Color.green);
            HasCollided = true;
            col = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            OnSwitchRelease?.Invoke();
            print("switch released");
            if(SwitchMat)
            SwitchMat.SetColor("_BaseColor", Color.red);
            if(other = col)
            {
                col = null;
                HasCollided = false;
            }
        }
    }
}
