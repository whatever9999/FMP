using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private TextMeshProUGUI startButtonText;
    [SerializeField] private TextMeshProUGUI statsUI;

    private void Start()
    {
        statsUI.text = "Clones Freed: " + SaveManager.instance.GetSave().clonesFreed + "\nClones Killed: " + SaveManager.instance.GetSave().clonesKilled;
    }

    public void StartButton()
    {
        startButtonText.text = "Start...";
        SceneManager.LoadScene(1);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void PanelButton(GameObject panel)
    {
        panel.SetActive(!panel.activeInHierarchy);
    }
}
