using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{


    public void LoadLevel(int level)
    {
        print("loading level" + level);
        SceneManager.LoadScene(level);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
