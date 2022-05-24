using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PickUpObjUI : MonoBehaviour
{
    TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        gameObject.SetActive(false);
    }

    public void ShowPickUpUI()
    {
        //show text of button to pick up
        gameObject.SetActive(true);
        text.text = "Press E to pick up";
    }

    public void ShowThrowUI()
    {
        //show text of button to throw
        text.text = "Press E to throw";
    }

}
