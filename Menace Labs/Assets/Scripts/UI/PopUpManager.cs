using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopUpManager : MonoBehaviour
{
    #region Chance Card
    public GameObject chanceCardPanel;
    public Image chanceImage;
    public TextMeshProUGUI chanceText;
    public Button optionAButton;
    public Button optionBButton;
    public Button chanceCloseButton;

    // TODO: Take in chance card data and update info accordingly
    public void ShowChanceCard()
    {
        // Ensure correct buttons are active
        optionAButton.gameObject.SetActive(true);
        optionBButton.gameObject.SetActive(true);
        chanceCloseButton.gameObject.SetActive(false);

        // Update info shown
        chanceImage.sprite = chanceImage.sprite;
        chanceText.text = "Text to explain options";
        optionAButton.GetComponentInChildren<Text>().text = "Option A";
        optionBButton.GetComponentInChildren<Text>().text = "Option B";

        chanceCardPanel.SetActive(true);
        UIManager.instance.TM.SetTimeSpeed(TimeManager.TimeSpeed.PAUSE);
    }

    public void OptionAButton()
    {
        // TODO: Carry out results of selecting A

        optionAButton.gameObject.SetActive(false);
        optionBButton.gameObject.SetActive(false);
        chanceCloseButton.gameObject.SetActive(true);
    }
    public void OptionBButton()
    {
        // TODO: Carry out results of selecting B

        optionAButton.gameObject.SetActive(false);
        optionBButton.gameObject.SetActive(false);
        chanceCloseButton.gameObject.SetActive(true);
    }
    #endregion // Chance Card

    #region Win Game
    public GameObject winGamePanel;
    public void ShowWinGame()
    {
        winGamePanel.SetActive(true);
        UIManager.instance.TM.SetTimeSpeed(TimeManager.TimeSpeed.PAUSE);
    }
    #endregion // Chance Card

    #region Disaster
    public GameObject disasterPanel;
    public TextMeshProUGUI disasterText;

    // TODO: Take in disaster data and update accordingly
    public void ShowDisaster()
    {
        disasterText.text = "Explanation of the disaster";

        disasterPanel.SetActive(true);
        UIManager.instance.TM.SetTimeSpeed(TimeManager.TimeSpeed.PAUSE);
    }
    #endregion // Chance Card

    #region Death
    public GameObject deathPanel;
    public TextMeshProUGUI deathText;

    // TODO: Take in death data and update accordingly
    public void ShowDeath()
    {
        disasterText.text = "Explanation of the death";

        deathPanel.SetActive(true);
        UIManager.instance.TM.SetTimeSpeed(TimeManager.TimeSpeed.PAUSE);
    }
    #endregion // Chance Card
}
