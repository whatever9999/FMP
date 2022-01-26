using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private TextMeshProUGUI nameText;

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
}
