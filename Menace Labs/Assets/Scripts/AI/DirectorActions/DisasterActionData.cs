using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Director/Actions/Disaster")]
public class DisasterActionData : DirectorActionData
{
    public DisasterData disaster;

    public override void TriggerAction()
    {
        ManagerHandler.instance.MenaceMetric.ResetTimer(MenaceMetric.MenaceData.TIME_SINCE_DISASTER);
        disaster.disasterEffect.TriggerEffect();
        ManagerHandler.instance.PopupM.ShowDisaster(disaster);
    }
}