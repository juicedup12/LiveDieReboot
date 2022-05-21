using UnityEditor;
using UnityEngine;

public class PlayerGoalCondition : MonoBehaviour, IWinCondition
{
    [SerializeField]
    private BoxCollider GoalCollider;

    private void FixedUpdate()
    {
        
    }
    public bool CheckWin()
    {
        if (gameObject.name == "Player")
            return true;
        return false;
    }

}
