using UnityEngine;
using UnityEngine.UI;

public class ShyUI : MonoBehaviour
{
    [SerializeField] private Transform visiblePos;
    [SerializeField] private Transform hiddenPos;

    [SerializeField] private float moveSpeed = 2.0f;

    bool showUI = true;
    public void ToggleUI() 
    { 
        showUI = !showUI;
    }

    private void Update()
    {
        if (Input.GetKeyDown(ManagerHandler.instance.camera.GetKey(CameraHandler.KeyTypes.TOGGLE_UI)))
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
