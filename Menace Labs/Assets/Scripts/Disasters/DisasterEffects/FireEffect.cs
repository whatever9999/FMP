using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/DisasterEffects/Fire")]
public class FireEffect : DisasterEffect
{
    public int number;

    public override void TriggerEffect()
    {
        // Spawn number fires randomly around map
    }
}