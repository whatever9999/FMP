using UnityEngine;
using TMPro;

public class FoodManager : MonoBehaviour
{
    [SerializeField] private int maxFoodAmount = 50;
    [SerializeField] private int startFoodAmount = 20;

    [SerializeField] private TextMeshProUGUI foodText;

    private int foodAmount;

    private float supplyIssueTimer;
    private int startSupplyIssue;
    bool supplyIssue = false;
    public bool IsSupplyIssue() { return supplyIssue; }

    private void Start()
    {
        foodAmount = startFoodAmount;
        UpdateUI();
    }

    private void Update()
    {
        int timeSinceSupplyIssue = ManagerHandler.instance.TimeM.TimeSince(startSupplyIssue);
        if (supplyIssue && timeSinceSupplyIssue >= supplyIssueTimer)
        {
            ManagerHandler.instance.NotificationM.AddNotification(NotificationManager.NotificationType.FOOD_SUPPLY_SORTED);
            SetFoodAmount(10);
            supplyIssue = false;
        }
    }

    public void SetSupplyIssueTimer(float length) 
    {
        SetFoodAmount(0);
        supplyIssue = true; 
        supplyIssueTimer = length;
        startSupplyIssue = ManagerHandler.instance.TimeM.GetCurrentTime();
    }

    public void SetFoodAmount(int amount)
    {
        foodAmount = amount;
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
