using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextParser : MonoBehaviour
{
    TextMeshProUGUI tmptext;
    private void Start()
    {
        tmptext = GetComponent<TextMeshProUGUI>();
    }
    public void SetFloatText(float f)
    {
        tmptext.text = f.ToString();
    }
}
