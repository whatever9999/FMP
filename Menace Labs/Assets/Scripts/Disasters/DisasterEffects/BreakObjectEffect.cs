using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/DisasterEffects/BreakObject")]
public class BreakObjectEffect : DisasterEffect
{
    public int number;

    public override void TriggerEffect()
    {
        // Break number objects randomly around map
    }
}