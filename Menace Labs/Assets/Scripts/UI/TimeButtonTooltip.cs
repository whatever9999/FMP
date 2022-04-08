using UnityEngine;
using TMPro;

public class TimeButtonTooltip : MonoBehaviour
{
    [SerializeField] private CameraHandler.KeyTypes keyType;
    private TextMeshProUGUI text;

    private void Awake()
    {
        if (!text) text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = "<" + SaveManager.instance.GetControl(keyType).ToString() + ">";
    }
}
