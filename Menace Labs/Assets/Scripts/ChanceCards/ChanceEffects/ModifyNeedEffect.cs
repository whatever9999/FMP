using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/ChanceEffects/ModifyNeed")]
public class ModifyNeedEffect : ChanceEffect
{
    public NeedsManager.NeedType needType;
    public float amount;

    public override void TriggerEffect()
    {
        ManagerHandler.instance.NeedsM.ModifyNeed(needType, amount);
    }
}