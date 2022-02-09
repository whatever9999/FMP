using UnityEngine;

public abstract class DisasterEffect : ScriptableObject
{
    public enum DisasterEffectType
    {
        NONE,
        BREAK_ALL_OBJECTS,
        BREAK_OBJECT,
        DISASTER_ILLNESS,
        FIRE,
        PUDDLE,
        FOOD_ISSUE,
    }

    public DisasterEffectType effectType;
    public abstract void TriggerEffect();
}