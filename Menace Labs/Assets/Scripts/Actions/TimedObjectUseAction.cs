using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimedObjectUseAction : Action
{
    [SerializeField] private Image actionImage;
    [SerializeField] private TextMeshProUGUI tooltipText;

    private TimedObject usedObject;
    public void SetObject(TimedObject setTo)
    {
        actionImage.sprite = setTo.GetActionIcon();
        tooltipText.text = setTo.GetTooltip();
        usedObject = setTo;
    }
    public TimedObject GetUsedObject() { return usedObject; }

    public override bool StartAction()
    {
        actionStatus = Action_Status.STARTED;
        return usedObject.StartUsing();
    }
    public override bool ContinueAction()
    {
        if (usedObject.IsFinished()) actionStatus = Action_Status.COMPLETED;
        return actionStatus == Action_Status.COMPLETED ? true : usedObject.Use();
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
