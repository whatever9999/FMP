// Uncomment for debug logging
#define DEBUG_UTILITY_AI

using UnityEngine;

public class Director : MonoBehaviour
{
    public enum DirectorGoalType
    {
        DECREASE_NEED_HUNGER,
        DECREASE_NEED_FUN,
        DECREASE_NEED_COMFORT,
        DECREASE_NEED_SOCIAL,
        DECREASE_NEED_BLADDER,
        DECREASE_NEED_HYGIENE,
        DECREASE_NEED_SLEEP,
        DECREASE_NEED_ENVIRONMENT,

        DECREASE_SKILL_CLEANING,
        DECREASE_SKILL_HANDINESS,
        DECREASE_SKILL_COOKING,
        DECREASE_SKILL_PHOTOGRAPHY,
        DECREASE_SKILL_DANCING,
        DECREASE_SKILL_PROGRAMMING,
        DECREASE_SKILL_GAMING,
        DECREASE_SKILL_DARTS,

        INCREASE_MADNESS_CHANCE,
        NUM_GOAL_TYPES,
    }

    private enum Bounds
    {
        LOWER,
        UPPER,
    }

    [SerializeField] private DirectorActionData[] actions;

    [SerializeField] private DifficultyData relaxDifficultyData;
    [SerializeField] private DifficultyData buildUpDifficultyData;

    private float maxGoalInsistency = 100;

    [Tooltip("How often the director checks for an action")]
    [SerializeField] private float directorTimer = 60;
    private float currentDirectorTimer;
    private float menaceTimer;
    private float currentMenaceTimer;

    private DifficultyData currentDifficulty;

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
        }
    }

    // Utility AI
    private void ChooseAction()
    {
#if DEBUG_UTILITY_AI
        for (DirectorGoalType i = 0; i < DirectorGoalType.NUM_GOAL_TYPES; i++)
        {
            Debug.Log("Insistency " + i + ": " + GetGoalInsistency(i));
        }
#endif // DEBUG_UTILITY_AI
        DirectorActionData bestAction = actions[0];
        float bestValue = CalculateDiscontentment(actions[0]);

        for (int i = 1; i < actions.Length; i++)
        {
            float thisValue = CalculateDiscontentment(actions[i]);

            // We want the action that yields the lowest amount of discontentment
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

        for(DirectorGoalType i = 0; i < DirectorGoalType.NUM_GOAL_TYPES; i++)
        {
            // We subtract as we want to lower insistency
            // E.g. decrease hunger insistency = 80, a chance card increases hunger need (-5) so 80 -- 5 = 85
            // Or card decreases hunger need (5) so 80 - 5 = 75
            float newValue = GetGoalInsistency(i) - action.insistencyChanges[(int)i];
            // E.g. 85/100 = 8.5 (high discontentment)
            // Or 75/100 = 7.5 (lower discontentment)
            float thisDiscontentment = GetDiscontentment(newValue);
            discontentment += thisDiscontentment;
#if DEBUG_UTILITY_AI
            Debug.Log("Discontentment of " + i + " = " + thisDiscontentment);
#endif // DEBUG_UTILITY_AI
        }

#if DEBUG_UTILITY_AI
        Debug.Log("Final discontentment = " + discontentment);
#endif // DEBUG_UTILITY_AI
        return discontentment;
    }
    // E.g. 0/10 = 0, 5/10 = 0.5, -5/10 = -0.5 so there is more discontentment the higher the insistency value is
    private float GetDiscontentment(float newInsistencyValue)
    {
        return newInsistencyValue / maxGoalInsistency;
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

        float performanceMetric = PerformanceM.GetPerformanceMetric();
        SetMenaceTimer(performanceMetric);
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

    // Insistency values (All between 0 and 100)
    private float GetGoalInsistency(DirectorGoalType type)
    {
        switch (type)
        {
            case DirectorGoalType.DECREASE_NEED_HUNGER:
                return ManagerHandler.instance.NeedsM.GetNeedValue(NeedsManager.NeedType.HUNGER);
            case DirectorGoalType.DECREASE_NEED_FUN:
                return ManagerHandler.instance.NeedsM.GetNeedValue(NeedsManager.NeedType.FUN);
            case DirectorGoalType.DECREASE_NEED_COMFORT:
                return ManagerHandler.instance.NeedsM.GetNeedValue(NeedsManager.NeedType.COMFORT);
            case DirectorGoalType.DECREASE_NEED_SOCIAL:
                return ManagerHandler.instance.NeedsM.GetNeedValue(NeedsManager.NeedType.SOCIAL);
            case DirectorGoalType.DECREASE_NEED_BLADDER:
                return ManagerHandler.instance.NeedsM.GetNeedValue(NeedsManager.NeedType.BLADDER);
            case DirectorGoalType.DECREASE_NEED_HYGIENE:
                return ManagerHandler.instance.NeedsM.GetNeedValue(NeedsManager.NeedType.HYGIENE);
            case DirectorGoalType.DECREASE_NEED_SLEEP:
                return ManagerHandler.instance.NeedsM.GetNeedValue(NeedsManager.NeedType.SLEEP);
            case DirectorGoalType.DECREASE_NEED_ENVIRONMENT:
                return ManagerHandler.instance.NeedsM.GetNeedValue(NeedsManager.NeedType.ENVIRONMENT);

            case DirectorGoalType.DECREASE_SKILL_CLEANING:
                return ManagerHandler.instance.SkillM.GetSkillLevel(SkillManager.SkillType.CLEANING) * 10;
            case DirectorGoalType.DECREASE_SKILL_HANDINESS:
                return ManagerHandler.instance.SkillM.GetSkillLevel(SkillManager.SkillType.HANDINESS) * 10;
            case DirectorGoalType.DECREASE_SKILL_COOKING:
                return ManagerHandler.instance.SkillM.GetSkillLevel(SkillManager.SkillType.COOKING) * 10;
            case DirectorGoalType.DECREASE_SKILL_PHOTOGRAPHY:
                return ManagerHandler.instance.SkillM.GetSkillLevel(SkillManager.SkillType.PHOTOGRAPHY) * 10;
            case DirectorGoalType.DECREASE_SKILL_DANCING:
                return ManagerHandler.instance.SkillM.GetSkillLevel(SkillManager.SkillType.DANCING) * 10;
            case DirectorGoalType.DECREASE_SKILL_PROGRAMMING:
                return ManagerHandler.instance.SkillM.GetSkillLevel(SkillManager.SkillType.PROGRAMMING) * 10;
            case DirectorGoalType.DECREASE_SKILL_GAMING:
                return ManagerHandler.instance.SkillM.GetSkillLevel(SkillManager.SkillType.GAMING) * 10;
            case DirectorGoalType.DECREASE_SKILL_DARTS:
                return ManagerHandler.instance.SkillM.GetSkillLevel(SkillManager.SkillType.DARTS) * 10;

            case DirectorGoalType.INCREASE_MADNESS_CHANCE:
                float madnessChance = 0;
                madnessChance += ManagerHandler.instance.NeedsM.GetNeedValue(NeedsManager.NeedType.SOCIAL);
                madnessChance += ManagerHandler.instance.NeedsM.GetNeedValue(NeedsManager.NeedType.FUN);
                madnessChance += ManagerHandler.instance.NeedsM.GetNeedValue(NeedsManager.NeedType.ENVIRONMENT);
                return madnessChance / 3;
        }
        return 0;
    }
}
