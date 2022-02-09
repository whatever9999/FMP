using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/DisasterEffects/Puddle")]
public class PuddleEffect : DisasterEffect
{
    public int number;

    public override void TriggerEffect()
    {
        // Spawn number puddles randomly around map
    }
}