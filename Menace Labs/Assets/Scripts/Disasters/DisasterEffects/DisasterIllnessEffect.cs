using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/DisasterEffects/DisasterIllness")]
public class DisasterIllnessEffect : DisasterEffect
{
    public override void TriggerEffect()
    {
        base.TriggerEffect();

        ManagerHandler.instance.NeedsM.SetIll(true);
    }
}