using UnityEngine;

public abstract class ChanceEffect : ScriptableObject
{
    public enum ChanceEffectType
    {
        NONE,
        ADD_TEST_TIME,
        MAX_NEED,
        LOSE_RANDOM_SKILL,
        END_TEST,
        MODIFY_SKILL,
        MODIFY_NEED,
        ILLNESS,
        INCREASE_FOOD,
    }

    public ChanceEffectType effectType;
    public abstract void TriggerEffect();
}