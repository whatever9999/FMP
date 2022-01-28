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

    public override bool StartAction()
    {
        started = true;
        return usedObject.StartUsing();
    }
    public override bool ContinueAction()
    {
        completed = usedObject.IsFinished();
        return completed ? true : usedObject.Use();
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
