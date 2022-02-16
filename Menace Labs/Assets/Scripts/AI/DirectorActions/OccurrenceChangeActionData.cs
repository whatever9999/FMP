using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Director/Actions/OccurrenceChange")]
public class OccurrenceChangeActionData : DirectorActionData
{
    public EventManager.EventType eventType;
    public int newValue;

    public override void TriggerAction()
    {
        ManagerHandler.instance.EventM.ModifyEventOccurrence(eventType, newValue);
    }
}
