using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/ChanceEffects/Illness")]
public class IllnessEffect : ChanceEffect
{
    public bool makeIll;

    public override void TriggerEffect()
    {
        ManagerHandler.instance.NeedsM.SetIll(true);
    }
}