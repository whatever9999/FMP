using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/DisasterEffects/DisasterIllness")]
public class DisasterIllnessEffect : DisasterEffect
{
    public override void TriggerEffect()
    {
        ManagerHandler.instance.NeedsM.SetIll(true);
    }
}