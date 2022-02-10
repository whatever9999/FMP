using UnityEngine;

#region Abstract Classes
public abstract class Event
{
    protected abstract void TriggerEvent();
    public abstract bool CheckTrigger();
}
// Check chance to trigger
public abstract class ChanceEvent : Event
{
    public override bool CheckTrigger()
    {
        float rand = Random.Range(0.0f, 100.0f);
        // TODO: Multiply chanceDecrease by skill level
        if (rand < chance - (chanceDecrease))
        {
            TriggerEvent();
            return true;
        }
        return false;
    }

    public void SetChance(float newChance) { chance = newChance; }

    [Tooltip("Multiplied by specified skill to decrease chance of event")]
    [SerializeField] private float chanceDecrease;
    // [SerializeField] private SkillType skill;
    [SerializeField] protected float chance;
}
// Trigger after a number of times checked
public abstract class OccurrenceEvent : Event
{
    public override bool CheckTrigger()
    {
        // If this trigger has been hit an occurence number of times then trigger the event
        if (++timesTriggered == occurence)
        {
            timesTriggered = 0;
            TriggerEvent();
            return true;
        }
        return false;
    }

    public void SetOccurrence(int newOccurrence) { occurence = newOccurrence; }

    [SerializeField] private int occurence;
    private int timesTriggered = 0;
}
// Always trigger
public abstract class HundredPercentEvent : Event
{
    public override bool CheckTrigger()
    {
        TriggerEvent();
        return true;
    }
}
#endregion // Abstract Classes

[System.Serializable]
public class FireEvent : ChanceEvent
{
    protected override void TriggerEvent()
    {
        ManagerHandler.instance.clone.SpawnObject(firePrefab);
    }

    [SerializeField] private GameObject firePrefab;
}

[System.Serializable]
public class ElectrocutionEvent : ChanceEvent
{
    protected override void TriggerEvent()
    {
        ManagerHandler.instance.clone.Electrocute();
    }
}

[System.Serializable]
public class RubbishEvent : HundredPercentEvent
{
    protected override void TriggerEvent()
    {
        ManagerHandler.instance.clone.SpawnObject(rubbishPrefab);
    }

    [SerializeField] private GameObject rubbishPrefab;
}

[System.Serializable]
public class PuddleEvent : HundredPercentEvent
{
    protected override void TriggerEvent()
    {
        ManagerHandler.instance.clone.SpawnObject(puddlePrefab);
    }

    [SerializeField] private GameObject puddlePrefab;
}
[System.Serializable]
public class BladderFailureEvent : HundredPercentEvent
{
    protected override void TriggerEvent()
    {
        ManagerHandler.instance.clone.SpawnObject(puddlePrefab);
        ManagerHandler.instance.NeedsM.SetNeed(NeedsManager.NeedType.BLADDER, NeedsManager.MAX_NEED_VALUE);
    }

    [SerializeField] private GameObject puddlePrefab;
}

[System.Serializable]
public class BreakingEvent : OccurrenceEvent
{
    protected override void TriggerEvent()
    {
        if (breakableObject)
        {
            ManagerHandler.instance.SoundM.PlayClip(SoundManager.SoundName.BROKEN_ITEM);
            breakableObject.SetToBreakableAlternate();
        }
        else
        {
            Debug.LogError("Don't have an object to break!");
        }

        // Don't need object once done
        breakableObject = null;
    }

    private ConstantObject breakableObject;
    public void SetBreakableObject(ConstantObject newBreakable) { breakableObject = newBreakable; }
}

[System.Serializable]
public class DirtyingEvent : OccurrenceEvent
{
    protected override void TriggerEvent()
    {
        if (dirtiableObject)
        {
            dirtiableObject.SetToDirtAlternate();
        }
        else
        {
            Debug.LogError("Don't have an object to dirty!");
        }

        // Don't need object once done
        dirtiableObject = null;
    }

    private ConstantObject dirtiableObject;
    public void SetDirtiableObject(ConstantObject newDirtiable) { dirtiableObject = newDirtiable; }
}

[System.Serializable]
public class DeathEvent : HundredPercentEvent
{
    protected override void TriggerEvent()
    {
        ManagerHandler.instance.SoundM.PlayClip(SoundManager.SoundName.DEATH);
        ManagerHandler.instance.AnimationM.SetAnimation(AnimationManager.AnimationType.DIE, true);
        ManagerHandler.instance.PopupM.ShowDeath();
    }
}

[System.Serializable]
public class PassOutEvent : HundredPercentEvent
{
    protected override void TriggerEvent()
    {
        // TODO: Add "Pass Out" Action
    }
}


[System.Serializable]
public class FoodDeliveryEvent : HundredPercentEvent
{
    protected override void TriggerEvent()
    {
        ManagerHandler.instance.FoodM.ModifyFoodAmount(foodAmount);
    }

    [SerializeField] private int foodAmount;
}

[System.Serializable]
public class DayStartEvent : HundredPercentEvent
{
    protected override void TriggerEvent()
    {
        // Age the clone
        ManagerHandler.instance.AgeM.ModifyCloneAge(1);
        // Set ambience
        ManagerHandler.instance.SoundM.ChangeAmbienceTrack(SoundManager.AmbienceName.DAY);
    }
}
[System.Serializable]
public class NightStartEvent : HundredPercentEvent
{
    protected override void TriggerEvent()
    {
        // Set ambience
        ManagerHandler.instance.SoundM.ChangeAmbienceTrack(SoundManager.AmbienceName.NIGHT);
    }
}