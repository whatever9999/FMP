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
    public bool react = true;

    // TriggerEffect implementations should call base to ensure clone reacts
    public virtual void TriggerEffect()
    {
        if (react) ManagerHandler.instance.ActionM.AddAction(ActionManager.ActionType.REACT, 1);
    }
}