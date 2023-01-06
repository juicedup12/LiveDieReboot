using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index, false);
        print("graphics quality is now " + QualitySettings.GetQualityLevel() );
    }
}
