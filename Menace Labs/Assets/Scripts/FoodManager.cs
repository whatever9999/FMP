using UnityEngine;

public class FoodManager : MonoBehaviour
{
    public int maxFoodAmount = 100;

    private int foodAmount;

    public void ModifyFoodAmount(int amount) 
    {
        foodAmount += amount;
        if (foodAmount < 0)
        {
            foodAmount = 0;
            Debug.LogError("Food is trying to be negative!");
        }
        else if (foodAmount > maxFoodAmount)
        {
            foodAmount = maxFoodAmount;
        }
    }

    public bool GotEnoughFood(int amountNeeded)
    {
        if (foodAmount >= amountNeeded) return true;
        return false;
    }
}
