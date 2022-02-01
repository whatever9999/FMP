using UnityEngine;
using UnityEngine.UI;

public class Need : MonoBehaviour
{
    [SerializeField] private NeedsManager.NeedType needType;
    [SerializeField] private float needModifier = 0.5f;
    [SerializeField] private Image needFill;
    [SerializeField] private NeedTrigger[] needTriggers;

    private Slider needSlider;

    private float previousNeedValue = 100.0f;
    private float currentNeedValue = 100.0f;

    public NeedsManager.NeedType GetNeedType() { return needType; }
    public float GetValue() { return currentNeedValue; }

    private void Start()
    {
        needSlider = GetComponent<Slider>();
    }

    public void UpdateNeed(int amount)
    {
        // Track what the original value was so we only trigger a NeedTrigger once
        previousNeedValue = currentNeedValue;

        // The environment need value is determined by the amount of puddles, rubbish and dirty objects in the clone's quarters
        if (needType == NeedsManager.NeedType.ENVIRONMENT)
        {
            int numberGrossObjects = 0;

            TimedObject[] objects = FindObjectsOfType<TimedObject>();
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].AffectsEnvironment()) numberGrossObjects++;
            }

            currentNeedValue = NeedsManager.MAX_NEED_VALUE - (numberGrossObjects * needModifier);
        }
        else
        {
            currentNeedValue -= needModifier * amount;
        }

        ClampNeed();
        CheckNeedTriggers();
        UpdateUI();
    }
    public void ModifyNeed(float amount)
    {
        // Track what the original value was so we only trigger a NeedTrigger once
        previousNeedValue = currentNeedValue;

        currentNeedValue += amount;

        ClampNeed();
        CheckNeedTriggers();
        UpdateUI();
    }

    private void ClampNeed()
    {
        if (currentNeedValue < 0) currentNeedValue = 0;
        else if (currentNeedValue > NeedsManager.MAX_NEED_VALUE) currentNeedValue = NeedsManager.MAX_NEED_VALUE;
    }

    private void UpdateUI()
    {
        needSlider.value = currentNeedValue;
        needFill.color = ManagerHandler.instance.NeedsM.GetNeedColor(currentNeedValue);
    }

    // If need goes below a need trigger value then trigger the event
    private void CheckNeedTriggers()
    {
        for (int i = 0; i < needTriggers.Length; i++)
        {
            // If we weren't above the trigger value before but are now then call the trigger
            if (previousNeedValue > (float)needTriggers[i].needLevel && currentNeedValue <= (float)needTriggers[i].needLevel)
            {
                ManagerHandler.instance.EventM.CheckEventTrigger(needTriggers[i].eventType);
            }
        }
    }
}
