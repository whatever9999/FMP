using UnityEngine;

public class ShyUI : MonoBehaviour
{
    [SerializeField] private Transform visiblePos;
    [SerializeField] private Transform hiddenPos;

    [SerializeField] private GameObject upArrowButton;
    [SerializeField] private GameObject downArrowButton;

    [SerializeField] private float moveSpeed = 2.0f;

    bool showUI = true;
    public void ToggleUI() 
    { 
        showUI = !showUI;
        upArrowButton.SetActive(!showUI);
        downArrowButton.SetActive(showUI);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleUI();
        }

        float step = moveSpeed * Time.unscaledDeltaTime;
        if (showUI)
        {
            if (Vector3.Distance(transform.position, visiblePos.position) >= 0.01f)
            {
                Vector3 newPosition = Vector3.Lerp(transform.position, visiblePos.position, step);
                transform.position = newPosition;
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, hiddenPos.position) >= 0.01f)
            {
                Vector3 newPosition = Vector3.Lerp(transform.position, hiddenPos.position, step);
                transform.position = newPosition;
            }
        }
    }
}
