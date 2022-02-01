using UnityEngine;

public class NeedsManager : MonoBehaviour
{
    public static NeedsManager instance;

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
    }
    public enum NeedLevel
    {
        EXTREMELY_LOW = 0,
        VERY_LOW = 20,
        LOW = 40,
    }

    [SerializeField] private Need[] needs;
    [SerializeField] private float timeToUpdateNeed = 1.0f;
    private float updateNeedTimer;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        updateNeedTimer += Time.deltaTime;

        // Update needs every timeToUpdateNeed seconds
        if (updateNeedTimer > timeToUpdateNeed)
        {
            for (int i = 0; i < needs.Length; i++)
            {
                needs[i].UpdateNeed(1);
            }

            updateNeedTimer = 0;
        }
    }

    public void ModifyNeed(NeedType needType, int amount)
    {
        for (int i = 0; i < needs.Length; i++)
        {
            if (needs[i].GetNeedType() == needType)
            {
                needs[i].ModifyNeed(amount);
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
}

[System.Serializable]
public struct NeedTrigger
{
    public NeedsManager.NeedLevel needLevel;
    public EventManager.EventType eventType;
}