using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Director/Actions/Card")]
public class CardActionData : DirectorActionData
{
    public ChanceCardData chanceCard;

    public override void TriggerAction()
    {
        ManagerHandler.instance.ActionM.AddAction(ActionManager.ActionType.TEST, 0);
        ManagerHandler.instance.director.SetChanceCard(chanceCard);
    }
}