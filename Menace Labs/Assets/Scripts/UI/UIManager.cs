using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;

    [SerializeField] private TextMeshProUGUI nameText;

    [SerializeField] private TextMeshProUGUI goalNameText;
    [SerializeField] private TextMeshProUGUI goalGoalText;
    [SerializeField] private TextMeshProUGUI goalDescriptionText;
    [SerializeField] private TextMeshProUGUI goalStoryText;
    [SerializeField] private TextMeshProUGUI goalAchievementText;

    [SerializeField] private TextMeshProUGUI ageTooltipText;

    [SerializeField] private ObjectTooltip objectTooltip;

    private float goalUpdateOccurence = 1.0f;
    private float goalUpdateTimer;

    void Start()
    {
        SetCloneName();
        SetAgeTooltip(ManagerHandler.instance.AgeM.GetCloneAge());
        UpdateGoal(ManagerHandler.instance.GoalM.GetGoal());
    }

    private void Update()
    {
        // Update the goal tooltip
        goalUpdateTimer += Time.deltaTime;
        if (goalUpdateTimer > goalUpdateOccurence)
        {
            goalUpdateTimer = 0;
            UpdateGoal(ManagerHandler.instance.GoalM.GetGoal());
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Handle the settings menu before the pause menu
            if (settingsMenu.activeSelf)
            {
                settingsMenu.SetActive(false);
            }
            else
            {
                // If the pause menu is active then we're currently paused so unpause
                if (pauseMenu.activeSelf)
                {
                    ManagerHandler.instance.TimeM.SetSpeedToPrevious();
                    pauseMenu.SetActive(false);
                }
                else
                {
                    ManagerHandler.instance.TimeM.SetTimeSpeed(TimeManager.TimeSpeed.PAUSE, true);
                    pauseMenu.SetActive(true);
                }
            }
        }
    }

    #region Buttons
    public void QuitButton()
    {
        // Reset the time speed so it's correct if we come back to the Game scene
        ManagerHandler.instance.TimeM.SetTimeSpeed(TimeManager.TimeSpeed.PLAY);
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        // Reset the time speed so it's correct if we come back to the Game scene
        ManagerHandler.instance.TimeM.SetTimeSpeed(TimeManager.TimeSpeed.PLAY);
        SceneManager.LoadScene(0);
    }

    public void Restart()
    {
        // Reset the time speed so it's correct if we come back to the Game scene
        ManagerHandler.instance.TimeM.SetTimeSpeed(TimeManager.TimeSpeed.PLAY);
        SceneManager.LoadScene(1);
    }
    #endregion // Buttons

    #region Setup
    private void SetCloneName()
    {
        nameText.text = "Clone #" + Random.Range(0, 9999).ToString();
    }

    // Take in goal info to add to tooltip
    public void SetGoal(string name, string goal, string description, string story)
    {
        goalNameText.text = name;
        goalGoalText.text = goal;
        goalDescriptionText.text = description;
        goalStoryText.text = story;
    }

    public void SetAgeTooltip(int age)
    {
        ageTooltipText.text = age.ToString() + "/6 days remaining";
    }
    #endregion // Setup

    public bool PauseMenuOpen() { return pauseMenu.activeInHierarchy; }

    public bool IsMouseOverUI()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray;

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, Mathf.Infinity, 1 << 6))
            {
                return false;
            }
            return true;
        }
        return false;
    }

    public void UpdateGoal(GoalData goal)
    {
        goalAchievementText.text = goal.GetGoalAchievement();
    }

    public void SetObjectTooltip(ConstantObject objectData)
    {
        objectTooltip.SetupTooltip(objectData);
    }
}
