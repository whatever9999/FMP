using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Director/Actions/ChanceChange")]
public class ChanceChangeActionData : DirectorActionData
{
    public EventManager.EventType eventType;
    public float newValue;

    public override void TriggerAction()
    {
        ManagerHandler.instance.EventM.ModifyEventChance(eventType, newValue);
    }
}