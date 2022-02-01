using UnityEngine;
using UnityEngine.UI;

public class GoalManager : MonoBehaviour
{
    [SerializeField] private Image goalIconImage;
    [SerializeField] private GoalData[] goalDatas;

    public enum GoalType
    {
        MEDIOCRE_MASTER,
        CONQUEROR_OF_CUISINE,
        FAMOUS_FIREFIGHTER,
        FOOTLOOSE_FIEND,
        DOMESTIC_DELIGHT,
        NUM_GOAL_TYPES,
    }

    private GoalData goal;

    private void Start()
    {
        int rand = Random.Range(0, goalDatas.Length);

        goal = goalDatas[rand];

        // Update UI
        goalIconImage.sprite = goal.goalIcon;
        ManagerHandler.instance.UIM.SetGoal(goal.name, goal.goal, goal.description, goal.story);
    }

    private void Update()
    {
        if (goal.IsGoalComplete())
        {
            ManagerHandler.instance.PopupM.ShowWinGame();
        }
    }
}
