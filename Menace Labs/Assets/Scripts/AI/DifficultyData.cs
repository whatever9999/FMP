using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Director/DifficultyData")]
public class DifficultyData : ScriptableObject
{
    public enum DifficultyType
    {
        RELAX,
        BUILD_UP
    }

    public DifficultyType difficultyType;
    [Tooltip("The lower and upper bounds of the target menace metric for this difficulty level (used with performance metric)")]
    public Vector2 menaceBounds;
    [Tooltip("The lower and upper bounds of the time that will be spent at this difficulty before moving to the next level (used with performance metric)")]
    public Vector2 timeBounds;
    [Tooltip("The base goal insistencies for each GoalType along with the values they increase at for this difficulty")]
    public DirectorGoal[] goalInsistencies;
}

[System.Serializable]
public struct DirectorGoal
{
    public enum GoalType
    {
        LOWER_HEALTH_METRIC,
        LOWER_SKILLS,
        INCREASE_MADNESS_CHANCE,
        NUM_GOAL_TYPES,
    }

    public GoalType type;
    public float baseValue;
    public float increaseValue;
}