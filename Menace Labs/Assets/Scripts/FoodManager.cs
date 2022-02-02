using UnityEngine;
using TMPro;

public class FoodManager : MonoBehaviour
{
    [SerializeField] private int maxFoodAmount = 100;

    [SerializeField] private TextMeshProUGUI foodText;

    private int foodAmount;

    private void Start()
    {
        foodAmount = maxFoodAmount;
        UpdateUI();
    }

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
        UpdateUI();
    }

    public bool GotEnoughFood(int amountNeeded)
    {
        if (foodAmount >= amountNeeded) return true;
        return false;
    }

    private void UpdateUI()
    {
        foodText.text = foodAmount.ToString();
    }
}
