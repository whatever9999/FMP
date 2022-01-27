using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    public enum EventType
    {
        FIRE,
        ELECTROCUTION,
        RUBBISH,
        PUDDLE,
        BREAKING,
        DIRTYING,
        NUM_EVENT_TYPES,
    }

    [SerializeField] private FireEvent fireEvent;
    [SerializeField] private ElectrocutionEvent electrocutionEvent;
    [SerializeField] private RubbishEvent rubbishEvent;
    [SerializeField] private PuddleEvent puddleEvent;
    [SerializeField] private BreakingEvent breakingEvent;
    [SerializeField] private DirtyingEvent dirtyingEvent;

    private Dictionary<EventType, Event> events = new Dictionary<EventType, Event>();

    private void Start()
    {
        instance = this;

        // Populate the events dictionary with the different event types
        events.Add(EventType.FIRE, fireEvent);
        events.Add(EventType.ELECTROCUTION, electrocutionEvent);
        events.Add(EventType.RUBBISH, rubbishEvent);
        events.Add(EventType.PUDDLE, puddleEvent);
        events.Add(EventType.BREAKING, breakingEvent);
        events.Add(EventType.DIRTYING, dirtyingEvent);
    }

    // Get the event requested and check its trigger
    public void CheckEventTrigger(EventType eventType)
    {
        Event checkEvent;
        bool gotEvent = events.TryGetValue(eventType, out checkEvent);

        if (gotEvent) checkEvent.CheckTrigger();
        else Debug.LogError("Failed to get event of type " + eventType);
    }
}
