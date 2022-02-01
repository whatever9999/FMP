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
        UIManager.instance.SetGoal(goal.name, goal.goal, goal.description, goal.story);
    }

    private void Update()
    {
        bool goalComplete = true;
        switch (goal.goalType)
        {
            case GoalType.MEDIOCRE_MASTER:
                // Check through all skills to see if they have been achieved
                for (int i = 0; i < (int)SkillManager.SkillType.NONE; i++)
                {
                    if (SkillManager.instance.GetSkillLevel((SkillManager.SkillType)i) < 10) goalComplete = false;
                }
                break;
            case GoalType.CONQUEROR_OF_CUISINE:
                break;
            case GoalType.FAMOUS_FIREFIGHTER:
                break;
            case GoalType.FOOTLOOSE_FIEND:
                break;
            case GoalType.DOMESTIC_DELIGHT:
                break;
            case GoalType.NUM_GOAL_TYPES:
                break;
        }

        if (goalComplete)
        {
            UIManager.instance.PUM.ShowWinGame();
        }
    }
}
