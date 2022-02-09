using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/ChanceEffects/IncreaseFood")]
public class IncreaseFoodEffect : ChanceEffect
{
    public int amount;

    public override void TriggerEffect()
    {
        ManagerHandler.instance.FoodM.ModifyFoodAmount(amount);
    }
}
