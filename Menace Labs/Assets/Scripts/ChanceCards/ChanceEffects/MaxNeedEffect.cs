using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/ChanceEffects/MaxNeed")]
public class MaxNeedEffect : ChanceEffect
{
    public NeedsManager.NeedType needType;

    public override void TriggerEffect()
    {
        ManagerHandler.instance.NeedsM.SetNeed(needType, NeedsManager.MAX_NEED_VALUE);
    }
}