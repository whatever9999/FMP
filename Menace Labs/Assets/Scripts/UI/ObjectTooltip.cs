using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectTooltip : MonoBehaviour
{
    [SerializeField] private Color positiveEffectColor = Color.green;
    [SerializeField] private Color negativeEffectColor = Color.red;

    private ConstantObject onObject;
    private Transform objectTooltipPos;

    private Dictionary<NeedsManager.NeedType, Image> needSymbols = new Dictionary<NeedsManager.NeedType, Image>();
    private Dictionary<SkillManager.SkillType, Image> skillSymbols = new Dictionary<SkillManager.SkillType, Image>();

    void Start()
    {
        needSymbols.Add(NeedsManager.NeedType.HUNGER, transform.Find("HungerSymbol").GetComponent<Image>());
        needSymbols.Add(NeedsManager.NeedType.COMFORT, transform.Find("ComfortSymbol").GetComponent<Image>());
        needSymbols.Add(NeedsManager.NeedType.BLADDER, transform.Find("BladderSymbol").GetComponent<Image>());
        needSymbols.Add(NeedsManager.NeedType.SLEEP, transform.Find("SleepSymbol").GetComponent<Image>());
        needSymbols.Add(NeedsManager.NeedType.FUN, transform.Find("FunSymbol").GetComponent<Image>());
        needSymbols.Add(NeedsManager.NeedType.SOCIAL, transform.Find("SocialSymbol").GetComponent<Image>());
        needSymbols.Add(NeedsManager.NeedType.HYGIENE, transform.Find("ShowerSymbol").GetComponent<Image>());
        needSymbols.Add(NeedsManager.NeedType.ENVIRONMENT, transform.Find("EnvironmentSymbol").GetComponent<Image>());

        skillSymbols.Add(SkillManager.SkillType.CLEANING, transform.Find("CleaningSymbol").GetComponent<Image>());
        skillSymbols.Add(SkillManager.SkillType.COOKING, transform.Find("CookingSymbol").GetComponent<Image>());
        skillSymbols.Add(SkillManager.SkillType.DANCING, transform.Find("DancingSymbol").GetComponent<Image>());
        skillSymbols.Add(SkillManager.SkillType.GAMING, transform.Find("GamingSymbol").GetComponent<Image>());
        skillSymbols.Add(SkillManager.SkillType.HANDINESS, transform.Find("HandinessSymbol").GetComponent<Image>());
        skillSymbols.Add(SkillManager.SkillType.PHOTOGRAPHY, transform.Find("PhotographySymbol").GetComponent<Image>());
        skillSymbols.Add(SkillManager.SkillType.PROGRAMMING, transform.Find("ProgrammingSymbol").GetComponent<Image>());
        skillSymbols.Add(SkillManager.SkillType.DARTS, transform.Find("DartsSymbol").GetComponent<Image>());

        SetupTooltip(null);
    }

    void Update()
    {
        // Track the object
        if (onObject && objectTooltipPos) transform.position = Camera.main.WorldToScreenPoint(objectTooltipPos.position);
    }

    public void SetupTooltip(ConstantObject objectData)
    {
        // Only show tooltips if they're not enabled
        bool tooltipsEnabled = SaveManager.instance.GetSave().enableObjectTooltips;
        if (tooltipsEnabled)
        {
            onObject = objectData;
            if (onObject) objectTooltipPos = onObject.transform.Find("TooltipLocation");

            if (objectData)
            {
                // Enable the symbols concerning the needs/skills this object affects
                List<ObjectEffect> effects = objectData.GetObjectEffects();
                for (int i = 0; i < effects.Count; i++)
                {
                    bool positiveEffect = effects[i].GetValue() > 0;

                    NeedsManager.NeedType needType = effects[i].GetNeedType();
                    if (needType != NeedsManager.NeedType.NONE)
                    {
                        needSymbols[needType].color = positiveEffect ? positiveEffectColor : negativeEffectColor;
                        needSymbols[needType].gameObject.SetActive(true);
                    }

                    SkillManager.SkillType skillType = effects[i].GetSkillType();
                    if (effects[i].GetSkillType() != SkillManager.SkillType.NONE)
                    {
                        skillSymbols[skillType].color = positiveEffect ? positiveEffectColor : negativeEffectColor;
                        skillSymbols[effects[i].GetSkillType()].gameObject.SetActive(true);
                    }
                }

                gameObject.SetActive(true);
            }
            else
            {
                DisableTooltip();
            }
        }
        // If tooltips are disabled make sure the tooltip is disabled if active
        else if (!tooltipsEnabled && gameObject.activeSelf)
        {
            DisableTooltip();
        }
    }

    private void DisableTooltip()
    {
        // Disable all symbols as we have no object
        for (SkillManager.SkillType i = 0; (int)i < skillSymbols.Count; i++)
        {
            skillSymbols[i].gameObject.SetActive(false);
        }
        for (NeedsManager.NeedType i = 0; (int)i < needSymbols.Count; i++)
        {
            needSymbols[i].gameObject.SetActive(false);
        }

        gameObject.SetActive(false);
    }
}
