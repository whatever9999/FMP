using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Director/Actions/Disaster")]
public class DisasterActionData : DirectorActionData
{
    public DisasterData disaster;

    public override void TriggerAction()
    {
        disaster.disasterEffect.TriggerEffect();
        ManagerHandler.instance.PopupM.ShowDisaster();
    }
}