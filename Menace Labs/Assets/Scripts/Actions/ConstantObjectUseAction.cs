using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConstantObjectUseAction : Action
{
    [SerializeField] private Image actionImage;
    [SerializeField] private TextMeshProUGUI tooltipText;

    private ConstantObject usedObject;
    public void SetObject(ConstantObject setTo) 
    {
        actionImage.sprite = setTo.GetActionIcon();
        tooltipText.text = setTo.GetTooltip();
        usedObject = setTo; 
    }

    public override bool StartAction()
    {
        started = true;
        return usedObject.StartUsing();
    }
    public override bool ContinueAction()
    {
        return usedObject.Use();
    }
    public override void EndAction()
    {
        usedObject.FinishUsing();
    }
    public override void CancelAction()
    {
        usedObject.CancelUsing();
    }
}
