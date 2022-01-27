using UnityEngine;

#region Abstract Classes
public abstract class Event
{
    protected abstract void TriggerEvent();
    public abstract void CheckTrigger();
}
// Check chance to trigger
public abstract class ChanceEvent : Event
{
    public override void CheckTrigger()
    {
        float rand = Random.Range(0.0f, 100.0f);
        // TODO: Multiply chanceDecrease by skill level
        if (rand < chance - (chanceDecrease))
        {
            TriggerEvent();
        }
    }

    [Tooltip("Multiplied by specified skill to decrease chance of event")]
    [SerializeField] private float chanceDecrease;
    // [SerializeField] private SkillType skill;
    [SerializeField] protected float chance;
}
// Trigger after a number of times checked
public abstract class OccurenceEvent : Event
{
    public override void CheckTrigger()
    {
        // If this trigger has been hit an occurence number of times then trigger the event
        if (++timesTriggered == occurence)
        {
            timesTriggered = 0;
            TriggerEvent();
        }
    }

    [SerializeField] private int occurence;
    private int timesTriggered = 0;
}
// Always trigger
public abstract class HundredPercentEvent : Event
{
    public override void CheckTrigger()
    {
        TriggerEvent();
    }
}
#endregion // Abstract Classes

[System.Serializable]
public class FireEvent : ChanceEvent
{
    // TODO: Instantiate a fire on top of the clone
    protected override void TriggerEvent()
    {
    }

    [SerializeField] private GameObject clone;
}

[System.Serializable]
public class ElectrocutionEvent : ChanceEvent
{
    // TODO: Electrocute clone
    protected override void TriggerEvent()
    {
    }

    [SerializeField] private GameObject clone;
}

[System.Serializable]
public class RubbishEvent : HundredPercentEvent
{
    // TODO: Put rubbish in front of the clone
    protected override void TriggerEvent()
    {
    }

    [SerializeField] private GameObject clone;
}

[System.Serializable]
public class PuddleEvent : HundredPercentEvent
{
    // TODO: Instantiate a puddle at the clone's position
    protected override void TriggerEvent()
    {
    }

    [SerializeField] private GameObject clone;
}

[System.Serializable]
public class BreakingEvent : OccurenceEvent
{
    // TODO: Break the breakable object
    protected override void TriggerEvent()
    {
        if (breakableObject)
        {

        }
        else
        {
            Debug.LogError("Don't have an object to break!");
        }

        // Don't need object once done
        breakableObject = null;
    }

    private GameObject breakableObject;
    public void SetBreakableObject(GameObject gameObject) { breakableObject = gameObject; }
}

[System.Serializable]
public class DirtyingEvent : OccurenceEvent
{
    // TODO: Dirty the dirtiable object
    protected override void TriggerEvent()
    {
        if (dirtiableObject)
        {

        }
        else
        {
            Debug.LogError("Don't have an object to dirty!");
        }

        // Don't need object once done
        dirtiableObject = null;
    }

    private GameObject dirtiableObject;
    public void SetDirtiableObject(GameObject gameObject) { dirtiableObject = gameObject; }
}