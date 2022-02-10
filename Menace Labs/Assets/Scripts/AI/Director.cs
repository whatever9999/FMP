using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour
{
    private enum Bounds
    {
        LOWER,
        UPPER,
    }

    // TODO: Add list of possible actions

    [SerializeField] private DifficultyData lowDifficultyData;
    [SerializeField] private DifficultyData mediumDifficultyData;
    [SerializeField] private DifficultyData highDifficultyData;

    [Tooltip("How often the director checks for an action")]
    [SerializeField] private float directorTimer = 10;
    private float currentDirectorTimer;
    private float menaceTimer;
    private float currentMenaceTimer;

    private DifficultyData currentDifficulty;

    private GoalInsistency[] currentGoalInsistencies;

    private PerformanceMetric PerformanceM;
    private MenaceMetric MenaceM;

    private void Start()
    {
        PerformanceM = FindObjectOfType<PerformanceMetric>();
        MenaceM = FindObjectOfType<MenaceMetric>();
    }

    private void Update()
    {
        // Update timers
        currentDirectorTimer += Time.deltaTime;
        menaceTimer += Time.deltaTime;

        // If it's time for the director to carry our an action they should do so
        if (currentDirectorTimer >= directorTimer)
        {
            // Once we've been at this menace level long enough we should move to the next one
            if (currentMenaceTimer >= menaceTimer)
            {
                ChangeDifficulty();
            }
            else
            {
                IncreaseInsistencies();
            }
            
            // The director uses utility AI to determine what action it wants to take
            ChooseAction();
        }
    }

    // Utility AI
    private void ChooseAction()
    {

    }
    private void CalculateDiscontentment()
    {

    }

    // Insistencies need to increase by DirectorGoal.increaseValue for each director tick
    private void IncreaseInsistencies()
    {

    }

    // When the difficulty is changed the insistencies and menace timer need to be set according to the difficulty level and performance metric
    private void ChangeDifficulty()
    {
        float performanceMetric = PerformanceM.GetPerformanceMetric();

        SetInsistencies();
        SetMenaceTimer(performanceMetric);
    }
    private void SetInsistencies()
    {

    }
    // The higher the performance, the shorter the time we'll spend at low/medium difficulty and the higher at high difficulty
    private void SetMenaceTimer(float performanceMetric)
    {
        float performanceEffect = 1 - performanceMetric;
        if (currentDifficulty.difficultyType == DifficultyData.DifficultyType.HIGH)
        {
            performanceEffect = performanceMetric;
        }

        menaceTimer = ((currentDifficulty.timeBounds[(int)Bounds.UPPER] - currentDifficulty.timeBounds[(int)Bounds.LOWER]) * performanceEffect) + currentDifficulty.timeBounds[(int)Bounds.LOWER];
    }
}
