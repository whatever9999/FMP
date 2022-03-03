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
        events.Add(EventType.BLADDER_FAILURE, bladderFailureEvent);
        events.Add(EventType.BREAKING, breakingEvent);
        events.Add(EventType.DIRTYING, dirtyingEvent);
        events.Add(EventType.DEATH, deathEvent);
        events.Add(EventType.PASS_OUT, passOutEvent);
        events.Add(EventType.FOOD_DELIVERY, foodDeliveryEvent);
        events.Add(EventType.DAY_START, dayStartEvent);
        events.Add(EventType.NIGHT_START, nightStartEvent);
    }

    // Get the event requested and check its trigger
    public bool CheckEventTrigger(EventType eventType)
    {
        Event checkEvent;
        bool gotEvent = events.TryGetValue(eventType, out checkEvent);

        if (gotEvent)
        {
            bool eventSucceeded = checkEvent.CheckTrigger();
            ManagerHandler.instance.MenaceMetric.ResetTimer(MenaceMetric.MenaceData.TIME_SINCE_EVENT);
            return eventSucceeded;
        }
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

    public void ModifyEventChance(EventType eventType, float newChance)
    {
        switch (eventType)
        {
            case EventType.FIRE:
                fireEvent.SetChance(newChance);
                break;
            case EventType.ELECTROCUTION:
                electrocutionEvent.SetChance(newChance);
                break;
            default:
                Debug.LogError("Trying to modify the event chance for an event without chance");
                break;
        }
    }
    public void ModifyEventOccurrence(EventType eventType, int newOccurrence)
    {
        switch (eventType)
        {
            case EventType.BREAKING:
                breakingEvent.SetOccurrence(newOccurrence);
                break;
            case EventType.DIRTYING:
                dirtyingEvent.SetOccurrence(newOccurrence);
                break;
            default:
                Debug.LogError("Trying to modify the event occurrence for an event without occurrence");
                break;
        }
    }
}
