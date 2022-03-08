using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PopUpManager : MonoBehaviour
{
    #region Chance Card
    public GameObject chanceCardPanel;
    public Image chanceImage;
    public TextMeshProUGUI chanceText;
    public Button optionAButton;
    public Button optionBButton;
    public Button chanceCloseButton;

    public void ShowChanceCard()
    {
        ChanceCardData chanceCard = ManagerHandler.instance.director.GetChanceCard();

        // Ensure correct buttons are active
        optionAButton.gameObject.SetActive(true);
        optionBButton.gameObject.SetActive(true);
        chanceCloseButton.gameObject.SetActive(false);

        // Update info shown
        chanceImage.sprite = chanceCard.image;
        chanceText.text = chanceCard.description;
        optionAButton.GetComponentInChildren<TextMeshProUGUI>().text = chanceCard.A_ButtonText;
        optionBButton.GetComponentInChildren<TextMeshProUGUI>().text = chanceCard.B_ButtonText;

        chanceCardPanel.SetActive(true);
        ManagerHandler.instance.TimeM.SetTimeSpeed(TimeManager.TimeSpeed.PAUSE);
    }

    // A = 0, B = 1
    public void ChanceCardButton(int optionChoice)
    {
        if (optionChoice != 0 && optionChoice != 1) Debug.LogError("Must pass 0(Option A) or 1(Option B) to ChanceCardButton function for PopUpManager");

        ChanceCardData.OptionChoice choice = (ChanceCardData.OptionChoice)optionChoice;
        ChanceCardData chanceCard = ManagerHandler.instance.director.GetChanceCard();
        bool succeeded = chanceCard.MakeChoice(choice);

        switch (choice)
        {
            case ChanceCardData.OptionChoice.OPTION_A:
                {
                    if (succeeded)
                    {
                        chanceText.text = chanceCard.A_SuccessDescription;
                        for (int i = 0; i < chanceCard.A_SuccessEffects.Length; i++)
                        {
                            chanceCard.A_SuccessEffects[i].TriggerEffect();
                        }
                    }
                    else
                    {
                        chanceText.text = chanceCard.A_FailureDescription;
                        for (int i = 0; i < chanceCard.A_FailureEffects.Length; i++)
                        {
                            chanceCard.A_FailureEffects[i].TriggerEffect();
                        }
                    }
                }
                break;
            case ChanceCardData.OptionChoice.OPTION_B:
                {
                    if (succeeded)
                    {
                        chanceText.text = chanceCard.B_SuccessDescription;
                        for (int i = 0; i < chanceCard.B_SuccessEffects.Length; i++)
                        {
                            chanceCard.B_SuccessEffects[i].TriggerEffect();
                        }
                    }
                    else
                    {
                        chanceText.text = chanceCard.B_FailureDescription;
                        for (int i = 0; i < chanceCard.B_FailureEffects.Length; i++)
                        {
                            chanceCard.B_FailureEffects[i].TriggerEffect();
                        }
                    }
                }
                break;
        }

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
        ManagerHandler.instance.TimeM.SetTimeSpeed(TimeManager.TimeSpeed.PAUSE);
    }
    #endregion // Chance Card

    #region Disaster
    public GameObject disasterPanel;
    public TextMeshProUGUI disasterText;

    public void ShowDisaster(DisasterData disaster)
    {
        disasterText.text = disaster.description;

        disasterPanel.SetActive(true);
        ManagerHandler.instance.TimeM.SetTimeSpeed(TimeManager.TimeSpeed.PAUSE);
    }
    #endregion // Chance Card

    #region Death
    public GameObject deathPanel;
    [SerializeField] private TextMeshProUGUI deathDescriptionText;
    [SerializeField] private DeathData[] deathDatas;

    public void ShowDeath(DeathData.DeathTypes death)
    {
        DeathData thisDeath = null;
        for (int i = 0; i < deathDatas.Length; i++)
        {
            if (deathDatas[i].type == death)
            {
                thisDeath = deathDatas[i];
            }
        }

        if (thisDeath)
        {
            // Set panel details
            deathDescriptionText.text = thisDeath.description;

            // Show death card once animation completes
            StartCoroutine(PopupTimer());
        }
        else
        {
            Debug.LogError("Failed to find death of type: " + death);
        }
    }

    public IEnumerator PopupTimer()
    {
        // Cut off a little of the animation time so the clone is still on the floor when the popup shows
        yield return new WaitForSeconds(ManagerHandler.instance.AnimationM.GetAnimationLength(AnimationManager.AnimationType.DIE) - 0.1f);
        deathPanel.SetActive(true);
        ManagerHandler.instance.TimeM.SetTimeSpeed(TimeManager.TimeSpeed.PAUSE);
    }
    #endregion // Chance Card
}
