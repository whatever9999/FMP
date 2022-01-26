using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    void Start()
    {
        instance = this;
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
