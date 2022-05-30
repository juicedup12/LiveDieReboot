using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class PickUpObjUI : MonoBehaviour
{
    TextMeshProUGUI text;
    Action OnPress;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        gameObject.SetActive(false);
    }

    public void ShowPickUpUI(Action PressCallback)
    {
        //show text of button to pick up
        gameObject.SetActive(true);
        text.text = "Press E to pick up";
        OnPress = PressCallback;
    }

    public void ShowThrowUI()
    {
        //show text of button to throw
        text.text = "Press E to throw";
        OnPress?.Invoke();
    }

    public void ExecuteThrow()
    {
        OnPress?.Invoke();
    }

}
