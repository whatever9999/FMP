// Uncomment for debug prints concerning the menace metric
 #define DEBUG_MENACE

using System.Collections.Generic;
using UnityEngine;

public class MenaceMetric : MonoBehaviour
{
    public enum MenaceData
    {
        BROKEN_ITEMS,
        HEALTH_METRIC,
        TIME_SINCE_EVENT,
        TIME_SINCE_DISASTER,
        TIME_SINCE_CHANCE_CARD_SUCCESS,
        TIME_SINCE_CHANCE_CARD_FAILURE,
        NUM_MENACE_DATAS,
    }

    [Tooltip("Weightings should add to 1. Array index corresponds to the MenaceData enum")]
    [SerializeField] private float[] menaceWeightings;

    [Tooltip("0 seconds since an event will have max menace for that attribute. The seconds for this value indicate when menace reaches 0.")]
    [SerializeField] private float maxTimeSinceEvent;
    [Tooltip("0 seconds since a disaster will have max menace for that attribute. The seconds for this value indicate when menace reaches 0.")]
    [SerializeField] private float maxTimeSinceDisaster;
    [Tooltip("0 seconds since chance card success will have min menace for that attribute. The seconds for this value indicate when menace reaches 1.")]
    [SerializeField] private float maxTimeSinceChanceCardSuccess;
    [Tooltip("0 seconds since chance card failure will have max menace for that attribute. The seconds for this value indicate when menace reaches 0.")]
    [SerializeField] private float maxTimeSinceChanceCardFailure;

    private List<ConstantObject> breakableObjects = new List<ConstantObject>();

    private float timeSinceEvent;
    private float timeSinceDisaster;
    private float timeSinceChanceCardSuccess;
    private float timeSinceChanceCardFailure;

    private void Start()
    {
        // Collect the breakable objects in the scene
        ConstantObject[] objects = FindObjectsOfType<ConstantObject>();
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i].GetBreakType() == ConstantObject.BreakType.WORKING)
            {
                breakableObjects.Add(objects[i]);
            }
        }

        // Set the inverse timers to their max values so the AI doesn't think the player is under loads of menace when they've just started
        timeSinceEvent = maxTimeSinceEvent;
        timeSinceDisaster = maxTimeSinceDisaster;
        timeSinceChanceCardFailure = maxTimeSinceChanceCardFailure;
    }

    private void Update()
    {
        // Keep timers updated
        timeSinceEvent += Time.deltaTime;
        timeSinceDisaster += Time.deltaTime;
        timeSinceChanceCardSuccess += Time.deltaTime;
        timeSinceChanceCardFailure += Time.deltaTime;
    }

    public void ResetTimer(MenaceData type)
    {
        switch (type)
        {
            case MenaceData.TIME_SINCE_EVENT:
                timeSinceEvent = 0.0f;
                break;
            case MenaceData.TIME_SINCE_DISASTER:
                timeSinceDisaster = 0.0f;
                break;
            case MenaceData.TIME_SINCE_CHANCE_CARD_SUCCESS:
                timeSinceChanceCardSuccess = 0.0f;
                break;
            case MenaceData.TIME_SINCE_CHANCE_CARD_FAILURE:
                timeSinceChanceCardFailure = 0.0f;
                break;
            default:
                Debug.LogError("Trying to reset timer for non-timed menace data: " + type);
                break;
        }
    }

    // The higher the menace metric, the more stress the player is under
    public float GetMenaceMetric()
    {
        float menaceMetric = 0.0f;

        // BROKEN OBJECTS
        // Count the number of broken objects
        int brokenObjects = 0;
        for (int i = 0; i < breakableObjects.Count; i++)
        {
            if (breakableObjects[i].GetBreakType() == ConstantObject.BreakType.BROKEN) brokenObjects++;
        }
        // Get normalised value for broken items
        float brokenItemValue = (float)brokenObjects / (float)breakableObjects.Count;
#if DEBUG_MENACE
        Debug.Log("Broken Item Normalised Value: " + brokenItemValue);
#endif //DEBUG_MENACE
        menaceMetric += brokenItemValue * menaceWeightings[(int)MenaceData.BROKEN_ITEMS];

        // HEALTH METRIC
        // A health metric of 1 means good health so make sure to consider the inverse
        float healthValue = 1 - ManagerHandler.instance.NeedsM.GetHealthMetric();
#if DEBUG_MENACE
        Debug.Log("Health Metric Normalised Value: " + healthValue);
#endif //DEBUG_MENACE
        menaceMetric += healthValue * menaceWeightings[(int)MenaceData.HEALTH_METRIC];

        // TIME SINCE EVENT
        float timeSinceEventValue = timeSinceEvent / maxTimeSinceEvent;
        // Clamp as time can be over max
        if (timeSinceEventValue > 1) timeSinceEventValue = 1;
#if DEBUG_MENACE
        Debug.Log("Time Since Event Normalised Value: " + (1 - timeSinceEventValue));
#endif //DEBUG_MENACE
        // Need the inverse as max menace is at 0 time
        menaceMetric += (1 - timeSinceEventValue) * menaceWeightings[(int)MenaceData.TIME_SINCE_EVENT];

        // TIME SINCE DISASTER
        float timeSinceDisasterValue = timeSinceDisaster / maxTimeSinceDisaster;
        // Clamp as time can be over max
        if (timeSinceDisasterValue > 1) timeSinceDisasterValue = 1;
#if DEBUG_MENACE
        Debug.Log("Time Since Disaster Normalised Value: " + (1 - timeSinceDisasterValue));
#endif //DEBUG_MENACE
        // Need the inverse as max menace is at 0 time
        menaceMetric += (1 - timeSinceDisasterValue) * menaceWeightings[(int)MenaceData.TIME_SINCE_DISASTER];

        // TIME SINCE CHANCE CARD SUCCESS
        float timeSinceChanceCardSuccessValue = timeSinceChanceCardSuccess / maxTimeSinceChanceCardSuccess;
        // Clamp as time can be over max
        if (timeSinceChanceCardSuccessValue > 1) timeSinceChanceCardSuccessValue = 1;
#if DEBUG_MENACE
        Debug.Log("Time Since Chance Card Success Normalised Value: " + timeSinceChanceCardSuccessValue);
#endif //DEBUG_MENACE
        menaceMetric += timeSinceChanceCardSuccessValue * menaceWeightings[(int)MenaceData.TIME_SINCE_CHANCE_CARD_SUCCESS];

        // TIME SINCE CHANCE CARD FAILURE
        float timeSinceChanceCardFailureValue = timeSinceChanceCardFailure / maxTimeSinceChanceCardFailure;
        // Clamp as time can be over max
        if (timeSinceChanceCardFailureValue > 1) timeSinceChanceCardFailureValue = 1;
#if DEBUG_MENACE
        Debug.Log("Time Since Chance Card Failure Normalised Value: " + (1 - timeSinceChanceCardFailureValue));
#endif //DEBUG_MENACE
        // Need the inverse as max menace is at 0 time
        menaceMetric += (1 - timeSinceChanceCardFailureValue) * menaceWeightings[(int)MenaceData.TIME_SINCE_CHANCE_CARD_FAILURE];

#if DEBUG_MENACE
        Debug.Log("Menace Metric: " + menaceMetric);
#endif //DEBUG_MENACE
        return menaceMetric;
    }
}