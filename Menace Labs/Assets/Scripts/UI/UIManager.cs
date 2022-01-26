using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private TextMeshProUGUI nameText;

    [SerializeField] private TextMeshProUGUI goalNameText;
    [SerializeField] private TextMeshProUGUI goalDescriptionText;
    [SerializeField] private TextMeshProUGUI goalStoryText;

    [SerializeField] private TextMeshProUGUI ageTooltipText;

    void Start()
    {
        instance = this;

        SetCloneName();
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    private void SetCloneName()
    {
        nameText.text = "Clone #" + Random.Range(0, 9999).ToString();
    }

    // TODO: Take in goal info to add to tooltip
    private void SetGoal()
    {
        goalNameText.text = "Goal Name";
        goalDescriptionText.text = "Goal Description";
        goalStoryText.text = "Goal Story";
    }

    // TODO: Get max number of days from the AgeManager
    private void SetAgeTooltip(int age)
    {
        ageTooltipText.text = age.ToString() + "/6 days remaining";
    }
}
