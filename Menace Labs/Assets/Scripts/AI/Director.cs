// Uncomment for debug logging
#define DEBUG_UTILITY_AI

using UnityEngine;
using System.Collections.Generic;

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
        // Only do so if the clone isn't away testing or dying Oo
        if (!ManagerHandler.instance.ActionM.IsCloneTesting() && !ManagerHandler.instance.ActionM.IsCloneDying())
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

        // Each action has a weight that will be used to randomly choose one
        float[] weights = new float[actions.Length];
        float totalWeight = 0;

        for (int i = 0; i < actions.Length; i++)
        {
            float actionWeight = CalculateUtility(actions[i]);
            weights[i] = totalWeight + actionWeight;
            totalWeight += actionWeight;
        }

        // Randomly choose an action using the weights
        float rand = Random.Range(0, totalWeight);
#if DEBUG_UTILITY_AI
        Debug.Log("Rand (0 - totalWeight) = " + rand);
#endif // DEBUG_UTILITY_AI
        float bottomWeight = 0;
        for (int i = 0; i < actions.Length; i++)
        {
            if (rand >= bottomWeight && rand < weights[i])
            {
#if DEBUG_UTILITY_AI
                Debug.Log("Triggering action " + actions[i].name);
#endif // DEBUG_UTILITY_AI
                actions[i].TriggerAction();
                break;
            }
            else
            {
                bottomWeight = weights[i];
            }
        }
    }

    // A value between 0 and 1
    private float CalculateUtility(DirectorActionData action)
    {
        float utility = 0;

        // Chance cards also have to consider utility for if the card succeeds
        CardActionData cardAction = action as CardActionData;

        // Sum the utility of each goal
        for (DirectorGoalType i = 0; i < DirectorGoalType.NUM_GOAL_TYPES; i++)
        {
            // The higher the goal insistency, the more useful an action that has a high utility for that goal
            utility += action.insistencyChanges[(int)i] * GetGoalInsistency(i);

            // Chance cards also have to consider utility for if the card succeeds
            if (cardAction)
            {
                utility += cardAction.cardSuccessInsistencyChanges[(int)i] * GetGoalInsistency(i);
            }
        }
        // Normalise the value (*2 for chance cards as they consider two sets of utilities)
        if (cardAction)
        {
            utility /= (int)DirectorGoalType.NUM_GOAL_TYPES * 2;
        }
        else
        {
            utility /= (int)DirectorGoalType.NUM_GOAL_TYPES;
        }

        // Adjust utility considering chance
        if (cardAction)
        {
            // The director wants the chance card to fail so there's a higher utility in failure
            float optionAFailure = 100 - cardAction.chanceCard.GetChance(ChanceCardData.OptionChoice.OPTION_A);
            float optionBFailure = 100 - cardAction.chanceCard.GetChance(ChanceCardData.OptionChoice.OPTION_B);
            float failureChance = (optionAFailure + optionBFailure) / 2;

            utility *= (failureChance / 100);
        }
        else
        {
            // Occurrence and disaster don't need to be adjusted as they're 100% but change change will be adjusted by the new value
            ChanceChangeActionData chanceChangeAction = action as ChanceChangeActionData;
            if (chanceChangeAction)
            {
                utility *= (chanceChangeAction.newValue / 100);
            }
        }

#if DEBUG_UTILITY_AI
        Debug.Log("Utility of " + action.name + ": " + utility);
#endif // DEBUG_UTILITY_AI
        return utility;
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

    // Insistency values (Between 0 and 1)
    private float GetGoalInsistency(DirectorGoalType type)
    {
        switch (type)
        {
            // Needs are on a quadratic curve
            case DirectorGoalType.DECREASE_NEED_HUNGER:
                return Mathf.Pow(ManagerHandler.instance.NeedsM.GetNeedValue(NeedsManager.NeedType.HUNGER) / 100, 2);
            case DirectorGoalType.DECREASE_NEED_FUN:
                return Mathf.Pow(ManagerHandler.instance.NeedsM.GetNeedValue(NeedsManager.NeedType.FUN) / 100, 2);
            case DirectorGoalType.DECREASE_NEED_COMFORT:
                return Mathf.Pow(ManagerHandler.instance.NeedsM.GetNeedValue(NeedsManager.NeedType.COMFORT) / 100, 2);
            case DirectorGoalType.DECREASE_NEED_SOCIAL:
                return Mathf.Pow(ManagerHandler.instance.NeedsM.GetNeedValue(NeedsManager.NeedType.SOCIAL) / 100, 2);
            case DirectorGoalType.DECREASE_NEED_BLADDER:
                return Mathf.Pow(ManagerHandler.instance.NeedsM.GetNeedValue(NeedsManager.NeedType.BLADDER) / 100, 2);
            case DirectorGoalType.DECREASE_NEED_HYGIENE:
                return Mathf.Pow(ManagerHandler.instance.NeedsM.GetNeedValue(NeedsManager.NeedType.HYGIENE) / 100, 2);
            case DirectorGoalType.DECREASE_NEED_SLEEP:
                return Mathf.Pow(ManagerHandler.instance.NeedsM.GetNeedValue(NeedsManager.NeedType.SLEEP) / 100, 2);
            case DirectorGoalType.DECREASE_NEED_ENVIRONMENT:
                return Mathf.Pow(ManagerHandler.instance.NeedsM.GetNeedValue(NeedsManager.NeedType.ENVIRONMENT) / 100, 2);

            case DirectorGoalType.DECREASE_SKILL_CLEANING:
                return ManagerHandler.instance.SkillM.GetSkillLevel(SkillManager.SkillType.CLEANING) / 10;
            case DirectorGoalType.DECREASE_SKILL_HANDINESS:
                return ManagerHandler.instance.SkillM.GetSkillLevel(SkillManager.SkillType.HANDINESS) / 10;
            case DirectorGoalType.DECREASE_SKILL_COOKING:
                return ManagerHandler.instance.SkillM.GetSkillLevel(SkillManager.SkillType.COOKING) / 10;
            case DirectorGoalType.DECREASE_SKILL_PHOTOGRAPHY:
                return ManagerHandler.instance.SkillM.GetSkillLevel(SkillManager.SkillType.PHOTOGRAPHY) / 10;
            case DirectorGoalType.DECREASE_SKILL_DANCING:
                return ManagerHandler.instance.SkillM.GetSkillLevel(SkillManager.SkillType.DANCING) / 10;
            case DirectorGoalType.DECREASE_SKILL_PROGRAMMING:
                return ManagerHandler.instance.SkillM.GetSkillLevel(SkillManager.SkillType.PROGRAMMING) / 10;
            case DirectorGoalType.DECREASE_SKILL_GAMING:
                return ManagerHandler.instance.SkillM.GetSkillLevel(SkillManager.SkillType.GAMING) / 10;
            case DirectorGoalType.DECREASE_SKILL_DARTS:
                return ManagerHandler.instance.SkillM.GetSkillLevel(SkillManager.SkillType.DARTS) / 10;

            case DirectorGoalType.INCREASE_MADNESS_CHANCE:
                float madnessNeeds = 0;
                madnessNeeds += ManagerHandler.instance.NeedsM.GetNeedValue(NeedsManager.NeedType.SOCIAL);
                madnessNeeds += ManagerHandler.instance.NeedsM.GetNeedValue(NeedsManager.NeedType.FUN);
                madnessNeeds += ManagerHandler.instance.NeedsM.GetNeedValue(NeedsManager.NeedType.ENVIRONMENT);
                return madnessNeeds / 300;
        }
        return 0;
    }
}
