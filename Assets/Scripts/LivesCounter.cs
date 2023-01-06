using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LivesCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    int lives = 0;
    [SerializeField] int MaxLives;

   

    void UpdateText()
    {
        text.text = "Lives: " + lives;
    }

    public void AddLives()
    {
        print("adding lives current lives is " + lives);
        lives++;
        print(" lives addes is " + lives);
        UpdateText();
    }

    public bool ReachedMaxLives()
    {
        if (lives >= MaxLives) return true;
        print("current lives is " + lives);
        return false;
    }
}
