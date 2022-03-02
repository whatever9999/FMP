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
}