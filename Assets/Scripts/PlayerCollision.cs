using System.Collections;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField]
    UIBehavior ui;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        print("collided with " + other.gameObject.name);
        if(other.name == "player")
        {
            print("game won");
            ui.ShowLevelClear();
        }
    }
}
