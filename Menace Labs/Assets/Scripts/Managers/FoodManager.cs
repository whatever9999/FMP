using UnityEngine;
using TMPro;

public class FoodManager : MonoBehaviour
{
    [SerializeField] private int maxFoodAmount = 100;

    [SerializeField] private TextMeshProUGUI foodText;

    private int foodAmount;

    private float supplyIssueTimer;
    public void SetSupplyIssueTimer(float length) { supplyIssue = true;  supplyIssueTimer = length; }
    bool supplyIssue = false;

    private void Start()
    {
        foodAmount = maxFoodAmount;
        UpdateUI();
    }

    private void Update()
    {
        supplyIssueTimer -= Time.deltaTime;
        if (supplyIssue && supplyIssueTimer <= 0)
        {
            supplyIssue = false;
        }
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

    // If there's a supply issue we can't use food
    public bool GotEnoughFood(int amountNeeded)
    {
        if (!supplyIssue && foodAmount >= amountNeeded) return true;
        return false;
    }

    private void UpdateUI()
    {
        foodText.text = foodAmount.ToString();
    }
}
