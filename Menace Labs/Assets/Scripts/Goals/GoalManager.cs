using UnityEngine;
using UnityEngine.UI;

public class GoalManager : MonoBehaviour
{
    [SerializeField] private Image goalIconImage;
    [SerializeField] private GoalData[] goalDatas;

    #region Goal Stats Tracking
    private int mealsMade = 0;
    public int GetMealsMade() { return mealsMade; }
    public void ModifyMealsMade(int amount) { mealsMade += amount; }

    private int firesSurvived = 0;
    public int GetFiresSurvived() { return firesSurvived; }
    public void ModifyFiresSurvived(int amount) { firesSurvived += amount; }

    private float hoursDancing = 0;
    public float GetHoursDancing() { return hoursDancing; }
    public void ModifyHoursDancing(float amount) { hoursDancing += amount; }

    private int timesCleaned = 0;
    public int GetTimesCleaned() { return timesCleaned; }
    public void ModifyTimesCleaned(int amount) { timesCleaned += amount; }
    #endregion // Goal Stats Tracking

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

    private void Awake()
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

    public float GetGoalMetric()
    {
        return goal.GetGoalMetric();
    }
}
