using UnityEngine;
using System.Collections.Generic;

public class NeedsManager : MonoBehaviour
{
    public const int MAX_NEED_VALUE = 100;

    public enum NeedType
    {
        HUNGER,
        COMFORT,
        BLADDER,
        SLEEP,
        FUN,
        SOCIAL,
        HYGIENE,
        ENVIRONMENT,
        NONE,
    }
    public enum NeedLevel
    {
        EXTREMELY_LOW = 0,
        VERY_LOW = 20,
        LOW = 40,
        HIGH = 90,
        MAX = 100,
    }

    [SerializeField] private Need[] needs;
    [SerializeField] private int updateMultiplier = 1;
    [SerializeField] private int fireMultiplier = 3;
    [SerializeField] private int illMultiplier = 2;
    [SerializeField] private float timeToUpdateNeed = 1.0f;
    [Tooltip("If the health metric is less than this value when the clone is electrocuted they have a chance of dying")]
    [SerializeField] private float electrocutionDeathCheck = 0.4f;
    [Tooltip("If fun or social are below this value the clone has a chance of going mad each update")]
    [SerializeField] private int madnessDeathCheck = 5;
    private float updateNeedTimer;

    private bool isIll = false;
    private bool onFire = false;
    public void SetIll(bool setTo) 
    { 
        isIll = setTo;
        if (isIll) ManagerHandler.instance.NotificationM.AddNotification(NotificationManager.NotificationType.ILLNESS);
    }
    public bool IsIll() { return isIll; }
    public void SetOnFire(bool setTo) { onFire = setTo; }
    public bool IsOnFire() { return onFire; }

    private void Update()
    {
        updateNeedTimer += Time.deltaTime;

        // If we're currently using an object don't touch needs that it affects
        List<NeedType> needsAffectedByObject = new List<NeedType>();
        if (ManagerHandler.instance.ActionM.GetCurrentAction() && ManagerHandler.instance.ActionM.GetCurrentAction().GetActionType() == ActionManager.ActionType.OBJECT_USE)
        {
            // Extract the needs affected by this object
            List<ObjectEffect> effects = (ManagerHandler.instance.ActionM.GetCurrentAction() as ObjectUseAction).GetUsedObject().GetObjectEffects();
            for (int i = 0; i < effects.Count; i++)
            {
                needsAffectedByObject.Add(effects[i].GetNeedType());
            }
        }

        // Update needs every timeToUpdateNeed seconds
        if (updateNeedTimer > timeToUpdateNeed)
        {
            for (int i = 0; i < needs.Length; i++)
            {
                // Only update the need if the current object in use doesn't affect it
                if (!needsAffectedByObject.Contains(needs[i].GetNeedType()))
                {
                    // If on fire all needs are decreased faster
                    if (onFire) needs[i].UpdateNeed(fireMultiplier);
                    // If ill sleep, bladder and hygiene needs decrease faster
                    else if (isIll && (needs[i].GetNeedType() == NeedType.SLEEP || needs[i].GetNeedType() == NeedType.BLADDER || needs[i].GetNeedType() == NeedType.HYGIENE))
                    {
                        needs[i].UpdateNeed(illMultiplier);
                    }
                    // Otherwise, needs decrease at a normal rate
                    else needs[i].UpdateNeed(updateMultiplier);
                }
            }

            updateNeedTimer = 0;

            MadnessCheck();

            if (isIll) IllnessCheck();

            SmellyCheck();
        }
    }

    public void ModifyNeed(NeedType needType, float amount)
    {
        for (int i = 0; i < needs.Length; i++)
        {
            if (needs[i].GetNeedType() == needType)
            {
                needs[i].ModifyNeed(amount);
            }
        }
    }
    public void SetNeed(NeedType needType, float amount)
    {
        for (int i = 0; i < needs.Length; i++)
        {
            if (needs[i].GetNeedType() == needType)
            {
                needs[i].SetNeed(amount);
            }
        }
    }

    public float GetNeedValue(NeedType needType)
    {
        for (int i = 0; i < needs.Length; i++)
        {
            if (needs[i].GetNeedType() == needType)
            {
                return needs[i].GetValue();
            }
        }

        Debug.LogError("Couldn't find need of type " + needType);
        return 0;
    }

    public Color GetNeedColor(float value)
    {
        Color needColor = Color.black;

        // Add normalised colors to create a gradient according to the need value
        needColor += Color.red * ((MAX_NEED_VALUE - value) / MAX_NEED_VALUE);
        needColor += Color.green * (value / MAX_NEED_VALUE);

        return needColor;
    }

    // Normalised average value of needs
    public float GetHealthMetric()
    {
        float health = 0;
        for (int i = 0; i < needs.Length; i++)
        {
            health += needs[i].GetValue();
        }
        health /= (needs.Length * MAX_NEED_VALUE);

        return health;
    }

    // If the clone has low fun or social check for a madness death on update
    private void MadnessCheck()
    {
        bool low_fun_social_and_environment = true;
        for (int i = 0; i < needs.Length; i++)
        {
            NeedType needType = needs[i].GetNeedType();
            if (needType == NeedType.FUN || needType == NeedType.SOCIAL || needType == NeedType.ENVIRONMENT)
            {
                if (needs[i].GetValue() > madnessDeathCheck)
                {
                    low_fun_social_and_environment = false;
                    break;
                }
            }
        }

        if (low_fun_social_and_environment)
        {
            // Chance increases the worse the values are
            float total = GetNeedValue(NeedType.FUN) + GetNeedValue(NeedType.SOCIAL) + GetNeedValue(NeedType.ENVIRONMENT);
            float rand = Random.Range(0.0f, 300.0f);

            if (rand >= total)
            {
                // If the clone is on fire then they'll die a fire death
                if (onFire)
                {
                    ManagerHandler.instance.EventM.SetDeathEventType(DeathData.DeathTypes.FIRE);
                }
                else
                {
                    ManagerHandler.instance.EventM.SetDeathEventType(DeathData.DeathTypes.MADNESS);
                }
                ManagerHandler.instance.ActionM.AddAction(ActionManager.ActionType.DIE, -1);
            }
        }
    }

    // If health metric is low electrocution will kill
    public void Electrocute()
    {
        if (GetHealthMetric() < electrocutionDeathCheck)
        {
            ManagerHandler.instance.EventM.SetDeathEventType(DeathData.DeathTypes.ELECTROCUTION);
            ManagerHandler.instance.ActionM.AddAction(ActionManager.ActionType.DIE, -1);
        }
    }

    // If the clone is ill and their sleep, comfort and hygiene are high, they will become better
    private void IllnessCheck()
    {
        if (GetNeedValue(NeedType.SLEEP) >= (int)NeedLevel.HIGH     && 
            GetNeedValue(NeedType.HYGIENE) >= (int)NeedLevel.HIGH   && 
            GetNeedValue(NeedType.COMFORT) >= (int)NeedLevel.HIGH   )
        {
            SetIll(false);
        }
    }

    // If the clone's hygiene is low then it will show with pfx
    private void SmellyCheck()
    {
        bool is_smelly = ManagerHandler.instance.clone.IsSmelly();
        if (!is_smelly && GetNeedValue(NeedType.HYGIENE) <= (int)NeedLevel.VERY_LOW)
        {
            ManagerHandler.instance.clone.SetSmelly(true);
        }
        else if (is_smelly)
        {
            ManagerHandler.instance.clone.SetSmelly(false);
        }
    }
}

[System.Serializable]
public struct NeedTrigger
{
    public NeedsManager.NeedLevel needLevel;
    public EventManager.EventType eventType;
}