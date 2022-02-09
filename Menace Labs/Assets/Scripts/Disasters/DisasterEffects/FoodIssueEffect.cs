using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/DisasterEffects/FoodIssue")]
public class FoodIssueEffect : DisasterEffect
{
    public float issueTimer;

    public override void TriggerEffect()
    {
        // Tell the food manager to stop food from being used for issueTimer seconds
    }
}