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
    public GameObject PauseParent;
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

    public void ShowUIPressButton(string textFunction, string ButtonIndicator)
    {
        print("showing UI description for " + textFunction);
        SquareImg.gameObject.SetActive(true);
        Command.gameObject.SetActive(true);
        RadialButtonText.text = ButtonIndicator;
        Command.text = textFunction;

    }

    

    public void HideUI()
    {
        print("hiding UI");
        StopAllCoroutines();
        SquareImg.gameObject.SetActive(false);
        Command.gameObject.SetActive(false);
        Radial.gameObject.SetActive(false);
        LevelStatusText.gameObject.SetActive(false);

    }

    public void SetRadialFillPercent(float percent)
    {
        Radial.fillAmount = percent;
    }

    public void ShowUIHoldButton(string TextFunction, string ButtonTextIndicator )
    {
        if (!Radial.isActiveAndEnabled)
        print("showing UI and asigning callback to radial complete");
        Radial.gameObject.SetActive(true);
        Command.gameObject.SetActive(true);
        RadialButtonText.text = ButtonTextIndicator;
        Command.text = TextFunction;
    }


    public void HoldExecute(float duration)
    {
        print("starting radial routine");

        if (Radial.IsActive()) ;
        //StartCoroutine(RadialUpdateRoutine(duration));
    }

    public void restartLevel()
    {
        loader.LoadLevel(level);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    //need to move this to player script
    //public IEnumerator RadialUpdateRoutine(float duration)
    //{
    //    //if(!Radial.isActiveAndEnabled)
    //    //{
    //    //    print("no hold action");
    //    //    yield break;
    //    //}
    //    print("coroutine started");
    //    timer = 0;
    //    while(timer < duration)
    //    {
    //        //print("timer is " + timer);
    //        timer += Time.unscaledDeltaTime;
    //        Radial.fillAmount = timer;
    //        yield return null;

    //    }
    //    print("UI timer complete");

    //    HideUI();
    //    CommandAction?.Invoke();
    //    CommandAction = null;
    //    Radial.fillAmount = 0;
    //}

    //need to move this to player class
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
        //CommandAction = () => { loader.LoadLevel(level +1); print("loading level" + level + 1); };
    }

    public void GoToNextLevel()
    {
        loader.LoadLevel(level + 1);
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
