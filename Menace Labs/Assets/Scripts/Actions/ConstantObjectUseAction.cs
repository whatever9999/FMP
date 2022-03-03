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
    public ConstantObject GetUsedObject() { return usedObject; }

    public override bool StartAction()
    {
        actionStatus = Action_Status.STARTED;
        return usedObject.StartUsing();
    }
    public override bool ContinueAction()
    {
        return usedObject.Use();
    }
    public override void EndAction()
    {
        base.EndAction();
        usedObject.FinishUsing();
    }
    public override void CancelAction()
    {
        base.CancelAction();
        usedObject.CancelUsing();
    }
}
