using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public enum EventType
    {
        FIRE,
        ELECTROCUTION,
        RUBBISH,
        PUDDLE,
        BLADDER_FAILURE,
        BREAKING,
        DIRTYING,
        DEATH,
        PASS_OUT,
        FOOD_DELIVERY,
        DAY_START,
        NIGHT_START,
        NUM_EVENT_TYPES,
    }

    [SerializeField] private FireEvent fireEvent;
    [SerializeField] private ElectrocutionEvent electrocutionEvent;
    [SerializeField] private RubbishEvent rubbishEvent;
    [SerializeField] private PuddleEvent puddleEvent;
    [SerializeField] private BladderFailureEvent bladderFailureEvent;
    [SerializeField] private BreakingEvent breakingEvent;
    [SerializeField] private DirtyingEvent dirtyingEvent;
    [SerializeField] private DeathEvent deathEvent;
    [SerializeField] private PassOutEvent passOutEvent;
    [SerializeField] private FoodDeliveryEvent foodDeliveryEvent;
    [SerializeField] private DayStartEvent dayStartEvent;
    [SerializeField] private NightStartEvent nightStartEvent;

    private Dictionary<EventType, Event> events = new Dictionary<EventType, Event>();

    private void Awake()
    {
        // Populate the events dictionary with the different event types
        events.Add(EventType.FIRE, fireEvent);
        events.Add(EventType.ELECTROCUTION, electrocutionEvent);
        events.Add(EventType.RUBBISH, rubbishEvent);
        events.Add(EventType.PUDDLE, puddleEvent);
        events.Add(EventType.BREAKING, breakingEvent);
        events.Add(EventType.DIRTYING, dirtyingEvent);
        events.Add(EventType.DEATH, deathEvent);
    }

    // Get the event requested and check its trigger
    public bool CheckEventTrigger(EventType eventType)
    {
        Event checkEvent;
        bool gotEvent = events.TryGetValue(eventType, out checkEvent);

        if (gotEvent) return checkEvent.CheckTrigger();
        else Debug.LogError("Failed to get event of type " + eventType);
        return false;
    }

    public void SetDirtiableObject(ConstantObject dirtiableObject)
    {
        dirtyingEvent.SetDirtiableObject(dirtiableObject);
    }
    public void SetBreakableObject(ConstantObject breakableObject)
    {
        breakingEvent.SetBreakableObject(breakableObject);
    }
}
