using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIBehavior : MonoBehaviour
{

    [SerializeField]
    private GameObject LevelStatus; 
    [SerializeField]
    private TextMeshProUGUI LevelStatusText, ButtonPrompt, Command; 
    [SerializeField]
    private Image Radial;


    public void ShowGameOver()
    {
        gameObject.SetActive(true);
        LevelStatusText.text = "You died";
        ButtonPrompt.text = "e";
        Command.text = "Press button to retry";
    }

    public void StartUIHold(float duration)
    {
        StartCoroutine("RadialUpdateRoutine", 1);
    }

    public IEnumerator RadialUpdateRoutine(float duration)
    {
        print("coroutine started");
        float timer = 0;
        while(timer < duration)
        {
            //print("timer is " + timer);
            timer += Time.deltaTime;
            Radial.fillAmount = timer;
            yield return null;
        }
        print("UI timer complete");
    }


    public void CancelRadial()
    {
        Radial.fillAmount = 0;
        print("cancelling routine");
        StopCoroutine("RadialUpdateRoutine");
    }

}
