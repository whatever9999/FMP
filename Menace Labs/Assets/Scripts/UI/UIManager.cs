using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] public PopUpManager PUM;
    [SerializeField] public TimeManager TM;

    [SerializeField] private TextMeshProUGUI nameText;

    [SerializeField] private TextMeshProUGUI goalNameText;
    [SerializeField] private TextMeshProUGUI goalDescriptionText;
    [SerializeField] private TextMeshProUGUI goalStoryText;

    [SerializeField] private TextMeshProUGUI ageTooltipText;

    void Start()
    {
        instance = this;

        SetCloneName();
        SetGoal();
        // TODO: Get max number of days from the AgeManager to use on start
        SetAgeTooltip(6);
    }

    #region Buttons
    public void QuitButton()
    {
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Restart()
    {
        SceneManager.LoadScene(1);
    }
    #endregion // Buttons

    #region Setup
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
    public void SetAgeTooltip(int age)
    {
        ageTooltipText.text = age.ToString() + "/6 days remaining";
    }
    #endregion // Setup
}
