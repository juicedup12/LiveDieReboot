using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{


    public void LoadLevel(int level)
    {

        if (SceneManager.sceneCountInBuildSettings < level + 1)
        {
            print("no scene " + level);
            return;
        }
            print("loading level" + level);
        SceneManager.LoadScene(level);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
