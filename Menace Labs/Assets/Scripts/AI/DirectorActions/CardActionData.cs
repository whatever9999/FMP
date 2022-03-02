using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Director/Actions/Card")]
public class CardActionData : DirectorActionData
{
    public ChanceCardData chanceCard;

    public override void TriggerAction()
    {
        // Adding a test action cancels all other actions so it should just be added to the end of the list (or it will get confused with its move to use)
        ManagerHandler.instance.ActionM.AddAction(ActionManager.ActionType.TEST, -1);
        ManagerHandler.instance.director.SetChanceCard(chanceCard);
    }
}