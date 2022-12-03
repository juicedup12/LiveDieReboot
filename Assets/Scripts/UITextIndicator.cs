using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//class for updating text on UI
//listens to player events
public class UITextIndicator : MonoBehaviour
{
    TextMeshProUGUI TextMesh;
    private void Awake()
    {
        TextMesh = GetComponent<TextMeshProUGUI>();
    }
    public void DisplayString(string text)
    {
        print(" text indicator responding to fire message with :" + text);
        TextMesh.text = text;
    }
}
