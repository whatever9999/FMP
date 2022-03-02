// Uncomment for debug logging
#define DEBUG_UTILITY_AI

using UnityEngine;

public class Director : MonoBehaviour
{
    private enum Bounds
    {
        LOWER,
        UPPER,
    }

    [SerializeField] private DirectorActionData[] actions;

    [SerializeField] private DifficultyData relaxDifficultyData;
    [SerializeField] private DifficultyData buildUpDifficultyData;

    [Tooltip("If a goal's insistency is at this level then the AI really needs to work on it, if it's at -ve this then there's no concern about it at all")]
    [SerializeField] private float maxGoalInsistency = 100;

    [Tooltip("How often the director checks for an action")]
    [SerializeField] private float directorTimer = 60;
    private float currentDirectorTimer;
    private float menaceTimer;
    private float currentMenaceTimer;

    private DifficultyData currentDifficulty;

    private float[] currentGoalInsistencies;

    private PerformanceMetric PerformanceM;
    private MenaceMetric MenaceM;

    // Stores the chance card for the test to trigger
    private ChanceCardData chanceCard;
    public void SetChanceCard(ChanceCardData card) { chanceCard = card; }
    public ChanceCardData GetChanceCard() { return chanceCard; }

    private void Start()
    {
        PerformanceM = FindObjectOfType<PerformanceMetric>();
        MenaceM = FindObjectOfType<MenaceMetric>();

        currentGoalInsistencies = new float[relaxDifficultyData.goalInsistencies.Length];
        ChangeDifficulty();
    }

    private void Update()
    {
        // Update timers
        // Only do so if the clone isn't away testing
        if (!ManagerHandler.instance.ActionM.IsCloneTesting())
        {
            // We only update the menace timer if we're in the correct menace range
            currentDirectorTimer += Time.deltaTime;
            float currentMenace = MenaceM.GetMenaceMetric();
            if (currentMenace >= currentDifficulty.menaceBounds[0] && currentMenace <= currentDifficulty.menaceBounds[1])
            {
                currentMenaceTimer += Time.deltaTime;
            }
            // If we're not in the correct menace range the timer should be at 0
            else
            {
                currentMenaceTimer = 0;
            }

            // If it's time for the director to carry our an action they should do so (unless we're relaxing)
            if (currentDirectorTimer >= directorTimer && currentDifficulty != relaxDifficultyData)
            {
                // The director uses utility AI to determine what action it wants to take
                ChooseAction();
                currentDirectorTimer = 0;
            }

            // Once we've been at this menace level long enough we should move to the next one
            if (currentMenaceTimer >= menaceTimer)
            {
                ChangeDifficulty();
                currentMenaceTimer = 0;
            }
            else
            {
                IncreaseInsistencies();
            }
        }
    }

    // Utility AI
    private void ChooseAction()
    {
#if DEBUG_UTILITY_AI
        for (int i = 0; i < currentGoalInsistencies.Length; i++)
        {
            Debug.Log("Insistency " + (DirectorGoal.GoalType)i + ": " + currentGoalInsistencies[i]);
        }
#endif // DEBUG_UTILITY_AI
        DirectorActionData bestAction = actions[0];
        float bestValue = CalculateDiscontentment(actions[0]);

        for (int i = 1; i < actions.Length; i++)
        {
            float thisValue = CalculateDiscontentment(actions[i]);

            if (thisValue < bestValue)
            {
#if DEBUG_UTILITY_AI
                Debug.Log("Changing action from " + bestAction.name + " to " + actions[i].name + " for value of " + thisValue);
#endif // DEBUG_UTILITY_AI
                bestValue = thisValue;
                bestAction = actions[i];
            }
        }

#if DEBUG_UTILITY_AI
        Debug.Log("Triggering action " + bestAction.name);
#endif // DEBUG_UTILITY_AI
        bestAction.TriggerAction();
    }
    private float CalculateDiscontentment(DirectorActionData action)
    {
#if DEBUG_UTILITY_AI
        Debug.Log("Calculate discontentment for " + action.name);
#endif // DEBUG_UTILITY_AI

        float discontentment = 0;

        for(int i = 0; i < currentGoalInsistencies.Length; i++)
        {
            float newValue = currentGoalInsistencies[i] + action.insistencyChanges[i];
            float thisDiscontentment = GetDiscontentment(newValue);
            discontentment += thisDiscontentment;
#if DEBUG_UTILITY_AI
            Debug.Log("Discontentment of " + (DirectorGoal.GoalType)i + " = " + thisDiscontentment);
#endif // DEBUG_UTILITY_AI
        }

#if DEBUG_UTILITY_AI
        Debug.Log("Final discontentment = " + discontentment);
#endif // DEBUG_UTILITY_AI
        return discontentment;
    }
    // E.g. 0/10 = 0, 5/10 = 0.5, 100/10 = 10 so there is more discontentment the higher the insistency value is
    private float GetDiscontentment(float newInsistencyValue)
    {
        return newInsistencyValue / maxGoalInsistency;
    }

    // Insistencies need to increase by DirectorGoal.increaseValue for each director tick
    private void IncreaseInsistencies()
    {
        for (int i = 0; i < currentDifficulty.goalInsistencies.Length; i++)
        {
            currentGoalInsistencies[i] += currentDifficulty.goalInsistencies[i].increaseValue;
        }
    }

    // If we're relaxing then change to build up, if we're building up reset values and try to relax
    private void ChangeDifficulty()
    {
        if (!currentDifficulty || currentDifficulty.difficultyType == DifficultyData.DifficultyType.RELAX)
        {
            currentDifficulty = buildUpDifficultyData;
        }
        else if (currentDifficulty.difficultyType == DifficultyData.DifficultyType.BUILD_UP)
        {
            // Reset any values the director may have changed (e.g. occurrence chances) so the player can take a breather in low difficulty
            ResetValues();
            currentDifficulty = relaxDifficultyData;
        }

        SetInsistencies();

        float performanceMetric = PerformanceM.GetPerformanceMetric();
        SetMenaceTimer(performanceMetric);
    }
    private void SetInsistencies()
    {
        for (int i = 0; i < currentDifficulty.goalInsistencies.Length; i++)
        {
            currentGoalInsistencies[i] = currentDifficulty.goalInsistencies[i].baseValue;
        }
    }
    // The higher the performance, the shorter the time we'll spend relaxing and the more time we'll spend building up
    private void SetMenaceTimer(float performanceMetric)
    {
        float performanceEffect = 1 - performanceMetric;
        if (currentDifficulty.difficultyType == DifficultyData.DifficultyType.BUILD_UP)
        {
            performanceEffect = performanceMetric;
        }

        menaceTimer = ((currentDifficulty.timeBounds[(int)Bounds.UPPER] - currentDifficulty.timeBounds[(int)Bounds.LOWER]) * performanceEffect) + currentDifficulty.timeBounds[(int)Bounds.LOWER];
    }

    private void ResetValues()
    {
        ManagerHandler.instance.EventM.ModifyEventOccurrence(EventManager.EventType.BREAKING, 3);
        ManagerHandler.instance.EventM.ModifyEventOccurrence(EventManager.EventType.DIRTYING, 3);
        ManagerHandler.instance.EventM.ModifyEventChance(EventManager.EventType.ELECTROCUTION, 20);
        ManagerHandler.instance.EventM.ModifyEventChance(EventManager.EventType.FIRE, 30);
    }
}
