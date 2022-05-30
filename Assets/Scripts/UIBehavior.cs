using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UIBehavior : MonoBehaviour
{

    [SerializeField]
    private GameObject PlayerPrefab; 
    [SerializeField]
    private TextMeshProUGUI LevelStatusText, RadialButtonText, Command, SquareInput; 
    [SerializeField]
    private Image Radial;
    [SerializeField]
    SceneLoader loader;
    [SerializeField]
    private Image SquareImg;
    public Action CommandAction;
    public GameObject PauseParent;
    public float timer;
    public int level;

    private void Start()
    {
        loader = GetComponent<SceneLoader>();
    }

    public void ShowGameOver()
    {

        ShowCenterText("You died");

    }

    public void ShowCenterText(string txt)
    {
        LevelStatusText.gameObject.SetActive(true);
        LevelStatusText.text = txt;

    }

    public void ShowUIPressButton(string textFunction, string ButtonIndicator, Action callback)
    {
        SquareImg.gameObject.SetActive(true);
        Command.gameObject.SetActive(true);
        RadialButtonText.text = ButtonIndicator;
        Command.text = textFunction;
        CommandAction = callback;
    }
    public void TapInvoke()
    {
        if (SquareInput.gameObject.activeSelf)
        {
            print("ui command action execute");
            CommandAction?.Invoke();
            CommandAction = null;
            //HideUI();
        }
    }

    
    public void InvokeCommand()
    {
        print("invoking command while timer is as " + timer);
        if (timer > .9) 
        CommandAction?.Invoke();
        CommandAction = null;
    }

    public void HideUI()
    {
        print("hiding UI");
        SquareImg.gameObject.SetActive(false);
        Command.gameObject.SetActive(false);
        Radial.gameObject.SetActive(false);
        LevelStatusText.gameObject.SetActive(false);

    }



    public void ShowUIHoldButton(string TextFunction, string ButtonTextIndicator, Action callback)
    {
        print("showing UI");
        Radial.gameObject.SetActive(true);
        Command.gameObject.SetActive(true);
        RadialButtonText.text = ButtonTextIndicator;
        Command.text = TextFunction;
        CommandAction = callback;
    }

    public void restartLevel()
    {
        loader.LoadLevel(level);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public IEnumerator RadialUpdateRoutine(float duration)
    {
        //if(!Radial.isActiveAndEnabled)
        //{
        //    print("no hold action");
        //    yield break;
        //}
        print("coroutine started");
        timer = 0;
        while(timer < duration)
        {
            //print("timer is " + timer);
            timer += Time.unscaledDeltaTime;
            Radial.fillAmount = timer;
            yield return null;

        }
        print("UI timer complete");

        HideUI();
        CommandAction?.Invoke();
        CommandAction = null;
        Radial.fillAmount = 0;
    }

    public void ShowLevelClear()
    {
        HideUI();
        print(" level clear");
        LevelStatusText.text = "Level Clear!";
        LevelStatusText.gameObject.SetActive(true);
        Radial.gameObject.SetActive(true);
        RadialButtonText.text = "e";
        Command.gameObject.SetActive(true);
        Command.text = "Press button to continue";
        CommandAction = () => { loader.LoadLevel(level +1); print("loading level" + level + 1); };
    }

    public void CancelRadial()
    {
        Radial.fillAmount = 0;
        print("cancelling routine");
        StopCoroutine("RadialUpdateRoutine");
        StopAllCoroutines();
    }


    public void PauseGame()
    {
        Time.timeScale = 0;
        PauseParent.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        PauseParent.SetActive(false);
    }

}
