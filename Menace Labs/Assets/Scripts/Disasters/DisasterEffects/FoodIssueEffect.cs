using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/DisasterEffects/FoodIssue")]
public class FoodIssueEffect : DisasterEffect
{
    public float issueTimer;

    public override void TriggerEffect()
    {
        base.TriggerEffect();

        ManagerHandler.instance.FoodM.SetSupplyIssueTimer(issueTimer);
    }
}