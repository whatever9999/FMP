using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

        UpdateUI();
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
    public void SetNeed(float amount)
    {
        // Track what the original value was so we only trigger a NeedTrigger once
        previousNeedValue = currentNeedValue;

        currentNeedValue = amount;

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

    public IEnumerator Flash(Color color)
    {
        Color originalColor = needFill.color;
        for (int i = 0; i < 5; i++)
        {
            needFill.color = color;
            yield return new WaitForSecondsRealtime(0.3f);
            needFill.color = originalColor;
            yield return new WaitForSecondsRealtime(0.3f);
        }
    }

    // If need goes below a need trigger value then trigger the event
    private void CheckNeedTriggers()
    {
        for (int i = 0; i < needTriggers.Length; i++)
        {
            // If we weren't above the trigger value before but are now then call the trigger
            if (previousNeedValue > (float)needTriggers[i].needLevel && currentNeedValue <= (float)needTriggers[i].needLevel)
            {
                switch (needTriggers[i].eventType)
                {
                    case EventManager.EventType.DEATH:
                        // Set the death type before adding the death action (which will check the event trigger)
                        // If we're on fire then this is a fire death, otherwise it's a need related death e.g. starvation
                        if (ManagerHandler.instance.clone.IsOnFire()) ManagerHandler.instance.EventM.SetDeathEventType(DeathData.DeathTypes.FIRE);
                        else ManagerHandler.instance.EventM.SetDeathEventType(DeathData.DeathTypes.STARVATION);

                        ManagerHandler.instance.ActionM.AddAction(ActionManager.ActionType.DIE, -1);
                        break;
                    case EventManager.EventType.PASS_OUT:
                        // Only check as long as we're not already passed out
                        if (!ManagerHandler.instance.ActionM.IsCurrently(ActionManager.ActionType.PASS_OUT))
                        {
                            ManagerHandler.instance.EventM.CheckEventTrigger(needTriggers[i].eventType);
                        }
                        break;
                    default:
                        ManagerHandler.instance.EventM.CheckEventTrigger(needTriggers[i].eventType);
                        break;
                }
            }
        }
    }
}
