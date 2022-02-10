using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/GoalData")]
public class GoalData : ScriptableObject
{
    public GoalManager.GoalType goalType;
    public string goalName;
    public string goal;
    [TextArea(15, 20)]
    public string description;
    [TextArea(15, 20)]
    public string story;
    public Sprite goalIcon;

    public bool IsGoalComplete()
    {
        bool goalComplete = true;
        switch (goalType)
        {
            case GoalManager.GoalType.MEDIOCRE_MASTER:
                // Check through all skills to see if they have been achieved
                for (int i = 0; i < (int)SkillManager.SkillType.NONE; i++)
                {
                    if (ManagerHandler.instance.SkillM.GetSkillLevel((SkillManager.SkillType)i) < SkillManager.MAX_SKILL_LEVEL) goalComplete = false;
                }
                break;
            case GoalManager.GoalType.CONQUEROR_OF_CUISINE:
                // Made 100 meals
                if (ManagerHandler.instance.GoalM.GetMealsMade() < 100) goalComplete = false;
                break;
            case GoalManager.GoalType.FAMOUS_FIREFIGHTER:
                // Survived 5 fires
                if (ManagerHandler.instance.GoalM.GetFiresSurvived() < 5) goalComplete = false;
                break;
            case GoalManager.GoalType.FOOTLOOSE_FIEND:
                // Danced 100 hours
                if (ManagerHandler.instance.GoalM.GetHoursDancing() < 100) goalComplete = false;
                break;
            case GoalManager.GoalType.DOMESTIC_DELIGHT:
                // Cleaning, cooking and handiness must be maxed and must have cleaned 100 times
                if (ManagerHandler.instance.GoalM.GetTimesCleaned() < 100 ||
                    ManagerHandler.instance.SkillM.GetSkillLevel(SkillManager.SkillType.COOKING) < SkillManager.MAX_SKILL_LEVEL ||
                    ManagerHandler.instance.SkillM.GetSkillLevel(SkillManager.SkillType.CLEANING) < SkillManager.MAX_SKILL_LEVEL ||
                    ManagerHandler.instance.SkillM.GetSkillLevel(SkillManager.SkillType.HANDINESS) < SkillManager.MAX_SKILL_LEVEL  )
                {
                    goalComplete = false;
                }
                break;
        }
        return goalComplete;
    }

    public float GetGoalMetric()
    {
        float goalMetric = 0.0f;
        switch (goalType)
        {
            case GoalManager.GoalType.MEDIOCRE_MASTER:
                // Add all skill levels
                for (int i = 0; i < (int)SkillManager.SkillType.NONE; i++)
                {
                    goalMetric += ManagerHandler.instance.SkillM.GetSkillLevel((SkillManager.SkillType)i);
                }
                // Normalise
                goalMetric /= (int)SkillManager.SkillType.NONE * SkillManager.MAX_SKILL_LEVEL;
                break;
            case GoalManager.GoalType.CONQUEROR_OF_CUISINE:
                goalMetric = ManagerHandler.instance.GoalM.GetMealsMade() / (float)100;
                break;
            case GoalManager.GoalType.FAMOUS_FIREFIGHTER:
                goalMetric = ManagerHandler.instance.GoalM.GetFiresSurvived() / (float)5;
                break;
            case GoalManager.GoalType.FOOTLOOSE_FIEND:
                goalMetric = ManagerHandler.instance.GoalM.GetHoursDancing() / (float)100;
                break;
            case GoalManager.GoalType.DOMESTIC_DELIGHT:
                // Add normalised skills
                goalMetric += ManagerHandler.instance.SkillM.GetSkillLevel(SkillManager.SkillType.COOKING) / (float)SkillManager.MAX_SKILL_LEVEL;
                goalMetric += ManagerHandler.instance.SkillM.GetSkillLevel(SkillManager.SkillType.CLEANING) / (float)SkillManager.MAX_SKILL_LEVEL;
                goalMetric += ManagerHandler.instance.SkillM.GetSkillLevel(SkillManager.SkillType.HANDINESS) / (float)SkillManager.MAX_SKILL_LEVEL;
                // Add nromalised times cleaned
                goalMetric = ManagerHandler.instance.GoalM.GetTimesCleaned() / (float)100;
                // Each element has a weighting of 1/4
                goalMetric /= 4;
                break;
        }
        return goalMetric;
    }
}
