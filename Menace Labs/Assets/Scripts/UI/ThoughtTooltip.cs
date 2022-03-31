using System.Collections.Generic;
using UnityEngine;

public class ThoughtTooltip : MonoBehaviour
{
    private Dictionary<NeedsManager.NeedType, Transform> needSymbols = new Dictionary<NeedsManager.NeedType, Transform>();

    [SerializeField] private Transform cloneThoughtPos;

    [SerializeField] private float thoughtDuration = 3.0f;
    private float thoughtTimer;

    void Start()
    {
        needSymbols.Add(NeedsManager.NeedType.HUNGER, transform.Find("HungerSymbol"));
        needSymbols.Add(NeedsManager.NeedType.COMFORT, transform.Find("ComfortSymbol"));
        needSymbols.Add(NeedsManager.NeedType.BLADDER, transform.Find("BladderSymbol"));
        needSymbols.Add(NeedsManager.NeedType.SLEEP, transform.Find("SleepSymbol"));
        needSymbols.Add(NeedsManager.NeedType.FUN, transform.Find("FunSymbol"));
        needSymbols.Add(NeedsManager.NeedType.SOCIAL, transform.Find("SocialSymbol"));
        needSymbols.Add(NeedsManager.NeedType.HYGIENE, transform.Find("ShowerSymbol"));
        needSymbols.Add(NeedsManager.NeedType.ENVIRONMENT, transform.Find("EnvironmentSymbol"));

        gameObject.SetActive(false);
    }

    void Update()
    {
        thoughtTimer += Time.unscaledDeltaTime;

        if (thoughtTimer > thoughtDuration)
        {
            thoughtTimer = 0;
            gameObject.SetActive(false);
        }

        transform.position = Camera.main.WorldToScreenPoint(cloneThoughtPos.position);
    }

    public void SetupTooltip(NeedsManager.NeedType need)
    {
        for (NeedsManager.NeedType i = 0; (int)i < needSymbols.Count; i++)
        {
            needSymbols[i].gameObject.SetActive(false);
            
        }
        needSymbols[need].gameObject.SetActive(true);

        gameObject.SetActive(true);
    }
}
