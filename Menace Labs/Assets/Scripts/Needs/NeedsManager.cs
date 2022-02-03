using UnityEngine;

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
    }

    [SerializeField] private Need[] needs;
    [SerializeField] private int updateMultiplier = 1;
    [SerializeField] private int fireMultiplier = 4;
    [SerializeField] private int illMultiplier = 3;
    [SerializeField] private float timeToUpdateNeed = 1.0f;
    [Tooltip("If the health metric is less than this value when the clone is electrocuted they have a chance of dying")]
    [SerializeField] private float electrocutionDeathCheck = 0.4f;
    [Tooltip("If fun or social are below this value the clone has a chance of going mad each update")]
    [SerializeField] private int madnessDeathCheck = 5;
    private float updateNeedTimer;

    private bool isIll = false;
    private bool onFire = false;
    public void SetIll(bool setTo) { isIll = setTo; }
    public void SetOnFire(bool setTo) { onFire = setTo; }

    private void Update()
    {
        updateNeedTimer += Time.deltaTime;

        // Update needs every timeToUpdateNeed seconds
        if (updateNeedTimer > timeToUpdateNeed)
        {
            for (int i = 0; i < needs.Length; i++)
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

            updateNeedTimer = 0;

            MadnessCheck();
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
        bool low_fun_or_social = false;
        for (int i = 0; i < needs.Length; i++)
        {
            NeedType needType = needs[i].GetNeedType();
            if (needType == NeedType.FUN || needType == NeedType.SOCIAL)
            {
                if (needs[i].GetValue() < madnessDeathCheck)
                {
                    low_fun_or_social = true;
                    break;
                }
            }
        }

        if (low_fun_or_social)
        {
            ManagerHandler.instance.EventM.CheckEventTrigger(EventManager.EventType.DEATH);
        }
    }

    // If health metric is low electrocution will kill
    public void Electrocute()
    {
        if (GetHealthMetric() < electrocutionDeathCheck)
        {
            ManagerHandler.instance.EventM.CheckEventTrigger(EventManager.EventType.DEATH);
        }
    }
}

[System.Serializable]
public struct NeedTrigger
{
    public NeedsManager.NeedLevel needLevel;
    public EventManager.EventType eventType;
}