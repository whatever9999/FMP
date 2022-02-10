// Uncomment for debug prints concerning the performance metric
// #define DEBUG_PERFORMANCE

using UnityEngine;

public class PerformanceMetric : MonoBehaviour
{
    public enum PerformanceData
    {
        AVERAGE_GAME_SPEED,
        CURRENT_GAME_SPEED,
        SPEED_CHANGES,
        ACTIONS_TRIGGERED,
        GOAL_ATTAINMENT,
        SKILL_DEVELOPMENT,
        NUM_PERFORMANCE_DATAS,
    }

    [Tooltip("Weightings should add to 1. Array index corresponds to the PerformanceData enum")]
    [SerializeField] private float[] performanceWeightings;

    [Tooltip("If speed changes are at this we think performance is maxed out")]
    [SerializeField] private float maxSpeedChangesPerMinute;
    [Tooltip("If actions triggered are at this we think performance is maxed out")]
    [SerializeField] private float maxActionsTriggeredPerMinute;

    private float averageGameSpeed = 1;
    private float totalGameSpeed = 0;

    private int speedChanges;
    private int actionsTriggered;

    private int gameTicks = 0;

    private const int MINUTE_SECONDS = 60;

    private void Update()
    {
        gameTicks++;

        // Calculate the average game speed
        totalGameSpeed += Time.timeScale;
        averageGameSpeed = totalGameSpeed / gameTicks;

        GetPerformanceMetric();
    }

    public void IncrementPerformanceAttribute(PerformanceData type)
    {
        switch (type)
        {
            case PerformanceData.SPEED_CHANGES:
                speedChanges++;
                break;
            case PerformanceData.ACTIONS_TRIGGERED:
                actionsTriggered++;
                break;
            default:
                Debug.LogError("Trying to reset timer for non-timed menace data: " + type);
                break;
        }
    }

    // The higher the performance metric, the better the player is doing at the game
    public float GetPerformanceMetric()
    {
        float performanceMetric = 0.0f;

        // AVERAGE GAME SPEED
        float averageGameSpeedValue = (averageGameSpeed / (float)(TimeManager.TimeSpeed.NUM_TIME_SPEEDS - 1));
#if DEBUG_PERFORMANCE
        Debug.Log("Average Game Speed Normalised Value: " + averageGameSpeedValue);
#endif //DEBUG_MENACE
        performanceMetric += averageGameSpeedValue * performanceWeightings[(int)PerformanceData.AVERAGE_GAME_SPEED];

        // CURRENT GAME SPEED
        float currentGameSpeedValue = (float)ManagerHandler.instance.TimeM.GetSpeed() / (float)(TimeManager.TimeSpeed.NUM_TIME_SPEEDS - 1);
#if DEBUG_PERFORMANCE
        Debug.Log("Current Game Speed Normalised Value: " + currentGameSpeedValue);
#endif //DEBUG_MENACE
        performanceMetric += currentGameSpeedValue * performanceWeightings[(int)PerformanceData.CURRENT_GAME_SPEED];

        // SPEED CHANGES
        // E.g. 20 / (10 * (180 / 60)) = 2/3
        float speedChangesValue = speedChanges / (maxSpeedChangesPerMinute * Time.realtimeSinceStartup / MINUTE_SECONDS);
        // Clamp
        if (speedChangesValue > 1) speedChangesValue = 1;
#if DEBUG_PERFORMANCE
        Debug.Log("Speed Changes Normalised Value: " + speedChangesValue);
#endif //DEBUG_MENACE
        performanceMetric += speedChangesValue * performanceWeightings[(int)PerformanceData.SPEED_CHANGES];

        // ACTIONS TRIGGERED
        // Similar to speed changes
        float actionsTriggeredValue = actionsTriggered / (maxActionsTriggeredPerMinute * Time.realtimeSinceStartup / MINUTE_SECONDS);
        // Clamp
        if (actionsTriggeredValue > 1) actionsTriggeredValue = 1;
#if DEBUG_PERFORMANCE
        Debug.Log("Actions Triggered Normalised Value: " + actionsTriggeredValue);
#endif //DEBUG_MENACE
        performanceMetric += actionsTriggeredValue * performanceWeightings[(int)PerformanceData.ACTIONS_TRIGGERED];

        // GOAL METRIC
        float goalAttainmentValue = ManagerHandler.instance.GoalM.GetGoalMetric();
#if DEBUG_PERFORMANCE
        Debug.Log("Goal Attainment Normalised Value: " + goalAttainmentValue);
#endif //DEBUG_MENACE
        performanceMetric += goalAttainmentValue * performanceWeightings[(int)PerformanceData.GOAL_ATTAINMENT];

        // SKILL DEVELOPMENT
        float skillDevelopmentValue = 0.0f;
        // Add all skill levels
        for (int i = 0; i < (int)SkillManager.SkillType.NONE; i++)
        {
            skillDevelopmentValue += ManagerHandler.instance.SkillM.GetSkillLevel((SkillManager.SkillType)i);
        }
        // Normalise
        skillDevelopmentValue /= (int)SkillManager.SkillType.NONE * SkillManager.MAX_SKILL_LEVEL;
#if DEBUG_PERFORMANCE
        Debug.Log("Skill Development Normalised Value: " + skillDevelopmentValue);
#endif //DEBUG_MENACE
        performanceMetric += skillDevelopmentValue * performanceWeightings[(int)PerformanceData.SKILL_DEVELOPMENT];

#if DEBUG_PERFORMANCE
        Debug.Log("Performance Metric: " + performanceMetric);
#endif //DEBUG_MENACE
        return performanceMetric;
    }
}
