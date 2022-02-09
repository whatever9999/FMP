using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/ChanceCardData")]
public class ChanceCardData : ScriptableObject
{
    public enum OptionChoice
    {
        OPTION_A,
        OPTION_B,
        NONE,
    }

    [Header("Card Data")]
    public Sprite image;
    public string title;
    [TextArea(15, 20)]
    public string description;

    [Header("Option A")]
    public string A_ButtonText;
    public float A_Chance;
    public SuccessModifier A_SuccessModifier;

    [TextArea(15, 20)]
    public string A_SuccessDescription;
    public ChanceEffect[] A_SuccessEffects;

    [TextArea(15, 20)]
    public string A_FailureDescription;
    public ChanceEffect[] A_FailureEffects;

    [Header("Option B")]
    public string B_ButtonText;
    public float B_Chance;
    public SuccessModifier B_SuccessModifier;

    [TextArea(15, 20)]
    public string B_SuccessDescription;
    public ChanceEffect[] B_SuccessEffects;

    [TextArea(15, 20)]
    public string B_FailureDescription;
    public ChanceEffect[] B_FailureEffects;

    public bool MakeChoice(OptionChoice choice)
    {
        // Generate random number
        float rand = Random.Range(0.0f, 100.0f);

        // Get chance according to choice made and if there are any modifiers
        float chance = 0;
        switch (choice)
        {
            case OptionChoice.OPTION_A:
                chance = A_Chance + A_SuccessModifier.CheckModifier();
                break;
            case OptionChoice.OPTION_B:
                chance = B_Chance + B_SuccessModifier.CheckModifier();
                break;
        }

        // Return if the choice succeeded or failed
        if (rand < chance) return true;
        return false;
    }

    
}

[System.Serializable]
public struct SuccessModifier
{
    public SkillManager.SkillType skillType;
    public NeedsManager.NeedType needType;
    [Tooltip("If the skill level or need value is lower or higher than this value then the modifier will take effect")]
    public float checkValue;
    [Tooltip("Will the modifier be applied if the skill/need is lower or higher than the checkValue?")]
    public bool checkIfLower;
    [Tooltip("How much the chance will be affected if the modifier check is true")]
    public float modifyValue;

    public float CheckModifier()
    {
        // Get the value of the skill/need being checked
        float value = 0;
        if (skillType != SkillManager.SkillType.NONE)
        {
            value = ManagerHandler.instance.SkillM.GetSkillLevel(skillType);
        }
        else if (needType != NeedsManager.NeedType.NONE)
        {
            value = ManagerHandler.instance.NeedsM.GetNeedValue(needType);
        }

        // Check if the value is lower/higher than the one we want
        bool applyModifier = checkIfLower ? value < checkValue : value > checkValue;
        if (applyModifier)
        {
            return modifyValue;
        }
        else
        {
            return 0;
        }
    }
}