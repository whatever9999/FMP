using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/ChanceEffects/EndTest")]
public class EndTestEffect : ChanceEffect
{
    public override void TriggerEffect()
    {
        ManagerHandler.instance.ActionM.EndTest();
    }
}