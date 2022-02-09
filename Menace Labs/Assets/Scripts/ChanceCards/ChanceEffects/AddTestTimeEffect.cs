using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/ChanceEffects/AddTestTime")]
public class AddTestTimeEffect : ChanceEffect
{
    public float timeToAdd;

    public override void TriggerEffect()
    {
        ManagerHandler.instance.ActionM.ModifyTestTime(timeToAdd);
    }
}